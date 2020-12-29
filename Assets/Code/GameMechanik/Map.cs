using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    public Car WinCar;

    [SerializeField] private Text[] WinTexts;
    [SerializeField] private Color WinTextsColor;

    [SerializeField] private Road _road;
    [SerializeField] private int _seed = 0;

    private float _suitSpeed = 0.2f;
    private int _maximumIteration = 4;
    public static bool Ready { get; private set; } = false;
    public static bool CanReadInput = false;

    public bool IsComplete(RoadPlace road)
    {
        if (road.IsMirror)
        {
            if ((int)road.TrueRotation == (int)road.transform.eulerAngles.y
                || (int)road.TrueRotation == (int)(road.transform.eulerAngles.y - 180)
                || (int)road.TrueRotation == (int)(road.transform.eulerAngles.y + 180))
            {
                return true;
            }
        }
        else

        {
            if ((int)road.TrueRotation == (int)road.transform.eulerAngles.y)
            {
                return true;
            }
        }

        return false;
    }

    private void OnUserTouch(GameObject hitObject)
    {
        if (hitObject.tag == "Start" && CanReadInput)
        {
            WinCar.StartMove();
        }
    }

    private IEnumerator Start()
    {
        PlayerInput.OnRoadRaycast += OnUserTouch;
        yield return new WaitWhile(() => !Input.anyKey);
        StartCoroutine(Suit());
        yield return new WaitForSeconds(_suitSpeed * 3 * (_maximumIteration-1));
        Ready = true;
        CanReadInput = true;
    }

    public IEnumerator Suit()
    {
        
        var objects = GameObject.FindGameObjectsWithTag("Road");
        System.Random random = new System.Random(_seed);

        for(int i = 0; i < objects.Length; i++)
        {
            var iteration = random.Next(0, _maximumIteration);
            var direct = random.Next(-1, 1);

            if (iteration > 0)
            {
                StartCoroutine(_road.RotateRoad(objects[i].transform, _suitSpeed, iteration, direct));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
   /* public IEnumerator CompleteLevel()
    {
        CanReadInput = false;
         foreach(var car in WinCars)
        {
            car.GetComponent<Car>().enabled = true;
        }
        yield return new WaitForSeconds(1f);

        foreach (var text in WinTexts)
        {
            StartCoroutine(ChangeTextColor(text, WinTextsColor, 1f));
        }

        yield return new WaitWhile(() => !Input.anyKey);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }*/

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
