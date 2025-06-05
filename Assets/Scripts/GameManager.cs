using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

enum StatusGame
{
    Init,
    Regenerate,
    Play,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{

    public GameObject refPrefabFigure;
    public GameObject refPrefabUiFigure;
    public GameObject refDisplayLose;
    public GameObject refDisplayWin;
    public GameObject refDisplayWait;

    [SerializeField] private int maxCountFigure = 100;
    [SerializeField] private float instanceIntervalSec = 1;
    private List<GameObject> listFigures;

    private GameObject[] actionArray = { null, null, null, null, null, null, null };
    private int currentAction = 0;
    private StatusGame statusGame = StatusGame.Play;

    private Dictionary<string, int> hashtableControlMod3;
    private int coutDelete = 0;

    // Start is called before the first frame update
    void Start()
    {
        listFigures = new List<GameObject>(maxCountFigure);
        hashtableControlMod3 = new Dictionary<string, int>();

        statusGame = StatusGame.Init;

        StartCoroutine(CreateFigures(maxCountFigure));
        //CreateFigures();
        //Debug.Log("Start() end");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAction >= 7)
        {
            statusGame = StatusGame.Lose;
            refDisplayLose.SetActive(true);
            return;
        }

        if (statusGame != StatusGame.Play)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                //Debug.Log("Click Mouse: " + Input.mousePosition);

                UnityEngine.Vector3 inputVector;
                if (Input.touchCount > 0)
                {
                    inputVector = new UnityEngine.Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                }
                else
                {
                    inputVector = Input.mousePosition;
                }

                //Debug.Log("Pos: "+Input.mousePosition);
                UnityEngine.Vector2 inputPosition = Camera.main.ScreenToWorldPoint(inputVector);
                RaycastHit2D hit = Physics2D.Raycast(inputPosition, UnityEngine.Vector2.zero);
                if (hit.collider != null)
                {
                    //Debug.Log("2D Click on " + hit.collider.gameObject.name);
                    Predicate<GameObject> myPredicate = obj => obj == hit.collider.gameObject;
                    GameObject foundObject = listFigures.Find(myPredicate);
                    if (foundObject != null)
                    {
                        //Debug.Log("Yes find GameObject");

                        Figure foundFigure = foundObject.GetComponent<Figure>();
                        if (foundFigure.GetTypeShape() == TypeShape.SpecialIce)
                        {
                            if (coutDelete < 9)
                            {
                                return;
                            }
                        }

                        //GameObject imageObject = new GameObject("NewImage");
                        GameObject imageObject = Instantiate(refPrefabUiFigure);
                        UiFigure uiFigure = imageObject.GetComponent<UiFigure>();

                        imageObject.transform.SetParent(FindObjectOfType<Canvas>().transform);

                        uiFigure.SetTypeShape(foundFigure.GetTypeShape());
                        uiFigure.SetTypeColor(foundFigure.GetTypeColor());
                        uiFigure.SetTypeAnimal(foundFigure.GetTypeAnimal());

                        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = inputVector;

                        GameObject templateAction = GameObject.Find("TemplateAction" + (currentAction + 1));
                        RectTransform templateActionRectTransform = templateAction.GetComponent<RectTransform>();
                        uiFigure.SetTargetPosition(new UnityEngine.Vector2(templateActionRectTransform.position.x, templateActionRectTransform.position.y));
                        uiFigure.SetStatusMove(true);
                        actionArray[currentAction] = imageObject;

                        string key = (int)foundFigure.GetTypeShape() + "_" + (int)foundFigure.GetTypeColor() + "_" + (int)foundFigure.GetTypeAnimal();
                        if (hashtableControlMod3.ContainsKey(key))
                        {
                            int count = hashtableControlMod3[key];
                            if (count == 1)
                            {
                                hashtableControlMod3.Remove(key);
                            }
                            else
                            {
                                count--;
                                hashtableControlMod3[key] = count;
                            }
                        }
                        listFigures.Remove(foundObject);
                        Destroy(hit.collider.gameObject);
                        coutDelete++;

                        currentAction++;
                        //Debug.Log("Start CheckingMatch()");
                        CheckingMatch();
                        //Debug.Log("End CheckingMatch()");
                    }
                    else
                    {
                        Debug.LogError("Not find GameObject");
                    }
                }
                else
                {
                    //Debug.Log("No raycast");
                }
            }
    }

