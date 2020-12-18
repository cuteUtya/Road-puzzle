using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerInput : MonoBehaviour
{
    public delegate void RoadRaycast(GameObject hitObject);
    public static event RoadRaycast OnRoadRaycast;
    private void Update()
    {
        Vector3 mousePosition = Camera.main.WorldToScreenPoint(Input.mousePosition);
        mousePosition.z = 1;

        if (Input.anyKeyDown && Map.Ready)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Road")
                {
#if UNITY_EDITOR
                    OnRoadRaycast.Invoke(hit.collider.gameObject);
#elif UNITY_ANDROID
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        OnRoadRaycast.Invoke(hit.collider.gameObject);
                    }
                    
#endif

                }
            }
        }
    }
}