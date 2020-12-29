using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class Road : MonoBehaviour
{
    private List<Transform> _moved = new List<Transform>();
    private bool _canChange = true;

    private void Start()
    {
        PlayerInput.OnRoadRaycast += OnRoadRaycast;
    }
    private void OnRoadRaycast(GameObject hitObject)
    {
        if (_canChange)
        {
            if (hitObject.tag == "Road")
            {
                StartCoroutine(RotateRoad(hitObject.transform, 1, 0.15f));
            }
        }
    }

    public IEnumerator RotateRoad(Transform road, int direct, float stepTime)
    {
        if (!_moved.Contains(road))
        {
            _moved.Add(road);
            road.DOMove(road.position + new Vector3(0, 0.5f, 0), stepTime);
            yield return new WaitForSeconds(stepTime);
            road.DORotate(road.eulerAngles + new Vector3(0, direct == 1 ? 90 : -90, 0), stepTime);
            yield return new WaitForSeconds(stepTime);
            road.DOMove(road.position - new Vector3(0, 0.5f, 0), stepTime);
            yield return new WaitForSeconds(stepTime);
            _moved.Remove(road);
        }
    }

    public IEnumerator RotateRoad(Transform road, float stepTime, int iteration, int direct)
    {
        if (!_moved.Contains(road) && iteration > 0)
        {
            _moved.Add(road);
            road.DOMove(road.position + new Vector3(0, 0.5f, 0), stepTime);
            yield return new WaitForSeconds(stepTime);
            road.DORotate(road.eulerAngles + new Vector3(0, iteration * (direct == -1 ? -1 : 1) * 90, 0), stepTime);
            yield return new WaitForSeconds(stepTime * iteration);
            road.DOMove(road.position - new Vector3(0, 0.5f, 0), stepTime);
            yield return new WaitForSeconds(stepTime);
            _moved.Remove(road);
        }
    }
}
