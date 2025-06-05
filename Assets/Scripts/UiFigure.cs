using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


//TODO дублирование кода, вынести в интерфейс
public class UiFigure : MonoBehaviour
{

    [SerializeField] private TypeShape typeShape;
    [SerializeField] private TypeColor typeColor;
    [SerializeField] private TypeAnimal typeAnimal;

    private Image imageRenderer;
    private Image imageRendererAnimal;
    private RectTransform rectTransform;
    private bool statusMove = false;
    private Vector2 targetPosition;
    //[SerializeField] private float moveSpeed = 50000.0f;
    private float moveSpeed = 2500.0f;

    public void SetTargetPosition(Vector2 pos)
    {
        targetPosition = pos;
    }

    public void SetStatusMove(bool status)
    {
        statusMove = status;
    }

    public bool GetStatusMove()
    {
        return statusMove;
    }

    public TypeShape GetTypeShape()
    {
        return typeShape;
    }

    public TypeColor GetTypeColor()
    {
        if (typeShape == TypeShape.SpecialWeight || typeShape == TypeShape.SpecialIce)
        {
            return 0;
        }
        else
        {
            return typeColor;   
        }
    }

    public TypeAnimal GetTypeAnimal()
    {if (typeShape == TypeShape.SpecialWeight || typeShape == TypeShape.SpecialIce)
        {
            return 0;
        }
        else
        {
            return typeAnimal;
        }
    }

    public void SetTypeShape(TypeShape typeShape)
    {
        this.typeShape = typeShape;
        string filename = "TypeShapes/";

        switch (typeShape)
        {
            case TypeShape.Circle:
                filename += "Circle";
                break;
            case TypeShape.Triangle:
                filename += "Triangle";
                break;
            case TypeShape.Polygon:
                filename += "HexagonFlatTop";
                break;
            case TypeShape.Square:
                filename += "Square";
                break;
            case TypeShape.SpecialWeight:
                filename += "SpecialWeight";
                break;
            case TypeShape.SpecialIce:
                filename += "SpecialIce";
                break;
            default:
                filename += "Square";
                break;
        }

        Sprite sprite = Resources.Load<Sprite>(filename);
        if (sprite != null)
        {
            imageRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogError("Not load sprite'" + filename + "'");
        }

    }

    public void SetTypeColor(TypeColor typeColor)
    {
        this.typeColor = typeColor;
        switch (typeColor)
        {
            case TypeColor.Red:
                imageRenderer.color = Color.red;
                break;
            case TypeColor.Blue:
                imageRenderer.color = Color.blue;
                break;
            case TypeColor.Green:
                imageRenderer.color = Color.green;
                break;
            default:
                imageRenderer.color = Color.white;
                break;
        }

        if (typeShape == TypeShape.SpecialWeight)
        {
            imageRenderer.color = Color.black;
        }
        else
        if (typeShape == TypeShape.SpecialIce)
        {
            imageRenderer.color = Color.blue;
        }
    }

    public void SetTypeAnimal(TypeAnimal typeAnimal)
    {
        this.typeAnimal = typeAnimal;
        string filename = "TypeAnimals/";

        switch (typeAnimal)
        {
            case TypeAnimal.Cat:
                filename += "cat";
                break;
            case TypeAnimal.Dog:
                filename += "dog";
                break;
            case TypeAnimal.Monkey:
                filename += "monkey";
                break;
            case TypeAnimal.Rabbit:
                filename += "rabbit";
                break;
            case TypeAnimal.Raccoon:
                filename += "raccoon";
                break;
            case TypeAnimal.Tiger:
                filename += "tiger";
                break;
            default:
                filename += "tiger";
                break;
        }

        if (typeShape == TypeShape.SpecialWeight)
        {
            filename = "TypeAnimals/special_weight";
        }
        else
        if (typeShape == TypeShape.SpecialIce)
        {
            filename = "TypeAnimals/special_ice";
        }

        Sprite sprite = Resources.Load<Sprite>(filename);
        if (sprite != null)
        {
            imageRendererAnimal.sprite = sprite;
        }
        else
        {
            Debug.LogError("Not load sprite '" + filename + "'");
        }

    }

    void Awake()
    {
        imageRenderer = GetComponent<Image>();
        Transform child = transform.Find("FrontImage");
        if (child != null)
        {
            imageRendererAnimal = child.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Not Find Child 'FrontImage'");
        }

        targetPosition = new Vector2(0, 0);
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (statusMove)
        {
            //Debug.Log("MoveSpeed:" + moveSpeed + " or " + moveSpeed * Time.deltaTime);
            rectTransform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); //* Time.deltaTime
            if (rectTransform.position.x == targetPosition.x && rectTransform.position.y == targetPosition.y)
            {
                Debug.Log("Stop Animation Move UI");
                statusMove = false;
            }
        }
    }
}
