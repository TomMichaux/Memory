using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public enum GameStatus
{
    waiting_on_first_card,
    waiting_on_second_card,
    match_found,
    no_match_found

}

public class Game : MonoBehaviour
{
    public GameStatus status;

    public GameObject[] selectedCards;

    private float timeoutTimer;

    public float timeoutTarget;

    [SerializeField] public int rows;

    [SerializeField] public int columns;

    [SerializeField] public string frontsidesFolder;

    [SerializeField] public string backsidesFolder;

    [SerializeField] public Sprite[] frontSprites;

    [SerializeField] public Sprite[] backSprites;

    [SerializeField] public Transform fieldAnchor;

    [SerializeField] public float offsetX;

    [SerializeField] public float offsetY;

    public Sprite[] selectedBackSprite;

    public List<Sprite> selectedFrontSprites;

    [SerializeField] public GameObject cardPrefab;

    private Stack<GameObject> stackOfCards;

    private GameObject[,] placedCards;



    public float totalPairs;
    private void MakeCards()
    {
        CalculateAmountOfPairs();

        LoadSprites();

        SelectFrontSprites();

        SelectBackSprite();

        ConstructCards();
    }

    private void DistributeCards()
    {
        placedCards = new GameObject[columns, rows];

        ShuffleCards();

        PlaceCardsOnField();
    }

    void Start()
    {

        MakeCards();

        DistributeCards();

        selectedCards = new GameObject[2];

        status = GameStatus.waiting_on_first_card;

    }


    void Update()
    {
        int aantal = 10;

        if (aantal % 2 == 0)
        {
            //Debug.Log("Het getal is even");
        }

        if (aantal % 3 != 0)
        {
            //Debug.Log("Het getal is niet deelbaar door 3. Er blijft " + aantal % 3 + "over.");
        }

        if (status == GameStatus.match_found || status == GameStatus.no_match_found)
        {
            RotateBackOrRemovePair();
        }
    }

    private void CalculateAmountOfPairs()
    {
        if(rows * columns % 2 == 0)
        {
            totalPairs = rows * columns / 2;
        }
    }

    private void LoadSprites()
    {
        frontSprites = Resources.LoadAll<Sprite>(frontsidesFolder);

        backSprites = Resources.LoadAll<Sprite>(backsidesFolder);
    }

    private void SelectFrontSprites()
    {
        if(frontSprites.Length < totalPairs)
        {
            Debug.LogError("Er zijn te weinig plaatjes om + " + totalPairs + " paren te maken");
        }

        new List<Sprite>(selectedFrontSprites);

        while(selectedFrontSprites.Count < totalPairs)
        {
            int randomGetal = Random.Range(0, frontSprites.Length);

            if (selectedFrontSprites.Contains(frontSprites[(int)randomGetal]) == false)
            {
                selectedFrontSprites.Add(frontSprites[(int)randomGetal]);
            }
        }
    }

    private void SelectBackSprite()
    {
        if (backSprites.Length > 0)
        {
            int BackRandomgetal = Random.Range(0, backSprites.Length);

            selectedBackSprite = backSprites;
        }
        else
        {
            Debug.LogError("Er zijn geen achterkant plaatjes om te selecteren.");
        }
    }

    private void ConstructCards()
    {
        stackOfCards = new Stack<GameObject>();

        GameObject parent = new GameObject();
        parent.name = "Cards";

        foreach( Sprite sprite in selectedFrontSprites)
        {
            for(int i = 0; i < 2; i++)
            {
                GameObject go = Instantiate(cardPrefab);
                Card cardscript = go.GetComponent<Card>();

                cardscript.SetBack(selectedBackSprite[0]);
                cardscript.SetFront(sprite);

                go.name = sprite.name;
                go.transform.parent = parent.transform;

                stackOfCards.Push(go);
            }
        }
     
    }

    private void ShuffleCards()
    {
        while(stackOfCards.Count > 0)
        {
            int randX = Random.Range(0, columns);

            int randY = Random.Range(0, rows);

            if (placedCards[randX, randY] == null)
            {
                Debug.Log("kaart" + stackOfCards.Peek().name + "is geplaatst op x:" + randX + "y:" + randY);
                placedCards[randX,randY] = stackOfCards.Pop();
            }

        }
    }

    private void PlaceCardsOnField()
    {
        for(int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject card = placedCards[x, y];

                Card cardscript = card.GetComponent<Card>();

                Vector2 cardsize = cardscript.GetBackSize();

                float xpos = fieldAnchor.transform.position.x + (x * (cardsize.x + offsetX));
                float ypos = fieldAnchor.transform.position.y - (y * (cardsize.y + offsetY));

                print(card.transform.lossyScale.x);

                placedCards[x, y].transform.position = new Vector3(xpos, ypos, 0f);
            }
        }
    }

    public void SelectCard(GameObject card)
    {
        if (status == GameStatus.waiting_on_first_card)
        {
            selectedCards[0] = card;

            status = GameStatus.waiting_on_second_card;
        }
        else if(status == GameStatus.waiting_on_second_card)
        {
            selectedCards[1] = card;

            CheckForMatchingPair();
        }
    }

    private void CheckForMatchingPair()
    {
        timeoutTimer = 0f;



        Debug.Log("what the eerste kaart ");
        Debug.Log(selectedCards[0]);
        Debug.Log(selectedCards[1]);

        if (selectedCards[0].name == selectedCards[1].name)
        {
            status = GameStatus.match_found;
        }
        else
        {
            status = GameStatus.no_match_found;
        }
    }

    private void RotateBackOrRemovePair()
    {
        timeoutTimer += Time.deltaTime;

        if(timeoutTimer >= timeoutTarget)
        {
            if(status == GameStatus.match_found)
            {
                selectedCards[0].SetActive(false);
                selectedCards[1].SetActive(false);
            }
            if(status == GameStatus.no_match_found)
            {
                selectedCards[0].GetComponent<Card>().TurnToBack();
                selectedCards[1].GetComponent<Card>().TurnToBack();
            }
            selectedCards[0] = null;
            selectedCards[1] = null;

            status = GameStatus.waiting_on_first_card;
        }
    }

    public bool AllowedToSelectCard(Card card)
    {
        if (selectedCards[0] == null)
        {
            return true;
        }
        if (selectedCards[1] == null)
        {
            if(selectedCards[0] != card.gameObject)
             {
                return true;
             }
        }
        return false;
    }
}
