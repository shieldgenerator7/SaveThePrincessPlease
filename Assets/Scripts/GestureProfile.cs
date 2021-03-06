﻿using UnityEngine;
using System.Collections;

public class GestureProfile
{//2017-12-7: copied from DashGuy.GestureProfile

    protected PlayerController plrController;
    protected Rigidbody2D rb2dPlayer;
    //protected Camera cam;
    //protected CameraController cmaController;

    public GestureProfile()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        plrController = player.GetComponent<PlayerController>();
        rb2dPlayer = player.GetComponent<Rigidbody2D>();
        //cam = Camera.main;
        //cmaController = cam.GetComponent<CameraController>();
    }
    public virtual void processTapGesture(Vector3 curMPWorld, RaycastHit2D[] rch2ds)
    {
        plrController.processTapGesture(curMPWorld, rch2ds);
    }
    public virtual void processTapGesture(Vector3 curMPWorld, GameObject go)
    {
        plrController.processTapGesture(curMPWorld, go);
    }
    public virtual void processTapGesture(Vector3 curMPWorld)
    {
        plrController.processTapGesture(curMPWorld);
    }
    public virtual void processHoldGesture(Vector3 curMPWorld, float holdTime, bool finished)
    {
        plrController.processHoldGesture(curMPWorld, holdTime, finished);
    }
    public void processDragGesture()
    {

    }
    public void processPinchGesture()
    {

    }
}
