// *****************************************************************
// Title: PokerHandVerifier.cs
// Author: John Fitzgerald
// Date: 3-23-17
// Description: Poker-Hand-Verifier displays the winning hand 
// between two 5-card poker hands. The program starts by prompting 
// the user for a menu option: random hands, manual input of hands,
// or quit program. After the user determines how they want to
// execute the input of hands, the program verifies the input, 
// orders the hands in ascending order, determines the winner, 
// and displays the results. Finally, the user is prompted to
// play again or quit program.
// *****************************************************************
using System;
using System.Collections.Generic;
using System.Linq;      // used for charArray.Contains()

public class Dealer
{
    static void Main(string[] args)
    {
        Initialize_Game();
        char charInput;

        Console.WriteLine("-----------------------------------");
        Console.WriteLine("Welcome to the Poker Hand Verifier!");
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("This program simulates two poker hands and verifies winner.");

        while (IsPlay)
        {
            Display.Display_Start_Menu();
            ConsoleKeyInfo decision = Console.ReadKey();

            charInput = Validation.Validate_Input_Menu(decision, "Start");
            Validation.Validate_Input_Decision(charInput);
            Display_Result();
        }
    }

    public static char[] MenuOptionsStart = { 'R', 'r', 'M', 'm', 'Q', 'q' };
    public static char[] MenuOptionsEnd = { 'P', 'p', 'Q', 'q' };
    public static string PlayerName = "Hand ";
    public static string DefaultName = "error";

    public static Deck Decks;
    public static Dictionary<char, int> Values;
    public static List<char> Values_List;
    public static Dictionary<char, int> Suits;
    public static Dictionary<string, int> HandStrengths;
    //public static List<Deck.Card> Deck;
    public static Dictionary<string, Player> Players;

    public static int NumberOfPlayers = 2;
    public static int NumberOfCardsPerHand = 5;

    public static bool IsPlay = true;
    public static bool IsManual = false;
    public static string ErrorHand;

    // initializes hands, deck, players, and populates the deck
    public static void Initialize_Game()
    {
        //Hands = new List<String>();
        //Deck = new List<Deck.Card>();
        Decks = new Deck();
        Players = new Dictionary<string, Player>();
    }

    // initializes the players (aka the hands)
    public static void Initialize_Players()
    {
        Players.Clear();

        foreach (var player in Player.Populate_Players(NumberOfPlayers))
            Players.Add(player.name, player);
    }

    // randomly generates cards for each hand
    public static void Randomly_Generate_Cards()
    {
        Random rand = new Random();
        int count = 0;
        List<Deck.Card> cardsToBeDealt = new List<Deck.Card>(Decks.FullDeck);
        int cardNumber;

        Console.WriteLine("\nRandomly generate cards");

        Initialize_Players();

        // deals cards to each player (simulates the fact that cards are dealt one at a time to each player)
        while (count < (NumberOfPlayers * NumberOfCardsPerHand) && count < Decks.FullDeck.Count)
        {
            int num = (count % 2) + 1;
            Players.TryGetValue(PlayerName + num, out Player player);

            cardNumber = rand.Next(0, 52 - count);
            // count modulus two will only work with two players
            player.hand.Add(cardsToBeDealt[cardNumber]);
            // remove card from cardsToBeDealt
            cardsToBeDealt.RemoveAt(cardNumber);

            ++count;
        }

        Order_Hands();
        Display_Hands();
    }

