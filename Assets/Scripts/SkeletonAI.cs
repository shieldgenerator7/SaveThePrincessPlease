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
        controller.processTapGesture(initialTarget.transform.position, initialTarget);
        GetComponent<HealthPool>().onHealthLost += changeTargetTemporarily;
	}
	
	// Update is called once per frame
	void Update () {
		if (controller.targetObj == null)
        {
            controller.processTapGesture(initialTarget.transform.position, initialTarget);
        }
	}

    private void OnCollisionEnter2D(Collision2D coll)
    {
        changeTargetTemporarily(coll.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        changeTargetTemporarily(coll.gameObject);
    }
    void changeTargetTemporarily(GameObject go)
    {
        HealthPool hp = go.GetComponentInParent<HealthPool>();
        if (hp)
        {
            controller.targetObj = null;
            controller.processTapGesture(transform.position, go);
            hp.onDeath += refocus;
        }
    }

    void refocus()
    {
        if (controller)
        {
            controller.processTapGesture(initialTarget.transform.position, initialTarget);
        }
    }
}
