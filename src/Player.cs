// *****************************************************************
// Title: Player.cs
// Author: John Fitzgerald
// Date: 1-29-17
// Description: The Player is essentially a hand of cards used in
// the Poker Game. Before the game is resolved, the hand should be
// sorted in ascending order.
// *****************************************************************
using System.Collections.Generic;

public class Player : PokerGame
{
    public string Name;
    public int ValueOfBestHand;
    public List<Deck.Card> Hand;

    // TODO: combine the two and create an easier ranking system
    public Dictionary<string, List<Deck.Card>> AllHandStrengths, BestHandStrength;

    // default constructor
    public Player()
    {
        Name = "";
        Hand = new List<Deck.Card>();
        AllHandStrengths = new Dictionary<string, List<Deck.Card>>();
        BestHandStrength = new Dictionary<string, List<Deck.Card>>();
        ValueOfBestHand = 0;
    }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_valueOfBestHand"></param>
    public Player(string _name, int _valueOfBestHand)
    {
        Name = _name;
        Hand = new List<Deck.Card>();
        AllHandStrengths = new Dictionary<string, List<Deck.Card>>();
        BestHandStrength = new Dictionary<string, List<Deck.Card>>();
        ValueOfBestHand = _valueOfBestHand;
    }

    /// <summary>
    /// copy constructor
    /// </summary>
    /// <param name="player"></param>
    public Player(Player player)
    {
        Name = player.Name;
        Hand = player.Hand;
        AllHandStrengths = player.AllHandStrengths;
        BestHandStrength = player.BestHandStrength;
        ValueOfBestHand = player.ValueOfBestHand;
    }

    /// <summary>
    /// create N players and populate their hands
    /// </summary>
    /// <param name="numPlayers"></param>
    /// <returns></returns>
    public static List<Player> Populate_Players(int numPlayers)
    {
        List<Player> players = new List<Player>();

        for (int i = 1; i <= numPlayers; ++i)
        {
            Player player = new Player(PlayerName + i, 0);
            player.Populate_Player_Hand_Strengths();
            players.Add(player);
        }

        return players;
    }

    /// <summary>
    /// populate the card hand strengths
    /// </summary>
    public void Populate_Player_Hand_Strengths()
    {
        foreach (var strength in Dealer_Single.HandStrengths)
            AllHandStrengths.Add(strength.Key, new List<Deck.Card> { new Deck.Card('0', '0') });

        //var item = HandStrengths.GetEnumerator();
        //item.MoveNext();
        //for (int i = 0; i <= HandStrengths.Count - 1; item.MoveNext(), ++i)
        //    player.allHandStrengths.Add(item.Current.Key.ToString(), new List<Card> { new Card('0', '0') });
    }

    /// <summary>
    /// order each player's hand in ascending order using selection sort algorithm [O(N^2)]
    /// </summary>
    public static void Order_Hands()
    {
        foreach (var player in Players)
        {
            List<Deck.Card> hand = player.Value.Hand;

            // order the hand in ascending order
            for (int i = 0; i < NumberOfCardsPerHand - 1; ++i)
            {
                int minimum = i;

                // iterate through list
                for (int j = i + 1; j < NumberOfCardsPerHand; ++j)
                {
                    Deck.Values.TryGetValue(hand[j].value, out int first);
                    Deck.Values.TryGetValue(hand[minimum].value, out int second);

                    // compare card values
                    if (first < second)
                        minimum = j;
                }

                // swap values
                if (minimum != i)
                    Swap(hand, i, minimum);
            }
        }
    }

    /// <summary>
    /// helper function for Order_Hands(): swaps two Cards in a List
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    private static void Swap(List<Deck.Card> hand, int index1, int index2)
    {
        Deck.Card temp = hand[index1];
        hand[index1] = hand[index2];
        hand[index2] = temp;
    }
}