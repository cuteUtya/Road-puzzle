using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPlace : MonoBehaviour
{
    public float TrueRotation;
    public bool IsMirror;

    private void Start()
    {
        TrueRotation = transform.eulerAngles.y;
    }
}
