using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;

public class Car : MonoBehaviour
{
    [SerializeField] private Map _map;

    [SerializeField] private Transform _centreOfMass;
    [SerializeField] private GameObject _explossionFX;
    [SerializeField] private SplineFollower _spline;
    [SerializeField] private float _targetSpeed;
    [SerializeField] private float _speedRate;

    [SerializeField] private GameObject _spawnPoint;

    // время, за которое машина доедет от точки спавна до дефолтной позиции
    [SerializeField] private float _moveTime;

    private Vector3 _lastRotation;

    private bool _playerLoose = false;
    private float _distanceToNextPlatform;
    private bool _carCrahed = false;

    protected static GameObject _copy = null;
    protected static Vector3 _defaultPosition;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = _centreOfMass.localPosition;
        if (_copy == null)
        {
            _copy = Instantiate(gameObject, _spawnPoint.transform.position, transform.rotation);
            _defaultPosition = transform.position;
        }
    }

    public void StartMove()
    {
        _spline.enabled = true;
    }

    public void Update()
    {
        if (_spline && !_carCrahed)
        {
            if (!_playerLoose)
            {
                var rotationDelta = transform.eulerAngles - _lastRotation;

                _spline.followSpeed = Mathf.Lerp(_spline.followSpeed, _targetSpeed / Mathf.Clamp(Mathf.Clamp(Mathf.Abs(rotationDelta.y), 1, Mathf.Infinity), 1, _targetSpeed), Time.deltaTime * _speedRate);

                _lastRotation = transform.eulerAngles;
            }

            if (!NextPlatormIsValid() && _distanceToNextPlatform < 0.5f)
            {
                RoadPlace current;

                if (TryGetCurrentPlatform(out current) && Map.CanReadInput )
                {
                    _carCrahed = true;
                    StartCoroutine(StopMoving());
                }
            }
        }
    }

    private IEnumerator StopMoving()
    {
        var copy = Instantiate(_copy);
        _map.WinCar = copy.GetComponent<Car>();
        copy.transform.DOMove(_defaultPosition, _moveTime);
        copy.SetActive(true);


        RoadPlace current;

        if (TryGetCurrentPlatform(out current) && Map.CanReadInput)
        {
            Destroy(_spline);
            //var angle = Mathf.Atan2(transform.position.x - current.transform.position.x, transform.position.z - current.transform.position.z) * Mathf.Rad2Deg;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = false;
            //GetComponent<Rigidbody>().AddTorque(current.RotationForce);
            GetComponent<Rigidbody>().velocity = ((current.MoveOutPoint + current.transform.position) - transform.position) * 3;
            transform.DORotate(Quaternion.LookRotation(current.MoveOutPoint, transform.up).eulerAngles, 0.5f);
        }

        yield return new WaitForSeconds(0.1f);

        GetComponent<Rigidbody>().useGravity = true;

            yield return null;
        

    }

    /*private IEnumerator MoveOut()
    {
        RoadPlace current;

        if (TryGetCurrentPlatform(out current) && Map.CanReadInput)
        {
            var spline = current.DeadWay;
            transform.position = spline.GetPoint(0).position;
            _spline.spline = spline;
        }
        yield return null;

        var copy = Instantiate(_copy);
        _map.WinCar = copy.GetComponent<Car>();
        copy.transform.DOMove(_defaultPosition, _moveTime);
        copy.SetActive(true);
    }*/

    private bool TryGetCurrentPlatform(out RoadPlace platform)
    {
        int layerMask = 1 << 8;

        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, -transform.up, out hit, Mathf.Infinity, layerMask))
        {
            RoadPlace road;
            if(hit.collider.gameObject.TryGetComponent<RoadPlace>(out road))
            {
                _distanceToNextPlatform = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);
                platform = road;
                return true;
            }
        }
        platform = null;
        return false;
    }

    private bool NextPlatormIsValid()
    {
        RaycastHit info;

        if (Physics.Raycast(transform.position + Vector3.up / 2 + (transform.rotation * Vector3.forward/1.5f), -Vector3.up, out info))
        {
            RoadPlace road;

            if (info.collider.gameObject.CompareTag("StaticRoad"))
            {
                return true;
            }

            if (info.collider.gameObject.TryGetComponent<RoadPlace>(out road))
            {
                if (_map.IsComplete(road))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground" && _carCrahed)
        {
            _explossionFX.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + Vector3.up/2 + (transform.rotation * Vector3.forward/1.5f), -Vector3.up);
        RaycastHit info;

        if (Physics.Raycast(transform.position + Vector3.up / 2 + (transform.rotation * (Vector3.forward/1.5f)), -Vector3.up, out info) )
        {
            RoadPlace road;

            if (info.collider.gameObject.TryGetComponent<RoadPlace>(out road))
            {
                Gizmos.color = _map.IsComplete(road) ? Color.green : Color.red;
                Gizmos.DrawSphere(info.collider.gameObject.transform.position, 0.1f);
            }
        }

        RoadPlace roaddown;
        if (TryGetCurrentPlatform(out roaddown))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(roaddown.transform.position, new Vector3(1, 0.1f, 1f));
        }
    }
}