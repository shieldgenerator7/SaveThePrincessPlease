﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{//2017-12-7: copied from Stonicorn.Utility

    /// <summary>
    /// Returns the given vector rotated by the given angle
    /// 2017-02-21: copied from a post by wpennypacker: https://forum.unity3d.com/threads/vector-rotation.33215/
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    public static Vector3 RotateZ(Vector3 v, float angle)
    {
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);

        float tx = (cos * v.x) - (sin * v.y);
        float ty = (cos * v.y) + (sin * v.x);

        return new Vector3(tx, ty);
    }
    /// <summary>
    /// Returns the angle of the given vector
    /// 2017-04-18: copied from an answer by Sigil: http://webcache.googleusercontent.com/search?q=cache:http://answers.unity3d.com/questions/162177/vector2angles-direction.html&num=1&strip=1&vwsrc=0
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    public static float RotationZ(Vector3 v1, Vector3 v2)
    {
        float angle = Vector2.Angle(v1, v2);
        Vector3 cross = Vector3.Cross(v1, v2);
        if (cross.z > 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }

    public static Vector3 PerpendicularRight(Vector3 v)
    {
        return RotateZ(v, -Mathf.PI / 2);
    }
    public static Vector3 PerpendicularLeft(Vector3 v)
    {
        return RotateZ(v, Mathf.PI / 2);
    }

    /**
    * 2016-03-25: copied from "2D Explosion Force" Asset: https://www.assetstore.unity3d.com/en/#!/content/24077
    * 2016-03-29: moved here from PlayerController
    * 2017-03-09: moved here from ForceTeleportAbility
    */
    public static void AddExplosionForce(Rigidbody2D body, float expForce, Vector3 expPosition, float expRadius)
    {
        var dir = (body.transform.position - expPosition);
        float calc = 1 - (dir.magnitude / expRadius);
        if (calc <= 0)
        {
            calc = 0;
        }

        body.AddForce(dir.normalized * expForce * calc);
    }
    /// <summary>
    /// Adds explosion force to the given Rigidbody2D based in part on its own mass
    /// </summary>
    /// <param name="body"></param>
    /// <param name="expForce"></param>
    /// <param name="expPosition"></param>
    /// <param name="expRadius"></param>
    /// <param name="maxForce">The maximum amount of force that can be applied</param>
    public static void AddWeightedExplosionForce(Rigidbody2D body, float expForce, Vector3 expPosition, float expRadius, float maxForce)
    {
        Vector2 dir = (body.transform.position - expPosition).normalized;
        float distanceToEdge = expRadius - distanceToObject(expPosition, body.gameObject);
        if (distanceToEdge < 0)
        {
            distanceToEdge = 0;
        }
        float calc = (distanceToEdge / expRadius);
        if (calc <= 0)
        {
            calc = 0;
        }
        float force = body.mass * distanceToEdge * calc * expForce / Time.fixedDeltaTime;
        force = Mathf.Min(force, maxForce);
        body.AddForce(dir * force);
    }
    public static float distanceToObject(Vector2 position, GameObject obj)
    {
        Vector2 center = getCollectiveColliderCenter(obj);
        Vector2 dir = (center - position).normalized;
        RaycastHit2D[] rch2ds = Physics2D.RaycastAll(position, dir);
        foreach (RaycastHit2D rch2d in rch2ds)
        {
            if (rch2d.collider.gameObject == obj)
            {
                return rch2d.distance;
            }
        }
        throw new UnityException("Object " + obj + "'s raycast not found! This should not be possible!");
    }
    /// <summary>
    /// Sums the centers of all non-trigger colliders
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector2 getCollectiveColliderCenter(GameObject obj)
    {
        int count = 0;
        Vector2 sum = Vector2.zero;
        foreach (Collider2D c2d in obj.GetComponents<Collider2D>())
        {
            if (!c2d.isTrigger)
            {
                sum += (Vector2)c2d.bounds.center;
                count++;
            }
        }
        return sum / count;
    }
    /// <summary>
    /// Determines whether the center of the first object has a direct line of sight to the center of the second object
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool lineOfSight(GameObject first, GameObject second)
    {
        Vector2 pos1 = first.transform.position;
        Vector2 pos2 = second.transform.position;
        RaycastHit2D[] rch2ds = Physics2D.RaycastAll(pos1, pos2 - pos1, Vector2.Distance(pos1, pos2));
        foreach (RaycastHit2D rch2d in rch2ds)
        {
            if (rch2d.collider.gameObject != first && rch2d.collider.gameObject != second)
            {
                return false;
            }
        }
        return true;
    }
    public static bool withinRange(GameObject obja, GameObject objb, float distance)
    {
        return withinRange(obja.transform.position, objb.transform.position, distance);
    }
    public static bool withinRange(Vector2 posa, Vector2 posb, float distance)
    {
        return (posb - posa).sqrMagnitude <= distance * distance;
    }
    /// <summary>
    /// Loops the value around until it falls in the range of [min, max]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float loopValue(float value, float min, float max)
    {
        float diff = max - min;
        while (value < min)
        {
            value += diff;
        }
        while (value > max)
        {
            value -= diff;
        }
        return value;
    }
    /// <summary>
    /// Converts the number from the range (curLow, curHigh) to the range (newLow, newHigh), inclusive.
    /// 2017-04-24: copied from Meowzart.Utility.convertToRange()
    /// </summary>
    /// <param name="number">A number between (curLow, curHigh), inclusive</param>
    /// <param name="curLow">The low end of the current range</param>
    /// <param name="curHigh">The high end of the current range</param>
    /// <param name="newLow">The low end of the new range</param>
    /// <param name="newHigh">The high end of the new range</param>
    /// <returns>A number between (newLow, newHigh), inclusive</returns>
    public static float convertToRange(float number, float curLow, float curHigh, float newLow, float newHigh)
    {
        //Input checking
        if (number > curHigh || number < curLow)
        {
            throw new System.ArgumentException("number is " + number + " but it should be between (" + curLow + ", " + curHigh + ")");
        }
        if (curLow > curHigh)
        {
            throw new System.ArgumentException("curLow (" + curLow + ") is higher than curHigh (" + curHigh + ")!");
        }
        if (newLow > newHigh)
        {
            throw new System.ArgumentException("newLow (" + newLow + ") is higher than newHigh (" + newHigh + ")!");
        }
        //Conversion
        return (((number - curLow) * (newHigh - newLow) / (curHigh - curLow)) + newLow);
    }
}
