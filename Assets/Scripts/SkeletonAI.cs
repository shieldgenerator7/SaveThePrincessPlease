using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent((typeof(PlayerController)))]
public class SkeletonAI : MonoBehaviour {

    public GameObject initialTarget;//usually a bridge

    private PlayerController controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<PlayerController>();
        controller.processTapGesture(initialTarget);
	}
	
	// Update is called once per frame
	void Update () {
		if (controller.targetObj == null)
        {
            controller.processTapGesture(initialTarget);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        controller.targetObj = null;
        HealthPool hp = collision.gameObject.GetComponent<HealthPool>();
        if (hp)
        {
            controller.processTapGesture(collision.gameObject);
            hp.onDeath += refocus;
        }
    }

    void refocus()
    {
        controller.processTapGesture(initialTarget);
    }
}
