using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Deck : MonoBehaviour
{


    //store the deck information
    [SerializeField] private GameObject[] _deckHolder=null;
    [SerializeField] private GameObject _deckPosition=null;

    //getters
    public GameObject[] deck { get { return _deck; } }
    public GameObject deckPosition { get { return _deckPosition; } }

    private int _weightTotal = 0;
    private GameObject[] _deck = new GameObject[52];

    /// <summary>
    /// start function called by Unity
    /// </summary>
    void Start()
    {
        
        //if deck is not null
        if (_deckHolder.Length > 0 && _deckPosition)
        {
            //convert canvas space to world space
            Vector3 deckPos = Camera.main.ScreenToWorldPoint(_deckPosition.transform.position);
            //keep the deck in the frame
            deckPos.z = 0;
            //move the deck to the center
            //52*0.08/2=2.08
            deckPos.x -= 2.08f;
            for (int i = 0; i < _deckHolder.Length; i++)
            {
                //create copies for the whole deck in then scene and cache reference
                _deck[i]=Instantiate(_deckHolder[i], new Vector3(deckPos.x += 0.08f, deckPos.y, deckPos.z-=0.01f), Quaternion.identity);
             
                //cache reference to the cards
                Card tempCard = _deckHolder[i].GetComponentInChildren<Card>();
                
                //calculate the weight in total
                _weightTotal += (int)tempCard.DrawProbability;
            }

        }
    }


    /// <summary>
    /// shuffle the deck
    /// </summary>
    public void shuffle()
    {
        //check if deck holder was null
        if (_deckHolder.Length > 0)
        {
            for(int i = 0; i < _deckHolder.Length; i++)
            {
                int randIndex = Random.Range(0, 52);
                GameObject temp = _deckHolder[i];
                _deckHolder[i] = _deckHolder[randIndex];
                _deckHolder[randIndex] = temp;
            }
        }
    }

    /// <summary>
    /// random choose a card index with weight
    /// </summary>
    /// <returns></returns>
    public int drawCardIndex()
    {
        //result holds the first value for which the sum of weights is greater than the random number generated
        int result = 0;
        //total sum of weight
        int total = 0;
        //check if the deck is null or not
        if (_deckHolder.Length > 0)
        {
            //choose a random number between 0 and weight total
            int rand = Random.Range(0, _weightTotal);
            for (result = 0; result < _deckHolder.Length; result++)
            {

                //calculate the total of the weights
                total += (int)_deckHolder[result].GetComponentInChildren<Card>().DrawProbability;
                //once we find a random number smaller than the total, we break
                if (total > rand) break;


            }
        }

        return result;
    }
}