    // prompt the user to enter the two hands
    public static void Manually_Enter_Cards()
    {
        string hand;
        bool isHandValid;
        Initialize_Players();

        Console.WriteLine("\nManually enter cards");

        // enter two paramaters in one line, split with a space
        Console.Write(">Dealer ");
        hand = Console.ReadLine();
        isHandValid = Validation.Validate_Input_Hand(hand);
        while (!isHandValid)
        {
            Console.WriteLine(ErrorHand);
            Console.Write(">Dealer ");
            hand = Console.ReadLine();

            isHandValid = Validation.Validate_Input_Hand(hand);
        }

        Order_Hands();
        //Display.Display_Hands();
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
                    Values.TryGetValue(hand[j].value, out int first);
                    Values.TryGetValue(hand[minimum].value, out int second);

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

    // display each player's hand
    public static void Display_Hands()
    {
        Console.Write(">Dealer ");
        // iterate through each player
        foreach (var player in Players)
        {
            List<Deck.Card> hand = player.Value.hand;

            foreach (Deck.Card card in hand)
                Console.Write(card.value.ToString() + card.suit.ToString());

            Console.Write(" ");
        }
        Console.Write("\n");
    }

    // display the win or tie
    public static void Display_Result()
    {
        string result = Get_Result();
        Console.WriteLine(result);
    }

    // get the result of the game (assumes cards are already ordered)
    public static string Get_Result()
    {
        string result = "";

        // *** Types of Poker Hands (ranked in ascending order) *** 
        // 1) High Card (x1 high value, x4 random)
        // 2) One Pair (x2 same value, x3 random)
        // 3) Two Pair (x2 same1 value, x2 same2 value, x1 random)
        // 4) Three of a Kind (x3 same value, x2 random)
        // 5) Straight (x5 sequential order with 2 different suits)
        // 6) Flush (x5 with same suit - player with highest value wins)
        // 7) Full House (x3 same1 value, x2 same2 value)
        // 8) Four of a Kind (x4 same value, x1 random)
        // 9) Straight Flush (x5 sequential order with same suit - higher value wins)
        // 10) Royal Flush (x5 sequential order with same suit: 10 through Ace)

        // Ties can happen as suits don't matter
        // three pairs only count as two of pairs (choose high two pairs)
        // Aces can't wrap to form a flush (e.g. can't have KA234)
        // Full House uses higher of x3 same value to resolve tie)

        // iterate through each player
        foreach (var player in Players)
        {
            List<Deck.Card> hand = player.Value.hand;
            Dictionary<string, List<Deck.Card>> allHandStrengths = new Dictionary<string, List<Deck.Card>>(player.Value.allHandStrengths);
            List<Deck.Card> cards = new List<Deck.Card>();
            List<Deck.Card> tempCard_List = new List<Deck.Card>();
            List<Deck.Card> tempOnePair_List = new List<Deck.Card>();
            List<Deck.Card> tempThreeOf_List = new List<Deck.Card>();
            List<char> typesOfSuitsinHand = new List<char>();

            Deck.Card card0, card1, card2, card3, card4;
            // **********************************************************************************
            // **************** high card (requires last card of organized hand) ****************
            // **********************************************************************************
            card0.value = hand[NumberOfCardsPerHand - 1].value;
            card0.suit = hand[NumberOfCardsPerHand - 1].suit;
            cards.Add(card0);

            allHandStrengths.TryGetValue("High Card", out tempCard_List);
            player.Value.bestHandStrength["High Card"] = new List<Deck.Card>(cards);
            player.Value.valueOfBestHand = HandStrengths["High Card"];

            tempCard_List.Clear();
            tempCard_List.Add(card0);

            int i = 0;

            // determine number of suits in hand
            foreach (Deck.Card card in hand)
            {
                if (!typesOfSuitsinHand.Contains(card.suit))
                    typesOfSuitsinHand.Add(card.suit);
            }
            // *********************************************************************
            // **************** Full House (two types x3x2 or x2x3) ****************
            // *********************************************************************
            // 2x3 full house
            if ((hand[i].value == hand[i + 1].value)
                    && (hand[i + 2].value == hand[i + 3].value && hand[i + 3].value == hand[i + 4].value))
            {
                cards.Clear();

                card0.value = hand[i].value;
                card0.suit = hand[i].suit;
                card1.value = hand[i + 1].value;
                card1.suit = hand[i + 1].suit;
                card2.value = hand[i + 2].value;
                card2.suit = hand[i + 2].suit;
                card3.value = hand[i + 3].value;
                card3.suit = hand[i + 3].suit;
                card4.value = hand[i + 4].value;
                card4.suit = hand[i + 4].suit;
                cards.Add(card0);
                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);

                // update three of a kind (to resolve ties)
                allHandStrengths.TryGetValue("Three of a Kind", out tempCard_List);
                tempCard_List.Clear();
                tempCard_List.Add(card2);
                tempCard_List.Add(card3);
                tempCard_List.Add(card4);

                allHandStrengths.TryGetValue("Full House", out tempCard_List);
                tempCard_List.Clear();

                // update full house list
                foreach (var _card in cards)
                    tempCard_List.Add(_card);

                player.Value.bestHandStrength["Full House"] = new List<Deck.Card>(cards);
                player.Value.valueOfBestHand = HandStrengths["Full House"];
            }
            // 3x2 full house
            else if ((hand[i].value == hand[i + 1].value && hand[i + 1].value == hand[i + 2].value)
                        && (hand[i + 3].value == hand[i + 4].value))
            {
                cards.Clear();

                card0.value = hand[i].value;
                card0.suit = hand[i].suit;
                card1.value = hand[i + 1].value;
                card1.suit = hand[i + 1].suit;
                card2.value = hand[i + 2].value;
                card2.suit = hand[i + 2].suit;
                card3.value = hand[i + 3].value;
                card3.suit = hand[i + 3].suit;
                card4.value = hand[i + 4].value;
                card4.suit = hand[i + 4].suit;
                cards.Add(card0);
                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);

                // update three of a kind (to resolve ties)
                allHandStrengths.TryGetValue("Three of a Kind", out tempCard_List);
                tempCard_List.Clear();
                tempCard_List.Add(card0);
                tempCard_List.Add(card1);
                tempCard_List.Add(card2);

                allHandStrengths.TryGetValue("Full House", out tempCard_List);
                tempCard_List.Clear();

                // update full house list
                foreach (var _card in cards)
                    tempCard_List.Add(_card);

                player.Value.bestHandStrength["Full House"] = new List<Deck.Card>(cards);
                player.Value.valueOfBestHand = HandStrengths["Full House"];
            }

            // *********************************************************************
            // **************** Straight/Straight Flush/Royal Flush ****************
            // *********************************************************************
            if ((Values_List[Values_List.IndexOf(hand[i].value) + 1] == hand[i + 1].value 
                    && Values_List[Values_List.IndexOf(hand[i + 1].value) + 1] == hand[i + 2].value
                    && Values_List[Values_List.IndexOf(hand[i + 2].value) + 1] == hand[i + 3].value
                    && Values_List[Values_List.IndexOf(hand[i + 3].value) + 1] == hand[i + 4].value) // values
                    &&
                    (typesOfSuitsinHand.Count <= 2)) // suits
            {
                string tempName = "";

                if (typesOfSuitsinHand.Count == 2)
                {
                    tempName = "Straight";
                    allHandStrengths.TryGetValue("Straight", out tempCard_List);
                }
                else if (typesOfSuitsinHand.Count == 1 && hand[i + 4].value == 'A')
                {
                    tempName = "Royal Flush";
                    allHandStrengths.TryGetValue("Royal Flush", out tempCard_List);
                }
                else if (typesOfSuitsinHand.Count == 1)
                {
                    tempName = "Straight Flush";
                    allHandStrengths.TryGetValue("Straight Flush", out tempCard_List);
                }

                tempCard_List.Clear();
                cards.Clear();

                card0.value = hand[i].value;
                card0.suit = hand[i].suit;
                card1.value = hand[i + 1].value;
                card1.suit = hand[i + 1].suit;
                card2.value = hand[i + 2].value;
                card2.suit = hand[i + 2].suit;
                card3.value = hand[i + 3].value;
                card3.suit = hand[i + 3].suit;
                card4.value = hand[i + 4].value;
                card4.suit = hand[i + 4].suit;
                cards.Add(card0);
                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);

                // update straight/straight flush list
                foreach (var _card in cards)
                    tempCard_List.Add(_card);

                player.Value.bestHandStrength[tempName] = new List<Deck.Card>(cards);
                player.Value.valueOfBestHand = HandStrengths[tempName];
            }
            // ***************************************
            // **************** Flush **************** 
            // ***************************************
            if (!(Values_List[Values_List.IndexOf(hand[i].value) + 1] == hand[i + 1].value 
                    && Values_List[Values_List.IndexOf(hand[i + 1].value) + 1] == hand[i + 2].value
                    && Values_List[Values_List.IndexOf(hand[i + 2].value) + 1] == hand[i + 3].value
                    && Values_List[Values_List.IndexOf(hand[i + 3].value) + 1] == hand[i + 4].value) // values
                    &&
                    (typesOfSuitsinHand.Count == 1)) // suits
            {
                allHandStrengths.TryGetValue("Flush", out tempCard_List);

                tempCard_List.Clear();
                cards.Clear();

                card0.value = hand[i].value;
                card0.suit = hand[i].suit;
                card1.value = hand[i + 1].value;
                card1.suit = hand[i + 1].suit;
                card2.value = hand[i + 2].value;
                card2.suit = hand[i + 2].suit;
                card3.value = hand[i + 3].value;
                card3.suit = hand[i + 3].suit;
                card4.value = hand[i + 4].value;
                card4.suit = hand[i + 4].suit;
                cards.Add(card0);
                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);

