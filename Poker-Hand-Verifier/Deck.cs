using System.Collections.Generic;

public class Deck : Dealer
{
    public static Dictionary<char, int> Values;
    public static List<char> Values_List;
    public static Dictionary<char, int> Suits;
    public static Dictionary<string, int> HandStrengths;
    public List<Card> FullDeck;

    public Deck()
    {
        FullDeck = new List<Card>();
        Populate_Deck();
    }

    public struct Card
    {
        public char value, suit;

        public Card(char _value, char _suit)
        {
            value = _value;
            suit = _suit;
        }
    }

    // populate a deck of 52 cards (based on content of Values and Suits)
    public void Populate_Deck()
    {
        Populate_Values();
        Populate_Values_List();
        Populate_Suits();
        Populate_Hand_Strengths();

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

    // populate the card values (Dictionary)
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

    // populate the card values (List)
    public void Populate_Values_List()
    {
        Values_List = new List<char>();

        foreach (var val in Values)
            Values_List.Add(val.Key);
    }

    // populate the card suits
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

    // initialize the player's hand strengths with 0
    public void Populate_Hand_Strengths()
    {
        HandStrengths = new Dictionary<string, int>()
        {
            {"High Card", 0},
            {"One Pair", 1},
            {"Two Pair", 2},
            {"Three of a Kind", 3},
            {"Straight", 4},
            {"Flush", 5},
            {"Full House", 6},
            {"Four of a Kind", 7},
            {"Straight Flush", 8},
            {"Royal Flush", 9}
        };
    }

    // TODO decouple dependancy: how to make this unreliant on Player and exist in Deck?
    // populate the card hand strengths
    public static Player Populate_AllHand_Strengths(Player player)
    {
        player.allHandStrengths = new Dictionary<string, List<Card>>()
        {
            {"High Card", new List<Card>()},
            {"One Pair", new List<Card>()},
            {"Two Pair", new List<Card>()},
            {"Three of a Kind", new List<Card>()},
            {"Straight", new List<Card>()},
            {"Flush", new List<Card>()},
            {"Full House", new List<Card>()},
            {"Four of a Kind", new List<Card>()},
            {"Straight Flush", new List<Card>()},
            {"Royal Flush", new List<Card>()}
        };

        // intiialzie with a 0 value card
        foreach (var handType in player.allHandStrengths)
        {
            Card card = new Card('0', '0');
            handType.Value.Add(card);
        }

        return player;
    }
}