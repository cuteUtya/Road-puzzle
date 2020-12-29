using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class RoadPlace : MonoBehaviour
{
    public float TrueRotation;
    public bool IsMirror;
    public Vector3 MoveOutPoint;

    private void Start()
    {
        TrueRotation = transform.eulerAngles.y;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(MoveOutPoint + transform.position, 0.1f);
    }
}
