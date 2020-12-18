using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;

public class Car : MonoBehaviour
{
    [SerializeField] private SplineFollower _spline;
    [SerializeField] private float _targetSpeed;
    [SerializeField] private float _speedRate;

    private Vector3 _lastRotation;
    public void Update()
    {
        var rotationDelta = transform.eulerAngles - _lastRotation;

        _spline.followSpeed = Mathf.Lerp(_spline.followSpeed, _targetSpeed / Mathf.Clamp(Mathf.Clamp(Mathf.Abs(rotationDelta.y), 1, Mathf.Infinity), 1, _targetSpeed), Time.deltaTime * _speedRate);//Mathf.Clamp(((20 / rotationDelta.y) * _targetSpeed), 1, Mathf.Infinity);

        Debug.Log(_spline.followSpeed + "   "  + rotationDelta.y);
        _lastRotation = transform.eulerAngles;
    }
}
