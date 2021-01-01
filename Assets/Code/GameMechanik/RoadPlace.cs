using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class RoadPlace : MonoBehaviour
{
    public float TrueRotation;
    public bool IsMirror;

    private void Start()
    {
        TrueRotation = transform.eulerAngles.y;
    }
}
