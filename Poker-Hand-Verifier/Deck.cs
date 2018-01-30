// *****************************************************************
// Title: Deck.cs
// Author: John Fitzgerald
// Date: 1-29-17
// Description: The Deck is the Poker Game's only deck between
// all Players. It contains a standard 52-card deck (along with 
// values and suits) and the hand strengths for the Poker Game.
// *****************************************************************
using System.Collections.Generic;

public class Deck : PokerGame
{
    public static Dictionary<char, int> Values, Suits;
    public static List<char> Values_List;
    public List<Card> FullDeck;

    /// <summary>
    /// default constructor
    /// </summary>
    public Deck()
    {
        FullDeck = new List<Card>();
        Populate_Deck();
    }

    /// <summary>
    /// a single card in the deck
    /// </summary>
    public struct Card
    {
        public char value, suit;

        public Card(char _value, char _suit)
        {
            value = _value;
            suit = _suit;
        }
    }

    /// <summary>
    /// populate a deck of 52 cards (based on content of Values and Suits)
    /// </summary>
    public void Populate_Deck()
    {
        Populate_Values();
        Populate_Values_List();
        Populate_Suits();

        foreach (var _value in Values)
        {
            foreach (var _suit in Suits)
            {
                Card newCard;
                newCard.value = _value.Key;
                newCard.suit = _suit.Key;

                FullDeck.Add(newCard);
            }
        }
    }

    /// <summary>
    /// populate the card values
    /// </summary>
    public void Populate_Values()
    {
        Values = new Dictionary<char, int>()
        {
            {'2', 0},
            {'3', 1},
            {'4', 2},
            {'5', 3},
            {'6', 4},
            {'7', 5},
            {'8', 6},
            {'9', 7},
            {'T', 8},
            {'J', 9},
            {'Q', 10},
            {'K', 11},
            {'A', 12}
        };
    }

    /// <summary>
    /// populate the card values (List)
    /// </summary>
    public void Populate_Values_List()
    {
        Values_List = new List<char>();

        foreach (var val in Values)
            Values_List.Add(val.Key);
    }

    /// <summary>
    /// populate the card suits
    /// </summary>
    public static void Populate_Suits()
    {
        Suits = new Dictionary<char, int>()
        {
            {'C', 0},
            {'S', 1},
            {'H', 2},
            {'D', 3}
        };
    }
}