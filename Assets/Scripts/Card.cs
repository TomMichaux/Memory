using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardStatus
{
    show_back = 0,
    show_front,
    rotating_to_back,
    rotating_to_front
}

public class Card : MonoBehaviour
{
    private void OnMouseUp()
    {
        Debug.Log("Geklikt op de kaart.");

        if (game.AllowedToSelectCard(this) == true)
        {
            if (status == CardStatus.show_back)
            {
                game.SelectCard(gameObject);
                TurnToFront();
            }
            else
            {
                TurnToBack();
            }
        }
    }

    private Game game;

    [SerializeField] float TurnTargetTime;

    private float turnTimer;

    private Quaternion startRotation;

    private Quaternion targetRotation;

    [SerializeField] private CardStatus status;

    public float percentage;

    public SpriteRenderer frontRenderer;

    public SpriteRenderer backRenderer;


    private void Awake()
    {
        status = CardStatus.show_back;

        GetFrontAndBackSpriteRenderers();

        game = FindObjectOfType<Game>();
    }

    void Start()
    {
        
    }


    void Update()
    {
        turnTimer += Time.deltaTime;

        percentage = turnTimer / TurnTargetTime;



        transform.rotation = Quaternion.Slerp(startRotation, targetRotation, percentage);

        if( percentage >= 1f)
        {
            if (status == CardStatus.rotating_to_back)
            {
                status = CardStatus.show_back;
            }
            else if (status == CardStatus.rotating_to_front)
            {
                
                status = CardStatus.show_front;

            }
        }
    }

    public void TurnToFront()
    {
        status = CardStatus.rotating_to_front;

        turnTimer = 0f;

        startRotation = transform.rotation;

        targetRotation = Quaternion.Euler(0, 180, 0);
    }

    public void TurnToBack()
    {
       status = CardStatus.rotating_to_back;

        turnTimer = 0f;

        startRotation = transform.rotation;

        targetRotation = Quaternion.Euler(0, 0, 0);
    }

    private void GetFrontAndBackSpriteRenderers()
    {
        foreach(Transform t in transform)
        {
            if (t.name == "Front") { 
                frontRenderer = t.GetComponent<SpriteRenderer>(); 
            }
            else if(t.name == "Back")
            {
                backRenderer = t.GetComponent<SpriteRenderer>();
            }
        }   
    } 

    public void SetFront(Sprite sprite)
    {
        if (frontRenderer != null)
        {
            frontRenderer.sprite = sprite;
        }
    }

    public void SetBack(Sprite sprite)
    {
        if(backRenderer != null)
        {
            backRenderer.sprite = sprite;
        }
    }


    public Vector2 GetFrontSize()
    {
        if (frontRenderer == null)
        {
            Debug.LogError("Er is geen frontrenderer gevonden");
        }
        return frontRenderer.bounds.size;
    }

    public Vector2 GetBackSize()
    {
        if (frontRenderer == null)
        {
            Debug.LogError("Er is geen backrenderer gevonden");
        }

        return backRenderer.bounds.size;
    }
}
