﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{//2017-12-7: copied from DashGuy.PlayerController

    [Header("Settings")]
    [Range(1, 10)]
    public float walkSpeed = 1.0f;
    public float moveThreshold = 0.1f;//how close to the target pos Knight will stop moving
    public float weaponRange = 0.2f;//how close enemies have to be for Knight to hit them
    public int weaponDamage = 100;//how much damage the weapon does
    [Range(1, 10)]
    public int dashFrames = 2;//how many frames it takes to complete the dash
    public Vector2 spawnPoint = Vector2.zero;//where the player respawns when he dies

    //Processing Variables
    private Vector2 targetPos;//the position that Knight wants to move to
    private GameObject targetObj;//the object that was tapped; Knight will interact with it when he gets to it
    private float halfWidth = 0;//half of Merky's sprite width
    private int removeVelocityFrames = 0;

    //Components
    private CameraController mainCamCtr;//the camera controller for the main camera
    private Rigidbody2D rb2d;
    private PolygonCollider2D pc2d;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        pc2d = GetComponent<PolygonCollider2D>();
        mainCamCtr = Camera.main.GetComponent<CameraController>();
        halfWidth = GetComponent<SpriteRenderer>().bounds.extents.magnitude;
    }

    void FixedUpdate()
    {
        if (!Utility.withinRange(targetPos, transform.position, moveThreshold))
        {
            rb2d.velocity = (targetPos - (Vector2)transform.position).normalized * walkSpeed;
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
        if (targetObj != null && Utility.withinRange(targetObj, gameObject, weaponRange))
        {
            HealthPool hp = targetObj.GetComponent<HealthPool>();
            if (hp)
            {
                hp.addHealthPoints(-weaponDamage);
            }
        }
        if (removeVelocityFrames >= 0)
        {
            removeVelocityFrames--;
            if (removeVelocityFrames < 0)
            {
                rb2d.velocity = Vector2.zero;
            }
        }
        if (!isMoving())
        {
            mainCamCtr.discardMovementDelay();
        }
    }

    /// <summary>
    /// Whether or not Merky is moving
    /// Does not consider rotation
    /// </summary>
    /// <returns></returns>
    bool isMoving()
    {
        return rb2d.velocity.magnitude >= 0.1f;
    }

    private bool teleport(Vector3 targetPos)//targetPos is in world coordinations (NOT UI coordinates)
    {
        //Get new position
        Vector3 newPos = targetPos;
        this.targetPos = targetPos;

        //Actually Teleport
        Vector3 oldPos = transform.position;
        Vector3 direction = newPos - oldPos;
        float distance = Vector3.Distance(oldPos, newPos);
        RaycastHit2D[] rch2ds = new RaycastHit2D[1];
        rch2ds = Physics2D.RaycastAll(transform.position, direction, distance);
        if (rch2ds.Length > 1 && rch2ds[1]
            && rch2ds[1].collider.gameObject.GetComponent<Rigidbody2D>() == null
            && !rch2ds[1].collider.isTrigger)
        {
            distance = rch2ds[1].distance;
            newPos = oldPos + direction.normalized * distance;
        }
        float dashSpeed = distance / (Time.deltaTime * dashFrames);
        rb2d.velocity = direction.normalized * walkSpeed;
        removeVelocityFrames = dashFrames;

        //Momentum Dampening
        if (rb2d.velocity.magnitude > 0.001f)//if Merky is moving
        {
            float newX = rb2d.velocity.x;//the new x velocity
            float newY = rb2d.velocity.y;
            if (Mathf.Sign(rb2d.velocity.x) != Mathf.Sign(direction.x))
            {
                newX = rb2d.velocity.x + direction.x;
                if (Mathf.Sign(rb2d.velocity.x) != Mathf.Sign(newX))
                {//keep from exploiting boost in opposite direction
                    newX = 0;
                }
            }
            if (Mathf.Sign(rb2d.velocity.y) != Mathf.Sign(direction.y))
            {
                newY = rb2d.velocity.y + direction.y;
                if (Mathf.Sign(rb2d.velocity.y) != Mathf.Sign(newY))
                {//keep from exploiting boost in opposite direction
                    newY = 0;
                }
            }
            rb2d.velocity = new Vector2(newX, newY);
        }
        //Gravity Immunity
        mainCamCtr.delayMovement(0.3f);
        return true;
    }

    /// <summary>
    /// Sets the spawn point to the given position
    /// </summary>
    /// <param name="newSpawnPoint"></param>
    public void setSpawnPoint(Vector2 newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
    }
    /// <summary>
    /// Kills the player and sends him back to spawn
    /// </summary>
    public void kill()
    {
        transform.position = spawnPoint;
    }

    /// <summary>
    /// Returns true if the given Vector3 is on Merky's sprite
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public bool gestureOnPlayer(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) < halfWidth;
    }

    public void processTapGesture(Vector3 gpos)
    {
        Vector3 prevPos = transform.position;
        Vector3 newPos = gpos;
        teleport(newPos);
    }
    public void processTapGesture(GameObject targetObj)
    {
        processTapGesture(targetObj.transform.position);
        this.targetObj = targetObj;
    }


    public void processHoldGesture(Vector3 gpos, float holdTime, bool finished)
    {
        targetPos = gpos;
    }
    public void dropHoldGesture()
    {
    }
}

