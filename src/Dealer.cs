// *****************************************************************
// Title: Dealer.cs
// Author: John Fitzgerald
// Date: 1-29-17
// Description: Dealer is a singleton class that uses a single Dealer 
// to either randomly generates players' hands, or it takes in user 
// input of each players' hands. Dealer also ranks each Player's hand 
// and determines the Poker Game's winning Player.
// *****************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class Dealer : PokerGame
{
    private static Dealer instance;
    public Dictionary<string, int> HandStrengths;

    /// <summary>
    /// ensure the class only returns a single instance
    /// </summary>
    public static Dealer Instance
    {
        get
        {
            if (instance == null)
                instance = new Dealer();

            return instance;
        }
    }

    /// <summary>
    /// default constructor
    /// </summary>
    private Dealer()
    {
        Populate_Hand_Strengths();
    }

    /// <summary>
    /// initialize the player's hand strengths with 0
    /// </summary>
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

    /// <summary>
    /// randomly generates cards for each hand
    /// </summary>
    public void Randomly_Generate_Cards()
    {
        Random rand = new Random();
        int count = 0;
        List<Deck.Card> cardsToBeDealt = new List<Deck.Card>(CardDeck.FullDeck);
        int cardNumber;

        Console.WriteLine("\nRandomly generate cards");

        // deals cards to each player (simulates the fact that cards are dealt one at a time to each player)
        while (count < (NumberOfPlayers * NumberOfCardsPerHand) && count < CardDeck.FullDeck.Count)
        {
            int num = (count % 2) + 1;
            Players.TryGetValue(PlayerName + num, out Player player);

            cardNumber = rand.Next(0, 52 - count);
            // count modulus two will only work with two players
            player.Hand.Add(cardsToBeDealt[cardNumber]);
            // remove card from cardsToBeDealt
            cardsToBeDealt.RemoveAt(cardNumber);

            ++count;
        }

        Player.Order_Hands();
        Display.Display_Hands();
    }

    /// <summary>
    /// prompt the user to enter the two hands
    /// </summary>
    public void Manually_Enter_Cards()
    {
        string hand;
        bool isHandValid;

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

        Player.Order_Hands();
        //Display.Display_Hands();
    }

    /// <summary>
    /// determine the rank of each player's hand
    /// </summary>
    public void Rank_Players_Hands()
    {
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
            List<Deck.Card> hand = player.Value.Hand;
            Dictionary<string, List<Deck.Card>> allHandStrengths = new Dictionary<string, List<Deck.Card>>(player.Value.AllHandStrengths);
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
            player.Value.BestHandStrength["High Card"] = new List<Deck.Card>(cards);
            player.Value.ValueOfBestHand = HandStrengths["High Card"];

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

                player.Value.BestHandStrength["Full House"] = new List<Deck.Card>(cards);
                player.Value.ValueOfBestHand = HandStrengths["Full House"];
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

                player.Value.BestHandStrength["Full House"] = new List<Deck.Card>(cards);
                player.Value.ValueOfBestHand = HandStrengths["Full House"];
            }

            // *********************************************************************
            // **************** Straight/Straight Flush/Royal Flush ****************
            // *********************************************************************
            if ((Deck.Values_List[Deck.Values_List.IndexOf(hand[i].value) + 1] == hand[i + 1].value 
                    && Deck.Values_List[Deck.Values_List.IndexOf(hand[i + 1].value) + 1] == hand[i + 2].value
                    && Deck.Values_List[Deck.Values_List.IndexOf(hand[i + 2].value) + 1] == hand[i + 3].value
                    && Deck.Values_List[Deck.Values_List.IndexOf(hand[i + 3].value) + 1] == hand[i + 4].value) // values
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

                    player.Value.BestHandStrength[tempName] = new List<Deck.Card>(cards);
                    player.Value.ValueOfBestHand = HandStrengths[tempName];
            }
            // ***************************************
            // **************** Flush **************** 
            // ***************************************
            if (!(Deck.Values_List[Deck.Values_List.IndexOf(hand[i].value) + 1] == hand[i + 1].value 
                    && Deck.Values_List[Deck.Values_List.IndexOf(hand[i + 1].value) + 1] == hand[i + 2].value
                    && Deck.Values_List[Deck.Values_List.IndexOf(hand[i + 2].value) + 1] == hand[i + 3].value
                    && Deck.Values_List[Deck.Values_List.IndexOf(hand[i + 3].value) + 1] == hand[i + 4].value) // values
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

                player.Value.BestHandStrength["Flush"] = new List<Deck.Card>(cards);
                player.Value.ValueOfBestHand = HandStrengths["Flush"];
            }

            if (player.Value.ValueOfBestHand< 8)   // less than straight flush or royal flush
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

                    player.Value.BestHandStrength["Four of a Kind"] = new List<Deck.Card>(cards);
                    player.Value.ValueOfBestHand = HandStrengths["Four of a Kind"];
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

                    player.Value.BestHandStrength["Four of a Kind"] = new List<Deck.Card>(cards);
                    player.Value.ValueOfBestHand = HandStrengths["Four of a Kind"];
                }

                if (player.Value.ValueOfBestHand< 4)   // less than flush
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

                        player.Value.BestHandStrength["Three of a Kind"] = new List<Deck.Card>(cards);
                        player.Value.ValueOfBestHand = HandStrengths["Three of a Kind"];
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

                        player.Value.BestHandStrength["Three of a Kind"] = new List<Deck.Card>(cards);
                        player.Value.ValueOfBestHand = HandStrengths["Three of a Kind"];
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

                        player.Value.BestHandStrength["Three of a Kind"] = new List<Deck.Card>(cards);
                        player.Value.ValueOfBestHand = HandStrengths["Three of a Kind"];
                    }

                    if (player.Value.ValueOfBestHand< 3)   // less than three of a kind
                    {
                        for (i = 0; i<NumberOfCardsPerHand; ++i)
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

                                    player.Value.BestHandStrength["One Pair"] = new List<Deck.Card>(cards);
                                    player.Value.ValueOfBestHand = HandStrengths["One Pair"];

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

                                            player.Value.BestHandStrength["Two Pair"] = new List<Deck.Card>(cards);
                                            player.Value.ValueOfBestHand = HandStrengths["Two Pair"];
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

                                                player.Value.BestHandStrength["Two Pair"] = new List<Deck.Card>(cards);
                                                player.Value.ValueOfBestHand = HandStrengths["Two Pair"];
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
    }

    /// <summary>
    /// determine and get the winner's name
    /// </summary>
    /// <returns></returns>
    public string Get_Winner()
    {
        // ******************************************
        // ***********Determine Winner **************
        // ******************************************
        Player winningPlayer = new Player();
        List<Player> players = new List<Player>();
        List<Deck.Card> tempCard1 = new List<Deck.Card>();
        List<Deck.Card> tempCard2 = new List<Deck.Card>();
        winningPlayer.Name = DefaultName;
        winningPlayer.ValueOfBestHand = 0;
        string typeOfHand;

        foreach (var player in Players)
            players.Add(player.Value);
            
        // iterate through each player
        for (int i = 0; i < NumberOfPlayers; ++i)
        {
            int playerNum = i + 1;
            players[i] = Players["Hand " + playerNum];

            if (players[i].ValueOfBestHand > winningPlayer.ValueOfBestHand)
                winningPlayer = new Player(players[i]);

            typeOfHand = HandStrengths.FirstOrDefault(x => x.Value == players[0].ValueOfBestHand).Key;

            // ***** Full House uses higher of x3 same value to resolve tie)
            if (typeOfHand == "Full House")
            {
                // retrieve values for highest three of a kind
                int sizeOfHand = 0;

                tempCard1 = players[0].BestHandStrength[typeOfHand];
                tempCard2 = players[1].BestHandStrength[typeOfHand];

                tempCard1 = players[0].AllHandStrengths["Three of a Kind"];
                tempCard2 = players[1].AllHandStrengths["Three of a Kind"];

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
        if (winningPlayer.Name == DefaultName)
        {
            // same type of hand: need to verify highest value card
            if (players[0].ValueOfBestHand == players[1].ValueOfBestHand)
            {
                // return the type of best hand each player had
                typeOfHand = HandStrengths.FirstOrDefault(x => x.Value == players[0].ValueOfBestHand).Key;
                tempCard1 = new List<Deck.Card>();
                tempCard2 = new List<Deck.Card>();
                int sizeOfHand = 0;

                tempCard1 = players[0].BestHandStrength[typeOfHand];
                tempCard2 = players[1].BestHandStrength[typeOfHand];

                sizeOfHand = tempCard1.Count - 1;

                // determine winner
                if (tempCard1[sizeOfHand].value > tempCard1[sizeOfHand].value)
                    winningPlayer = new Player(players[0]);
                else
                    winningPlayer = new Player(players[1]);
            }
        }

        return winningPlayer.Name + " wins";
    }
}