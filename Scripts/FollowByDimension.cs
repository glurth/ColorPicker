using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowByDimension : MonoBehaviour
{
    public bool followX;
    public bool followY;
    public bool followZ;
    public Transform toFollow;

    void Update()
    {
        Vector3 pos = transform.localPosition;
        if (followX)
            pos.x = toFollow.localPosition.x;
        if (followY)
            pos.y = toFollow.localPosition.y;
        if (followZ)
            pos.z = toFollow.localPosition.z;
        transform.localPosition = pos;
    }
}
