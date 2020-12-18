using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class Road : MonoBehaviour
{
    [SerializeField] private Map _map;

    private List<Transform> _moved = new List<Transform>();
    private bool _canChange = true;

    private void Start()
    {
        PlayerInput.OnRoadRaycast += OnRoadRaycast;
    }
    private void OnRoadRaycast(GameObject road)
    {
        if (_canChange)
        {
            StartCoroutine(RotateRoad(road.transform, 1, 0.15f));
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

        if (_map.IsComplete())
        {
            _canChange = false;
            StartCoroutine(_map.CompleteLevel());
        }
    }

    public IEnumerator RotateRoad(Transform road, float stepTime, int iteration, int direct)
    {
        if (!_moved.Contains(road))
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

        if (_map.IsComplete())
        {
            _canChange = false;
            StartCoroutine(_map.CompleteLevel());
        }
        /*
        for (int i = 0; i < iteration; i++)
        {
            StartCoroutine(RotateRoad(road, Random.Range(-1, 2), stepTime));
            yield return new WaitForSeconds(stepTime * 3);
        }*/
    }
}
