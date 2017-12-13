using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{//2017-12-7: copied from DashGuy.PlayerController

    [Header("Settings")]
    [Range(1, 10)]
    public float walkSpeed = 1.0f;
    public float moveThreshold = 0.1f;//how close to the target pos Knight will stop moving
    public float weaponRange = 1;//how close enemies have to be for Knight to hit them
    public int weaponDamage = 100;//how much damage the weapon does
    public float weaponAttackRate = 60;//how many times per minute you can attack
    [Range(1, 10)]
    public int dashFrames = 2;//how many frames it takes to complete the dash
    public Vector2 spawnPoint = Vector2.zero;//where the player respawns when he dies

    //Processing Variables
    private Vector2 targetPos;//the position that Knight wants to move to
    public GameObject targetObj;//the object that was tapped; Knight will interact with it when he gets to it
    private bool reachedDestination = true;//whether or not he's reached the target position
    private bool collidedWithTarget = false;
    private float halfWidth = 0;//half of Merky's sprite width
    private int removeVelocityFrames = 0;
    private float lastWeaponAttackTime;
    private float weaponAttackDelay;//the delay between each attack (sec)

    //Components
    private CameraController mainCamCtr;//the camera controller for the main camera
    private Rigidbody2D rb2d;
    private PolygonCollider2D pc2d;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        pc2d = GetComponent<PolygonCollider2D>();
        animator = GetComponent<Animator>();
        mainCamCtr = Camera.main.GetComponent<CameraController>();
        halfWidth = GetComponent<SpriteRenderer>().bounds.extents.magnitude;
        targetPos = transform.position;
        weaponAttackDelay = 60 / weaponAttackRate;
    }

    void FixedUpdate()
    {
        //Target Object
        if (targetObj != null)
        {
            if (collidedWithTarget || Utility.withinRange(gameObject, targetObj, weaponRange))
            {
                targetPos = transform.position;
                HealthPool hp = targetObj.GetComponent<HealthPool>();
                if (hp)
                {
                    if (Time.time > lastWeaponAttackTime + weaponAttackDelay)
                    {
                        animator.SetBool("isAttacking", true);
                        lastWeaponAttackTime = Time.time;
                        collidedWithTarget = false;
                    }
                }
            }
            else
            {
                //If the target can move,
                if (targetObj.GetComponent<Rigidbody2D>())
                {
                    //chase it.
                    targetPos = targetObj.transform.position;
                }
            }
        }
        //Target Position
        if (!reachedDestination)
        {
            //Sprite Flipping
            if (targetPos != null || targetObj != null)
            {
                Vector3 scale = transform.localScale;
                scale.x = (targetPos.x < transform.position.x
                    || (targetObj != null && targetObj.transform.position.x < transform.position.x))
                    ? -1 : 1;
                transform.localScale = scale;
            }
            //Target Position
            if (!Utility.withinRange(targetPos, transform.position, moveThreshold))
            {
                rb2d.velocity = (targetPos - (Vector2)transform.position).normalized * walkSpeed;
            }
            else
            {
                rb2d.velocity = Vector2.zero;
                reachedDestination = true;
            }
        }
        //Removed Velocity Frames
        if (removeVelocityFrames >= 0)
        {
            removeVelocityFrames--;
            if (removeVelocityFrames < 0)
            {
                rb2d.velocity = Vector2.zero;
            }
        }
        //Camera Movement Delay
        if (!isMoving())
        {
            mainCamCtr.discardMovementDelay();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == targetObj)
        {
            collidedWithTarget = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject == targetObj)
        {
            if (animator.GetBool("isAttacking"))
            {
                HealthPool hp = targetObj.GetComponent<HealthPool>();
                if (hp)
                {
                    hp.addHealthPoints(-weaponDamage);
                }
                targetObj = null;
            }
            else
            {
                collidedWithTarget = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject == targetObj)
        {
            collidedWithTarget = true;
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
        reachedDestination = false;
    }
    public void processTapGesture(Vector3 gpos, GameObject targetObj)
    {
        processTapGesture(gpos);
        if (targetObj != gameObject)//don't target yourself
        {
            this.targetObj = targetObj;
        }
    }


    public void processHoldGesture(Vector3 gpos, float holdTime, bool finished)
    {
        targetPos = gpos;
        reachedDestination = false;
    }
    public void dropHoldGesture()
    {
    }

    //
    // Animation Methods
    //
    void stopAttacking()
    {
        animator.SetBool("isAttacking", false);
        targetObj = null;
    }
}