                // update flush list
                foreach (var _card in cards)
                    tempCard_List.Add(_card);

                player.Value.bestHandStrength["Flush"] = new List<Deck.Card>(cards);
                player.Value.valueOfBestHand = HandStrengths["Flush"];
            }

            if (player.Value.valueOfBestHand < 8)   // less than straight flush or royal flush
            {
                // ************************************************
                // **************** Four of a Kind ****************
                // ************************************************
                if (hand[i].value == hand[i + 1].value
                    && hand[i + 1].value == hand[i + 2].value
                    && hand[i + 2].value == hand[i + 3].value)
                {
                    cards.Clear();

                    card0.value = hand[i].value;
                    card0.suit = hand[i].suit;
                    card1.value = hand[i + 1].value;
                    card1.suit = hand[i + 1].suit;
                    card2.value = hand[i + 2].value;
                    card2.suit = hand[i + 2].suit;
                    card3.value = hand[i + 3].value;
                    card3.suit = hand[i + 3].suit;
                    cards.Add(card0);
                    cards.Add(card1);
                    cards.Add(card2);
                    cards.Add(card3);

                    allHandStrengths.TryGetValue("Four of a Kind", out tempCard_List);
                    tempCard_List.Clear();

                    // assign hand to player
                    foreach (var _card in cards)
                        tempCard_List.Add(_card);

                    player.Value.bestHandStrength["Four of a Kind"] = new List<Deck.Card>(cards);
                    player.Value.valueOfBestHand = HandStrengths["Four of a Kind"];
                }
                // four of a kind starting at index 1
                else if (hand[i + 1].value == hand[i + 2].value
                        && hand[i + 2].value == hand[i + 3].value
                        && hand[i + 3].value == hand[i + 4].value)
                {
                    cards.Clear();

                    card0.value = hand[i + 1].value;
                    card0.suit = hand[i + 1].suit;
                    card1.value = hand[i + 2].value;
                    card1.suit = hand[i + 2].suit;
                    card2.value = hand[i + 3].value;
                    card2.suit = hand[i + 3].suit;
                    card3.value = hand[i + 4].value;
                    card3.suit = hand[i + 4].suit;
                    cards.Add(card0);
                    cards.Add(card1);
                    cards.Add(card2);
                    cards.Add(card3);

                    allHandStrengths.TryGetValue("Four of a Kind", out tempCard_List);
                    tempCard_List.Clear();

                    // assign hand to player
                    foreach (var _card in cards)
                        tempCard_List.Add(_card);

                    player.Value.bestHandStrength["Four of a Kind"] = new List<Deck.Card>(cards);
                    player.Value.valueOfBestHand = HandStrengths["Four of a Kind"];
                }

                if (player.Value.valueOfBestHand < 4)   // less than flush
                {
                    // *************************************************
                    // **************** Three of a Kind ****************
                    // *************************************************
                    if (hand[i].value == hand[i + 1].value && hand[i + 1].value == hand[i + 2].value)
                    {
                        cards.Clear();

                        card0.value = hand[i].value;
                        card0.suit = hand[i].suit;
                        card1.value = hand[i + 1].value;
                        card1.suit = hand[i + 1].suit;
                        card2.value = hand[i + 2].value;
                        card2.suit = hand[i + 2].suit;
                        cards.Add(card0);
                        cards.Add(card1);
                        cards.Add(card2);

                        allHandStrengths.TryGetValue("Three of a Kind", out tempCard_List);
                        tempCard_List.Clear();

                        // assign hand to player
                        foreach (var _card in cards)
                            tempCard_List.Add(_card);

                        player.Value.bestHandStrength["Three of a Kind"] = new List<Deck.Card>(cards);
                        player.Value.valueOfBestHand = HandStrengths["Three of a Kind"];
                    }
                    // three of a kind starting at index 1
                    else if (hand[i + 1].value == hand[i + 2].value && hand[i + 2].value == hand[i + 3].value)
                    {
                        cards.Clear();

                        card0.value = hand[i + 1].value;
                        card0.suit = hand[i + 1].suit;
                        card1.value = hand[i + 2].value;
                        card1.suit = hand[i + 2].suit;
                        card2.value = hand[i + 3].value;
                        card2.suit = hand[i + 3].suit;
                        cards.Add(card0);
                        cards.Add(card1);
                        cards.Add(card2);

                        allHandStrengths.TryGetValue("Three of a Kind", out tempCard_List);
                        tempCard_List.Clear();

                        // assign hand to player
                        foreach (var _card in cards)
                            tempCard_List.Add(_card);

                        player.Value.bestHandStrength["Three of a Kind"] = new List<Deck.Card>(cards);
                        player.Value.valueOfBestHand = HandStrengths["Three of a Kind"];
                    }
                    // three of a kind starting at index 2
                    else if (hand[i + 2].value == hand[i + 3].value && hand[i + 3].value == hand[i + 4].value)
                    {
                        cards.Clear();

                        card0.value = hand[i + 2].value;
                        card0.suit = hand[i + 2].suit;
                        card1.value = hand[i + 3].value;
                        card1.suit = hand[i + 3].suit;
                        card2.value = hand[i + 4].value;
                        card2.suit = hand[i + 4].suit;
                        cards.Add(card0);
                        cards.Add(card1);
                        cards.Add(card2);

                        allHandStrengths.TryGetValue("Three of a Kind", out tempCard_List);
                        tempCard_List.Clear();

                        // assign hand to player
                        foreach (var _card in cards)
                            tempCard_List.Add(_card);

                        player.Value.bestHandStrength["Three of a Kind"] = new List<Deck.Card>(cards);
                        player.Value.valueOfBestHand = HandStrengths["Three of a Kind"];
                    }

                    if (player.Value.valueOfBestHand < 3)   // less than three of a kind
                    {
                        for (i = 0; i < NumberOfCardsPerHand; ++i)
                        {
                            cards.Clear();
                            card0.value = hand[i].value;
                            card0.suit = hand[i].suit;
                            cards.Add(card0);
                            // *****************************************
                            // **************** Pair(s) ****************
                            // *****************************************
                            if (i + 1 < NumberOfCardsPerHand)   // bounds check
                            {
                                // pair found
                                if (hand[i].value == hand[i + 1].value)
                                {
                                    allHandStrengths.TryGetValue("One Pair", out tempCard_List);

                                    cards.Clear();
                                    cards.Add(card0);
                                    card1.value = hand[i + 1].value;
                                    card1.suit = hand[i + 1].suit;
                                    cards.Add(card1);
                                    tempCard_List.Clear();

                                    // assigns hand to player
                                    foreach (var _card in cards)
                                        tempCard_List.Add(_card);

                                    // store previous one pair for possible two pair
                                    tempOnePair_List = new List<Deck.Card>(tempCard_List);

                                    player.Value.bestHandStrength["One Pair"] = new List<Deck.Card>(cards);
                                    player.Value.valueOfBestHand = HandStrengths["One Pair"];

                                    // ******************************************
                                    // **************** Two Pair ****************
                                    // ******************************************
                                    if (i + 3 < NumberOfCardsPerHand)   // bounds check
                                    {
                                        // two pair found
                                        if (hand[i + 2].value == hand[i + 3].value)
                                        {
                                            // update one pair
                                            allHandStrengths.TryGetValue("One Pair", out tempCard_List);
                                            tempCard_List.Clear();
                                            cards.Clear();

                                            card2.value = hand[i + 2].value;
                                            card2.suit = hand[i + 2].suit;
                                            card3.value = hand[i + 3].value;
                                            card3.suit = hand[i + 3].suit;
                                            cards.Add(card2);
                                            cards.Add(card3);

                                            // update one pair list
                                            foreach (var _card in cards)
                                                tempCard_List.Add(_card);

                                            // add current two pair to previous two pair
                                            foreach (var _card in cards)
                                                tempOnePair_List.Add(_card);

                                            allHandStrengths.TryGetValue("Two Pair", out tempCard_List);
                                            tempCard_List.Clear();

                                            // assign hand to player
                                            foreach (var _card in tempOnePair_List)
                                                tempCard_List.Add(_card);

                                            player.Value.bestHandStrength["Two Pair"] = new List<Deck.Card>(cards);
                                            player.Value.valueOfBestHand = HandStrengths["Two Pair"];
                                        }
                                        // two pair found
                                        if (i + 4 < NumberOfCardsPerHand)   // bounds check
                                        {
                                            if (hand[i + 3].value == hand[i + 4].value)
                                            {
                                                // update one pair
                                                allHandStrengths.TryGetValue("One Pair", out tempCard_List);
                                                tempCard_List.Clear();
                                                cards.Clear();

                                                card2.value = hand[i + 3].value;
                                                card2.suit = hand[i + 3].suit;
                                                card3.value = hand[i + 4].value;
                                                card3.suit = hand[i + 4].suit;
                                                cards.Add(card2);
                                                cards.Add(card3);

                                                // update one pair list
                                                foreach (var _card in cards)
                                                    tempCard_List.Add(_card);

                                                // add current two pair to previous two pair
                                                foreach (var _card in cards)
                                                    tempOnePair_List.Add(_card);

                                                allHandStrengths.TryGetValue("Two Pair", out tempCard_List);
                                                tempCard_List.Clear();

                                                // assign hand to player
                                                foreach (var _card in tempOnePair_List)
                                                    tempCard_List.Add(_card);

                                                player.Value.bestHandStrength["Two Pair"] = new List<Deck.Card>(cards);
                                                player.Value.valueOfBestHand = HandStrengths["Two Pair"];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // ******************************************
        // ***********Determine Winner **************
        // ******************************************
        Player winningPlayer = new Player();
        List<Player> players = new List<Player>();
        List<Deck.Card> tempCard1 = new List<Deck.Card>();
        List<Deck.Card> tempCard2 = new List<Deck.Card>();
        winningPlayer.name = DefaultName;
        winningPlayer.valueOfBestHand = 0;
        string typeOfHand;

        foreach (var player in Players)
            players.Add(player.Value);
            
        // iterate through each player
        for (int i = 0; i < NumberOfPlayers; ++i)
        {
            int playerNum = i + 1;
            players[i] = Players["Hand " + playerNum];

            if (players[i].valueOfBestHand > winningPlayer.valueOfBestHand)
                winningPlayer = new Player(players[i]);

            typeOfHand = HandStrengths.FirstOrDefault(x => x.Value == players[0].valueOfBestHand).Key;

            // ***** Full House uses higher of x3 same value to resolve tie)
            if (typeOfHand == "Full House")
            {
                // retrieve values for highest three of a kind
                int sizeOfHand = 0;

                tempCard1 = players[0].bestHandStrength[typeOfHand];
                tempCard2 = players[1].bestHandStrength[typeOfHand];

                tempCard1 = players[0].allHandStrengths["Three of a Kind"];
                tempCard2 = players[1].allHandStrengths["Three of a Kind"];

                // compare highest three of a kind values and determine winner
                sizeOfHand = tempCard1.Count - 1;

                // determine winner
                if (tempCard1[sizeOfHand].value > tempCard1[sizeOfHand].value)
                    winningPlayer = new Player(players[0]);
                else
                    winningPlayer = new Player(players[1]);
            }
        }

        // resolve tie based on higher value card
        if (winningPlayer.name == DefaultName)
        {
            // same type of hand: need to verify highest value card
            if (players[0].valueOfBestHand == players[1].valueOfBestHand)
            {
                // return the type of best hand each player had
                typeOfHand = HandStrengths.FirstOrDefault(x => x.Value == players[0].valueOfBestHand).Key;
                tempCard1 = new List<Deck.Card>();
                tempCard2 = new List<Deck.Card>();
                int sizeOfHand = 0;

                tempCard1 = players[0].bestHandStrength[typeOfHand];
                tempCard2 = players[1].bestHandStrength[typeOfHand];

                sizeOfHand = tempCard1.Count - 1;

                // determine winner
                if (tempCard1[sizeOfHand].value > tempCard1[sizeOfHand].value)
                    winningPlayer = new Player(players[0]);
                else
                    winningPlayer = new Player(players[1]);
            }
        }

        result = winningPlayer.name + " wins";

        return result;
    }
}