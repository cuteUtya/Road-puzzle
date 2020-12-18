using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    [SerializeField] private Vector2 _maximumPosition;
    [SerializeField] private Vector2 _minimumPosition;

    private void Update()
    {

        if (Input.touchCount == 1 && Map.CanReadInput)
        {
            var calculatedPosition = transform.localPosition - (Vector3)Input.GetTouch(0).deltaPosition * Time.deltaTime * _moveSpeed;
            transform.localPosition = calculatedPosition;
        }

        transform.localPosition = transform.localPosition.Clamp(_minimumPosition, _maximumPosition);
    }
}

public static class Vector2Extension
{
    public static Vector2 Clamp(this Vector2 vector, Vector2 minumum, Vector2 maximum )
    {
        return new Vector2(Mathf.Clamp(vector.x, minumum.x, maximum.x), Mathf.Clamp(vector.y, minumum.y, maximum.y));
    }
}
public static class Vector3Extension
{
    public static Vector2 Clamp(this Vector3 vector, Vector2 minumum, Vector2 maximum)
    {
        return new Vector2(Mathf.Clamp(vector.x, minumum.x, maximum.x), Mathf.Clamp(vector.y, minumum.y, maximum.y));
    }
}