using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthPool))]
public class KnightController : MonoBehaviour {

    public float knockBackDistance = 5f;//how far the knight gets knocked back when he gets hit

    private HealthPool hp;
    private PlayerController pc;
    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        hp = GetComponent<HealthPool>();
        hp.onHealthLost += knockBackFromDamage;
        pc = GetComponent<PlayerController>();
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	void knockBackFromDamage(GameObject attacker)
    {
        transform.position = transform.position + (transform.position - attacker.transform.position).normalized * knockBackDistance;
        rb2d.velocity = Vector2.zero;
        pc.cancelMovement();
    }
}
