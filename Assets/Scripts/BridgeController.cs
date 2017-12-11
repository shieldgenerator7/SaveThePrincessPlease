using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent((typeof(HealthPool)))]
public class BridgeController : MonoBehaviour {

    public GameObject support;//the sub GameObject that has the sprite that will be used as the health bar
    public GameObject planksIntact;//the object with intact planks
    public GameObject planksDestroyed;//the object with broken planks

    private float baseScaleX = 1;//the base scale x of the support object
    private HealthPool hp;
    private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        hp = GetComponent<HealthPool>();
        hp.onHealthLost += takeDamage;
        hp.onDeath += destroyBridge;
        sr = support.GetComponent<SpriteRenderer>();
        baseScaleX = sr.size.x;
        foreach (Transform t in transform)
        {
            t.gameObject.AddComponent<DisplayUpdater>();
        }
	}
	
    void takeDamage()
    {
        sr.size = new Vector2(baseScaleX * hp.HP / hp.maxHP,1);
    }

    void destroyBridge()
    {
        planksIntact.SetActive(false);
        planksDestroyed.SetActive(true);
        //Also disable the trigger on this object
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
