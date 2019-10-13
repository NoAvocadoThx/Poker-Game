using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType{Spade=4, Heart=3, Diamond=2, Club=1};



[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour
{
    //public
    [Header("Card Settings")]
    public float DrawProbability = 1.0f;
    public int fps = 60;
    public int rotDegPerSecond = 800;
    public bool isFaceUp = false;

 

    [HideInInspector]
    public bool canFlip = false;
   

    //SerializeField
    [SerializeField][Range(1,13)] private int _cardValue=0;
    [SerializeField] private CardType _cardType=CardType.Club;

    //private 
    private const float FLIP_LIMIT = 180f;
    private bool isProcessing = false;
    private bool isDone = false;

    //getters for card value and type
    public int CardValue { get { return _cardValue; } }
    public CardType cardType { get { return _cardType; } }

   
    /// <summary>
    /// Configure probability
    /// </summary>
    private void Awake()
    {
        // Ace of spade has 3x more probability of being drawn
        if (_cardType == CardType.Spade && _cardValue == 1) DrawProbability = 3;
        // Hearts have 2x probability
        else if (_cardType == CardType.Heart) DrawProbability = 2;
        else DrawProbability = 1;
    }


    


    /// <summary>
    /// when the user has pressed the mouse button while over the Collider
    /// </summary>
    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown Hit");
        //if it is flipping, we do not rotate
        if (isProcessing) return;
        //if we have drawn the card, we can flip the card
        if (canFlip) StartCoroutine(flipping());
    }




    /// <summary>
    /// lerp the card from deck to desired position
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="lerpStartTime"></param>
    /// <param name="lerpTime"></param>
    /// <returns></returns>
    public Vector3 MyLerp(Vector3 start, Vector3 end, float lerpStartTime, float lerpTime )
    {
        float timeSinceStarted = Time.time - lerpStartTime;
        float percentage = timeSinceStarted / lerpTime;
        var result = Vector3.Lerp(start, end, Mathf.SmoothStep(0.0f, 1.0f, percentage));
        return result;
    }


    /// <summary>
    /// non stop flipping coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator flipping()
    {
        //the card is flipping 
        isProcessing = true;
        //if corountine done
        
        while (!isDone)
        {
            float degree = rotDegPerSecond * Time.deltaTime;
            //determine the flipping degree
            if (isFaceUp)
                degree = -degree;
            //rotate the parent container 
            transform.parent.transform.Rotate(new Vector3(0, degree, 0));
            //if we flipped 180 degrees
            if (FLIP_LIMIT < transform.parent.transform.eulerAngles.y)
            {
                isDone = true;
            }
            //return
            yield return null;
        }
        //faceUp
        isFaceUp = true;
        //the card is not flipping
        isProcessing = false;
    }
}
