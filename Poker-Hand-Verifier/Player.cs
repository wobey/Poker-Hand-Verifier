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
}