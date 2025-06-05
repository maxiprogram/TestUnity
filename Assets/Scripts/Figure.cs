using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//TypeShape-нельзя задавать значение явно
public enum TypeShape
{
    Square,
    Circle,
    Triangle,
    Polygon
};

//TypeColor-нельзя задавать значение явно
public enum TypeColor
{
    Red,
    Blue,
    Green
};

//TypeAnimal-нельзя задавать значение явно
public enum TypeAnimal
{
    Cat,
    Dog,
    Monkey,
    Rabbit,
    Raccoon,
    Tiger
};



public class Figure : MonoBehaviour
{

    [SerializeField] private TypeShape typeShape;
    [SerializeField] private TypeColor typeColor;
    [SerializeField] private TypeAnimal typeAnimal;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRendererAnimal;

    public TypeShape GetTypeShape()
    {
        return typeShape;
    }

    public TypeColor GetTypeColor()
    {
        return typeColor;
    }

    public TypeAnimal GetTypeAnimal()
    {
        return typeAnimal;
    }

    public void SetTypeShape(TypeShape typeShape, bool isInitPhysic = true)
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
            default:
                filename += "Square";
                break;
        }

        Sprite sprite = Resources.Load<Sprite>(filename);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogError("Not load sprite'" + filename + "'");
        }

        if (isInitPhysic)
        {
            this.AddComponent<CircleCollider2D>();
            this.AddComponent<Rigidbody2D>();
            //this.AddComponent<PolygonCollider2D>();   
        }

    }

    public void SetTypeColor(TypeColor typeColor)
    {
        this.typeColor = typeColor;
        switch (typeColor)
        {
            case TypeColor.Red:
                spriteRenderer.color = Color.red;
                break;
            case TypeColor.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case TypeColor.Green:
                spriteRenderer.color = Color.green;
                break;
            default:
                spriteRenderer.color = Color.white;
                break;
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

        Sprite sprite = Resources.Load<Sprite>(filename);
        if (sprite != null)
        {
            spriteRendererAnimal.sprite = sprite;
        }
        else
        {
            Debug.LogError("Not load sprite '" + filename + "'");
        }

    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Transform child = transform.Find("FrontSprite");
        if (child != null)
        {
            spriteRendererAnimal = child.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Not Find Child 'FrontSprite'");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
