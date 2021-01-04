using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;

public class Car : MonoBehaviour
{
    [SerializeField] private Map _map;

    [SerializeField] private AnimationCurve _brakeGraphic;
    [SerializeField] private Transform _body;
    [SerializeField] private SplineFollower _spline;

    [SerializeField] private GameObject _spawnPoint;

    // время, за которое машина доедет от точки спавна до дефолтной позиции
    [SerializeField] private float _moveTime;

    private bool _playerLoose = false;
    private float _distanceToNextPlatform;
    private bool _carCrahed = false;

    protected static GameObject _copy = null;
    protected static Vector3 _defaultPosition;

    private void Start()
    {
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

    public void FixedUpdate()
    {
        if (_spline && !_carCrahed)
        {
            
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
        List<Material> carMats = new List<Material>();

        foreach(var render in transform.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (var material in render.materials)
            {
                ChangeRenderMode(material, BlendMode.Transparent);
                carMats.Add(material);
            }
        }

        float time = 0;
        float defSpeed = _spline.followSpeed;

        float t = 1f;
        while (t > 0)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;

            Debug.Log(_brakeGraphic.Evaluate(time) + "  " + _spline.followSpeed);

            t = _brakeGraphic.Evaluate(time);
            _spline.followSpeed = Mathf.Lerp(0, defSpeed, t);

            foreach(var mat in carMats)
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, t);
            }
        }

        _spline.followSpeed = 0;

        var copy = Instantiate(_copy);
        _map.WinCar = copy.GetComponent<Car>();
        copy.transform.DOMove(_defaultPosition, _moveTime);
        copy.SetActive(true);

        Destroy(this.gameObject);
        /*
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
        */
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

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }

    }
}