    private void ControlMod3()
    {
        foreach (var key in hashtableControlMod3.Keys.ToList())
        {
            //Debug.Log("key: " + key + " value: " + hashtableControlMod3[key]);
            if ((hashtableControlMod3[key] % 3) != 0)
            {
                string[] types = key.Split("_");
                //Debug.Log("types: " + types[0] + " " + types[1] + " " + types[2]);
                TypeShape typeShape = (TypeShape)Int32.Parse(types[0]);
                TypeColor typeColor = (TypeColor)Int32.Parse(types[1]);
                TypeAnimal typeAnimal = (TypeAnimal)Int32.Parse(types[2]);

                int countIter = 0;
                while (hashtableControlMod3[key] % 3 != 0)
                {
                    //TODO дублирование кода создания
                    UnityEngine.Vector3 pos = new UnityEngine.Vector3(transform.position.x, transform.position.y, transform.position.z);
                    pos.x = UnityEngine.Random.Range(-1.88f, 1.88f); //TODO лучше высчитывать динамически
                    GameObject objectFigure = Instantiate(refPrefabFigure, pos, UnityEngine.Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)));
                    Figure figure = objectFigure.GetComponent<Figure>();
                    figure.SetTypeShape(typeShape);
                    figure.SetTypeColor(typeColor);
                    figure.SetTypeAnimal(typeAnimal);

                    listFigures.Add(objectFigure);

                    hashtableControlMod3[key] = hashtableControlMod3[key] + 1;
                    countIter++;
                }
                //Debug.Log("countIter: " + countIter);
            }
        }

        refDisplayWait.SetActive(false);
        statusGame = StatusGame.Play;
    }

    private IEnumerator CreateFigures(int countFigure)
    //private void CreateFigures()
    {

        for (int i = 0; i < countFigure; i++)
        {
            //Debug.Log("iter " + i);
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(transform.position.x, transform.position.y, transform.position.z);
            pos.x = UnityEngine.Random.Range(-1.88f, 1.88f); //TODO лучше высчитывать динамически
            //Debug.Log("Random.Range: "+pos.x);
            GameObject objectFigure = Instantiate(refPrefabFigure, pos, UnityEngine.Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)));
            Figure figure = objectFigure.GetComponent<Figure>();
            RandomProperty(figure.GetComponent<Figure>());

            listFigures.Add(objectFigure);
            yield return new WaitForSeconds(instanceIntervalSec);
        }


        ControlMod3();
    }

    private void RandomProperty(Figure figure)
    {
        //TODO max-не лучшее решение
        int maxShape = Enum.GetValues(typeof(TypeShape)).Cast<int>().Max();
        int maxColor = Enum.GetValues(typeof(TypeColor)).Cast<int>().Max();
        int maxAnimal = Enum.GetValues(typeof(TypeAnimal)).Cast<int>().Max();


        TypeShape rndTypeShape = (TypeShape)UnityEngine.Random.Range(0, maxShape + 1);
        TypeColor rndTypeColor = (TypeColor)UnityEngine.Random.Range(0, maxColor + 1);
        TypeAnimal rndTypeAnimal = (TypeAnimal)UnityEngine.Random.Range(0, maxAnimal + 1);

        /*if (listFigures.Count >= 6)
        {
            rndTypeShape = TypeShape.SpecialWeight;
            rndTypeColor = TypeColor.Green;
            rndTypeAnimal = TypeAnimal.Cat;
            figure.SetTypeShape(TypeShape.SpecialWeight);
            figure.SetTypeColor(TypeColor.Green);
            figure.SetTypeAnimal(TypeAnimal.Cat);
        }
        else
        {
            rndTypeShape = TypeShape.Square;
            rndTypeColor = TypeColor.Blue;
            rndTypeAnimal = TypeAnimal.Dog;
            figure.SetTypeShape(TypeShape.Square);
            figure.SetTypeColor(TypeColor.Blue);
            figure.SetTypeAnimal(TypeAnimal.Dog);
        }*/

        figure.SetTypeShape(rndTypeShape);
        figure.SetTypeColor(rndTypeColor);
        figure.SetTypeAnimal(rndTypeAnimal);

        string key = (int)rndTypeShape + "_" + (int)rndTypeColor + "_" + (int)rndTypeAnimal;
        if (rndTypeShape == TypeShape.SpecialWeight)
        {
            key = (int)rndTypeShape + "_0_0";
        } else
        if (rndTypeShape == TypeShape.SpecialIce)
        {
            key = (int)rndTypeShape + "_0_0";
        } else
        if (rndTypeShape == TypeShape.SpecialFriction)
        {
            key = (int)rndTypeShape + "_0_0";
        } else
        if (rndTypeShape == TypeShape.SpecialBomb)
        {
            key = (int)rndTypeShape + "_0_0";
        }

        if (hashtableControlMod3.ContainsKey(key))
        {
            //Debug.Log("Increment hashtableControlMod3");
            hashtableControlMod3[key] = hashtableControlMod3[key] + 1;
        }
        else
        {
            //Debug.Log("Add hashtableControlMod3");
            hashtableControlMod3.Add(key, 1);
        }
    }

    public void CheckingMatch() //IEnumerator
    {
        //Debug.Log("CheckingMatch(): " + currentAction);
        bool isMatch = false;
        for (int i = 0; i < 7; i++)
        {
            if (actionArray[i] == null)
            {
                break;
            }
            if (i == 0 || i == 1)
            {
                continue;
            }

            UiFigure uiFigureCurrent = actionArray[i].GetComponent<UiFigure>();
            UiFigure uiFigurePrevious = actionArray[i - 1].GetComponent<UiFigure>();
            UiFigure uiFigurePrevious2 = actionArray[i - 2].GetComponent<UiFigure>();
            string keyCurrent = uiFigureCurrent.GetTypeShape() + "_" + uiFigureCurrent.GetTypeColor() + "_" + uiFigureCurrent.GetTypeAnimal();
            string keyPrevious = uiFigurePrevious.GetTypeShape() + "_" + uiFigurePrevious.GetTypeColor() + "_" + uiFigurePrevious.GetTypeAnimal();
            string keyPrevious2 = uiFigurePrevious2.GetTypeShape() + "_" + uiFigurePrevious2.GetTypeColor() + "_" + uiFigurePrevious2.GetTypeAnimal();
            //Debug.Log("cmp keys: "+keyCurrent+" "+keyPrevious+" "+keyPrevious2);
            if (keyCurrent == keyPrevious && keyCurrent == keyPrevious2)
            {
                isMatch = true;
                currentAction -= 3;
                StartCoroutine(AwaitUiFigureMove(uiFigureCurrent, i));
            }
        }
        //Debug.Log("countMatch: " + countMatch);
        if (listFigures.Count == 0 && isMatch == false)
        {
            statusGame = StatusGame.Lose;
            refDisplayLose.SetActive(true);
        }
    }

    private IEnumerator AwaitUiFigureMove(UiFigure uiFigure, int index)
    {
        bool isSpecialBomb = false;
        if (uiFigure.GetTypeShape() == TypeShape.SpecialBomb)
        {
            isSpecialBomb = true;
        }

        while (uiFigure.GetStatusMove())
            {
                yield return new WaitForSeconds(0.01f);
            }
        Destroy(actionArray[index]);
        actionArray[index] = null;
        Destroy(actionArray[index - 1]);
        actionArray[index - 1] = null;
        Destroy(actionArray[index - 2]);
        actionArray[index - 2] = null;

        if (index - 3 >= 0 && isSpecialBomb)
        {
            Destroy(actionArray[index - 3]);
            actionArray[index - 3] = null;
            currentAction--;
        }

        if (listFigures.Count == 0 && actionArray[0] == null)
            {
                statusGame = StatusGame.Win;
                refDisplayWin.SetActive(true);
            }
            else if (listFigures.Count == 0 && actionArray[0] != null)
            {
                statusGame = StatusGame.Lose;
                refDisplayLose.SetActive(true);
            }
    }

    //TODO Потеря из-за /3 или многократное увеличение(переделать Random)
    public void RegenerateFigures()
    {
        if (statusGame == StatusGame.Init || statusGame == StatusGame.Regenerate)
        {
            return;
        }
        refDisplayWait.SetActive(true);

        int countFigures = listFigures.Count();
        //Debug.Log("RegenerateFigures() count_new:" + countFigures);
        //countFigures = countFigures / 3;
        countFigures = (int)Math.Ceiling(countFigures / 3.0f);
        if (countFigures == 0)
        {
            countFigures = 1;
        }

        foreach (GameObject item in listFigures)
        {
            Destroy(item);
        }
        listFigures.Clear();

        hashtableControlMod3.Clear();

        if (statusGame == StatusGame.Lose)
        {
            refDisplayLose.SetActive(false);
        }
        else if (statusGame == StatusGame.Win)
        {
            refDisplayWin.SetActive(false);
        }

        coutDelete = 0;

        statusGame = StatusGame.Regenerate;
        StartCoroutine(CreateFigures(countFigures));

    }

    public void ClearAllFigures()
    {
        if (statusGame == StatusGame.Init || statusGame == StatusGame.Regenerate)
        {
            return;
        }
        refDisplayWait.SetActive(true);

        foreach (GameObject item in listFigures)
        {
            Destroy(item);
        }
        listFigures.Clear();

        for (int i = 0; i < 7; i++)
        {
            if (actionArray[i] != null)
            {
                Destroy(actionArray[i]);
            }
            actionArray[i] = null;
        }

        hashtableControlMod3.Clear();

        if (statusGame == StatusGame.Lose)
        {
            refDisplayLose.SetActive(false);
        }
        else if (statusGame == StatusGame.Win)
        {
            refDisplayWin.SetActive(false);
        }

        currentAction = 0;

        coutDelete = 0;

        statusGame = StatusGame.Init;
        StartCoroutine(CreateFigures(maxCountFigure));
    }

}
