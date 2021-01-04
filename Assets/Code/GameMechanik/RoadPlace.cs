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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), ((int)(transform.position.y / 0.5f) * 0.5f), Mathf.RoundToInt(transform.position.z));
        }
    }
}
