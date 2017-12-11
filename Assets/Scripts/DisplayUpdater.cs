using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUpdater : MonoBehaviour {

    private SpriteRenderer sr;

    // Use this for initialization
    void Start ()
    {
        sr = GetComponent<SpriteRenderer>();
        updateDisplayOrder();
        //If this object is stationary,
        if (GetComponent<Rigidbody2D>() == null)
        {
            //Remove this script after setting the display order
            Destroy(this);
        }
    }
	
	// Update is called once per frame
	void Update () {
        updateDisplayOrder();
    }

    void updateDisplayOrder()
    {
        sr.sortingOrder = -(int)(transform.position.y * 10);
    }
}
