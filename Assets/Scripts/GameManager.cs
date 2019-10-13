using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Serailize fields
    [Header("Game Settings")]
    [SerializeField] private Deck _deck = null;
    public float lerpTime = 9f;
    public float lerpStartTime = 0;

    [Header("UI Settings")]
    [SerializeField] private RectTransform _cardPosition_1 = null;
    [SerializeField] private RectTransform _cardPosition_2 = null;
    [SerializeField] private Button _drawButton = null;
    [SerializeField] private Button _discardButton = null;
    [SerializeField] private Text _gamesWon = null;
    [SerializeField] private Text _gamesLost = null;
    [SerializeField] private Text _statement = null;
    [SerializeField] private Text _shuffleText = null;

    //public variables
    [HideInInspector]
    public bool canFlip = false;

    //private variables
    // Int
    private const int DECK_LENGTH = 52;
    private int _winScore;
    private int _lostScore;
    private int index1;
    private int index2;
    // List
    private List<int> _DrawnCards;

    // Bool
    //bool that determines the current state
    //true -- our next move is to draw cards
    //false -- our next move is to discard these 2 cards
    private bool _isDrawing;
    private bool _discardClicked;
    private bool _flippingDone;
    private bool _restarting;

    // Vector3
    private Vector3 _card1Pos;
    private Vector3 _card2Pos;

    // String
    private const string LOSE_TEXT = "2nd Card Loses!";
    private const string WIN_TEXT = "2nd Card Wins!";
    private const string INSTRUTION_TEXT = "Flip these cards using \nmouse left click";

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        //drawn Cards
        _DrawnCards = new List<int>();
        //check if deck is null or not
        if (_deck == null)
        {
            Debug.Log("Deck is null!");
            return;
        }
        //check if card1Position and card2Position were hooked up in the inpector
        if (_cardPosition_1 && _cardPosition_2)
        {
            //convert canvas space to world space
            _card1Pos = Camera.main.ScreenToWorldPoint(_cardPosition_1.position);
            _card2Pos = Camera.main.ScreenToWorldPoint(_cardPosition_2.position);
            //prevent the card from re-positioning to the back of the camera
            _card1Pos.z = 0;
            _card2Pos.z = 0;
        }
        //reset the scores
        _winScore = 0;
        _lostScore = 0;

        //reset the indices
        index1 = -1;
        index2 = -1;

        //set texts
        _statement.text = INSTRUTION_TEXT;
      
        //set UI
        Invalidate();

        //not restarting
        _restarting = false;

        //shuffle the deck
        _deck.shuffle();

        //lerp start time
        lerpStartTime = Time.time;
    }

    /// <summary>
    /// update UI buttons so they reflect current state
    /// </summary>
    void Invalidate()
    {
        //when is in drawing state
        //enable draw button and disable discard button
        if (_isDrawing)
        {
            _drawButton.gameObject.SetActive(false);
            _discardButton.gameObject.SetActive(true);
            //only show statement when flip done
            if(_flippingDone)
                _statement.gameObject.SetActive(true);
        }
        //when is in discarding state
        //enable discard button and disable draw button
        else
        {
            _drawButton.gameObject.SetActive(true);
            _discardButton.gameObject.SetActive(false);
            if(!_flippingDone)
                _statement.gameObject.SetActive(false);
        }
        //if we are not done flipping
        //we cannot discard current cards
        if (_flippingDone)
            _discardButton.interactable = false;
        //if done, we can discard
        else
            _discardButton.interactable = true;

        //set shuffle texts
        _shuffleText.text = null;
        _shuffleText.gameObject.SetActive(false);

        //if restarting
        if (_restarting)
        {
            //disable all buttons
            _drawButton.gameObject.SetActive(false);
            //although discard button wont show, disable it anyway
            _discardButton.gameObject.SetActive(false);
            //display shuffle text
            _shuffleText.gameObject.SetActive(true);
            _shuffleText.text = "Shuffling Deck..";
        }

    }


    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
       
        //if we have draw any cards
        if (index1 != -1 && index2 != -1)
        {
            //if these cards still exists
            if (_deck.deck[index1] && _deck.deck[index2])
            {
                //cache reference for readability
                Card firstCard = _deck.deck[index1].GetComponentInChildren<Card>();
                Card secondCard = _deck.deck[index2].GetComponentInChildren<Card>();
                //only after all the cards are flipped, we can do the compare
                if (_deck.deck[index1].GetComponentInChildren<Card>().isFaceUp &&
                   _deck.deck[index2].GetComponentInChildren<Card>().isFaceUp )
                {
                    if (_flippingDone) {
                        //do the compare
                        if (isSecondCardHigher(index1, index2))
                            //if second card's value is higher, set the win score
                            _gamesWon.text = _winScore.ToString();
                        else
                            //if second card's value is lower, set the lost score
                            _gamesLost.text = _lostScore.ToString();
                        _flippingDone = false;
                    }

                    //restart the game 
                    //once the deck is empty, we shuffle the deck and reload the scene
                    if (_DrawnCards.Count == DECK_LENGTH && !_isDrawing)
                    {
                        _restarting = true;

                        //call restart after 5 seconds
                        Invoke("restart", 2);
                       
                    }
                }

                //if clicked draw button
                if (_isDrawing)
                {
                    //lerp the card from its original position to desired position
                    _deck.deck[index1].transform.position = firstCard.MyLerp(_deck.deck[index1].transform.position, _card1Pos, lerpStartTime, lerpTime);
                    _deck.deck[index2].transform.position = secondCard.MyLerp(_deck.deck[index2].transform.position, _card2Pos, lerpStartTime, lerpTime);
                }
            }
        }

        

        //update the UI
        Invalidate();

        //quit the game by pressing esc
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }


    }

  

    /// <summary>
    /// when clicking on draw button
    /// </summary>
    public void OnDrawButton()
    {
        //update current state
        _isDrawing = true;
        _flippingDone = true;
        //draw the first card
        index1 = _deck.drawCardIndex();
        //choose a card that has not yet chosen before
        while (_DrawnCards.Contains(index1))
        {
            index1 = _deck.drawCardIndex();

        }
        //add this card to the chosen list
        _DrawnCards.Add(index1);
        /*
        Note: disable these 2 lines will cause OnMouseDown() not working properly sometimes,
              seems the ray called by OnMouseDown() is not hitting the colliders on the drown card sometimes due to unknown collider issues after moving.
              Deactive and Reactive the cards will fix the problem.
        */
        _deck.deck[index1].gameObject.SetActive(false);
        _deck.deck[index1].gameObject.SetActive(true);

        //draw the second card
        index2 = _deck.drawCardIndex();
        //choose a card that has not yet chosen before
        while (_DrawnCards.Contains(index2))
        {
            index2 = _deck.drawCardIndex();

        }
        //add this card to the chosen list
        _DrawnCards.Add(index2);
        /*
        Note: disable these 2 lines will cause OnMouseDown() not working properly sometimes,
              seems the ray called by OnMouseDown() is not hitting the colliders on the drown card sometimes due to unknown collider issues after moving.
              Deactive and Reactive the cards will fix the problem.
        */
        _deck.deck[index2].gameObject.SetActive(false);
        _deck.deck[index2].gameObject.SetActive(true);
        //we can now flip the cards
        _deck.deck[index1].GetComponentInChildren<Card>().canFlip = true;
        _deck.deck[index2].GetComponentInChildren<Card>().canFlip = true;

        //update the UI
        Invalidate();
        //reset the time started
        lerpStartTime = Time.time;
    }

    /// <summary>
    /// when clicking on the discard button
    /// </summary>
    public void OnDiscardButton()
    {
        //update current state
        _isDrawing = false;
        //clear statement
        _statement.text = INSTRUTION_TEXT;
        //if the animator on the cards exists
        if (_deck.deck[index1].GetComponent<Animator>() && _deck.deck[index2].GetComponent<Animator>())
        {
            //play the discard animation
            _deck.deck[index1].GetComponent<Animator>().SetTrigger("canDiscard");
            _deck.deck[index2].GetComponent<Animator>().SetTrigger("canDiscard");
        }
        //discard these 2 cards after 1.5 seconds for the animation to finish
        Destroy(_deck.deck[index1],1.5f);
        Destroy(_deck.deck[index2],1.5f);
        //update UI
        Invalidate();
       
    }

    /// <summary>
    /// compare these 2 cards drawn
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    /// <returns></returns>
    private bool isSecondCardHigher(int index1, int index2)
    {

        if (index1 != -1 && index2 != -1)
        {
            //cache reference for readability
            Card firstCard = _deck.deck[index1].GetComponentInChildren<Card>();
            Card secondCard = _deck.deck[index2].GetComponentInChildren<Card>();
            //if the first card's numeric value is higher, lost score +1
            if (firstCard.CardValue > secondCard.CardValue)
            {
                _lostScore++;
                //set lose text
                _statement.text = LOSE_TEXT;
            }

            //if the numeric value is the same
            else if (firstCard.CardValue == secondCard.CardValue)
            {
                //we compare the suit value
                //Spades > Hearts > Diamonds > Clubs
                if ((int)firstCard.cardType > (int)secondCard.cardType)
                {
                    _lostScore++;
                    //set lose text
                    _statement.text = LOSE_TEXT;
                }
                else
                {
                    _winScore++;
                    //set win text
                    _statement.text = WIN_TEXT;
                    return true;
                }
            }

            //if the second card's numeric value is higher
            else
            {
                _winScore++;
                //set win text
                _statement.text = WIN_TEXT;
                return true;
            }
        }
        return false;
    }




    /// <summary>
    /// function that restarts the game
    /// </summary>
    private void restart()
    {

        //reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }


}
