using System.Collections.Generic;

public class Player : Dealer
{
    public string name;
    public List<Deck.Card> hand;
    public Dictionary<string, List<Deck.Card>> allHandStrengths;
    public Dictionary<string, List<Deck.Card>> bestHandStrength;
    public int valueOfBestHand;

    public Player()
    {
        name = "";
        hand = new List<Deck.Card>();
        allHandStrengths = new Dictionary<string, List<Deck.Card>>();
        bestHandStrength = new Dictionary<string, List<Deck.Card>>();
        valueOfBestHand = 0;
    }

    public Player(string _name, int _valueOfBestHand)
    {
        name = _name;
        hand = new List<Deck.Card>();
        allHandStrengths = new Dictionary<string, List<Deck.Card>>();
        bestHandStrength = new Dictionary<string, List<Deck.Card>>();
        valueOfBestHand = _valueOfBestHand;
    }

    public Player(Player player)
    {
        name = player.name;
        hand = player.hand;
        allHandStrengths = player.allHandStrengths;
        bestHandStrength = player.bestHandStrength;
        valueOfBestHand = player.valueOfBestHand;
    }

    public static List<Player> Populate_Players(int numPlayers)
    {
        List<Player> players = new List<Player>();

        for (int i = 1; i <= numPlayers; ++i)
        {
            Player player = new Player(PlayerName + i, 0);
            players.Add(Deck.Populate_AllHand_Strengths(player));
        }

        return players;
    }

    // order each player's hand in ascending order using selection sort algorithm [O(N^2)]
    public static void Order_Hands()
    {
        // iterate through each player
        foreach (var player in Players)
        {
            List<Deck.Card> hand = player.Value.hand;

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

    // helper function for Order_Hands(): swaps two Cards in a List
    private static void Swap(List<Deck.Card> hand, int index1, int index2)
    {
        Deck.Card temp = hand[index1];
        hand[index1] = hand[index2];
        hand[index2] = temp;
    }
}