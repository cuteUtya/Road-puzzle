using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject[] WinCars;

    [SerializeField] private Text[] WinTexts;
    [SerializeField] private Color WinTextsColor;

    [SerializeField] private Road _road;
    [SerializeField] private int _seed = 0;

    private float _suitSpeed = 0.2f;
    private int _maximumIteration = 5;
    public static bool Ready { get; private set; } = false;

    public bool IsComplete()
    {
        var roads = GameObject.FindObjectsOfType<RoadPlace>();//FindGameObjectsWithTag("Road");

        int trues = 0;
        foreach(var road in roads)
        {
            if (road.IsMirror)
            {
                if (road.TrueRotation == (int)road.transform.eulerAngles.y 
                    || road.TrueRotation == (int)(road.transform.eulerAngles.y - 180) 
                    || road.TrueRotation == (int)(road.transform.eulerAngles.y + 180))
                {
                    trues++;
                }
            }
            else
            {
                if(road.TrueRotation == (int)road.transform.eulerAngles.y)
                {
                    trues++;
                }
            }
        }

        return trues == roads.Length;
    }

    private IEnumerator Start()
    {
        yield return new WaitWhile(() => !Input.anyKey);
        StartCoroutine(Suit());
        yield return new WaitForSeconds(_suitSpeed * 3 * (_maximumIteration-1));
        Ready = true;
    }

    public IEnumerator Suit()
    {
        var objects = GameObject.FindGameObjectsWithTag("Road");
        System.Random random = new System.Random(_seed);

        for(int i = 0; i < objects.Length; i++)
        {
            StartCoroutine(_road.RotateRoad(objects[i].transform, _suitSpeed, random.Next(0, _maximumIteration)));
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator CompleteLevel()
    {
         foreach(var car in WinCars)
        {
            car.SetActive(true);
        }
        yield return new WaitForSeconds(1f);

        foreach (var text in WinTexts)
        {
            StartCoroutine(ChangeTextColor(text, WinTextsColor, 1f));
        }

        yield return new WaitWhile(() => !Input.anyKey);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator ChangeTextColor(Text text, Color to, float duraction)
    {
        float time = 0f;
        Color defColor = text.color;

        while(time < duraction)
        {
            text.color = Color.Lerp(defColor, to, time / duraction);
            yield return null;
            time += Time.deltaTime;
        }
    }
}
