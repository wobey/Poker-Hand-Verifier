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

namespace PokerHandVerifier
{
    public class Player
    {
        public string name;
        public List<Dealer.Card> hand;
        public Dictionary<string, List<Dealer.Card>> allHandStrengths;
        public Dictionary<string, List<Dealer.Card>> bestHandStrength;
        public int valueOfBestHand;

        public Player()
        {
            name = "";
            hand = new List<Dealer.Card>();
            allHandStrengths = new Dictionary<string, List<Dealer.Card>>();
            bestHandStrength = new Dictionary<string, List<Dealer.Card>>();
            valueOfBestHand = 0;
        }

        public Player(Player player)
        {
            name = player.name;
            hand = player.hand;
            allHandStrengths = player.allHandStrengths;
            bestHandStrength = player.bestHandStrength;
            valueOfBestHand = player.valueOfBestHand;
        }
    }

    public class Dealer
    {
        public static char[] MenuOptionsStart = { 'R', 'r', 'M', 'm', 'Q', 'q' };
        public static char[] MenuOptionsEnd = { 'P', 'p', 'Q', 'q' };
        public static string PlayerName = "Hand ";

        public static Dictionary<char, int> Values;
        public static List<char> Values_List;
        public static Dictionary<char, int> Suits;
        public static Dictionary<string, int> HandStrengths;
        public static List<Card> Deck;
        public static Dictionary<string, Player> Players;

        public static int NumberOfPlayers = 2;
        public static int NumberOfCardsPerHand = 5;

        public static bool IsPlay = true;
        public static bool IsManual = false;
        public static string ErrorHand;

        public struct Card
        {
            public char value;
            public char suit;
        }

        static void Main()
        {
            Initialize_Game();

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Welcome to the Poker Hand Verifier!");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("This program simulates two poker hands and verifies winner.");

            while (IsPlay)
            {
                char charInput;

                // initial user decision
                if (!IsManual)
                {
                    Display_Start_Menu();
                    ConsoleKeyInfo decision = Console.ReadKey();

                    charInput = Validate_Input_Menu(decision, "Start");
                    Input_Decision(charInput);
                }
                else
                {
                    charInput = 'M';
                    Input_Decision(charInput);
                }

                // ******** End Menu wasn't necessary **********
                //// play again?
                //if (IsPlay)
                //{
                //    // allow user to repeatedly input manual values
                //    if (!IsManual)
                //    {
                //        Display_Start_Menu();
                //        decision = Console.ReadKey();
                //        //charInput = Validate_Input_Menu(decision, "End");
                //        charInput = Validate_Input_Menu(decision, "Start");
                //    }
                    
                //    Input_Decision(charInput);
                //}
            }
        }

        // initializes hands, deck, players, and populates the deck
        public static void Initialize_Game()
        {
            //Hands = new List<String>();
            Deck = new List<Card>();
            Players = new Dictionary<string, Player>();

            Populate_Deck();
        }

        // populate a deck of 52 cards (based on content of Values and Suits)
        public static void Populate_Deck()
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

                    Deck.Add(newCard);
                }
            }
        }

        // populate the card values (Dictionary)
        public static void Populate_Values()
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
        public static void Populate_Values_List()
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
        public static void Populate_Hand_Strengths()
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
                Card card;
                card.value = '0';
                card.suit = '0';
                handType.Value.Add(card);
            }

            return player;
        }

        // initializes the players (aka the hands)
        public static void Initialize_Players()
        {
            Players.Clear();

            for (int j = 1; j <= NumberOfPlayers; ++j)
            {
                Player player = new Player();
                player.name = PlayerName + j;
                player.hand = new List<Card>();
                player.bestHandStrength = new Dictionary<string, List<Card>>();
                player.allHandStrengths = new Dictionary<string, List<Card>>();
                player.valueOfBestHand = 0;

                player = Populate_AllHand_Strengths(player);

                Players.Add(player.name, player);
            }
        }

        // display the initial menu
        public static void Display_Start_Menu()
        {
            Console.WriteLine("\nPlease enter one of the following keys:");
            Console.WriteLine("- Randomly generate cards: R or r");
            Console.WriteLine("- Manually enter cards: M or m");
            Console.WriteLine("- Quit program: Q or q");
        }

        // display the end menu
        public static void Display_End_Menu()
        {
            Console.WriteLine("\nPlease enter one of the following keys:");
            Console.WriteLine("- Play again: P or p");
            Console.WriteLine("- Quit program: Q or q");
        }

        // display play again
        public static void Display_Play_Again()
        {
            Console.WriteLine("\nPlay again");
        }

        // display quit program and terminate play loop
        public static void Display_Quit_Program()
        {
            Console.WriteLine("\nQuit program");
            IsPlay = false;
        }

        // validate user input for menus
        public static char Validate_Input_Menu(ConsoleKeyInfo key, string menuType)
        {
            char charInput = key.KeyChar;
            char[] menuOptions;

            // determine type of menu options being validated
            if (menuType == "Start")
                menuOptions = MenuOptionsStart;
            else
                menuOptions = MenuOptionsEnd;

            // prompt user for a valid option
            while (!menuOptions.Contains(charInput))
            {
                Console.WriteLine("\n{0} is not valid. Please enter a valid character: ", key.KeyChar);
                charInput = Console.ReadKey().KeyChar;
            }

            if (charInput == 'M' || charInput == 'm')
                IsManual = true;

            return charInput;
        }

        // validate the cards in the hand
        public static bool Validate_Input_Hand(string hand)
        {
            // proper length of input (+1 to account for space between hands)
            int inputLength = NumberOfCardsPerHand * NumberOfPlayers * 2 + 1;
            string[] hands;

            // check size of hand
            if (hand.Length > inputLength || hand.Length < inputLength)
            {
                ErrorHand = "Please input two hands seperated by a space.";
                return false;
            }

            // split cards into two hands
            hands = hand.Split(' ');

            // verify each hand contains valid cards and add them to the player's hand
            for (int i = 0; i < NumberOfPlayers; ++i)
            {
                // find player
                Player player;
                int handNum = i + 1;
                Players.TryGetValue(PlayerName + handNum, out player);

                // check contents of hand
                for (int j = 0; j < NumberOfCardsPerHand * 2; ++j)
                {
                    Card checkCard;
                    checkCard.value = hands[i][j];
                    checkCard.suit = hands[i][++j];
                    if (!Deck.Contains(checkCard))
                    {
                        // clear each player's hand (in-case user changes previous players' cards)
                        foreach (var _player in Players)
                            _player.Value.hand.Clear();

                        ErrorHand = "Invalid characters for card detected: " + checkCard.value + checkCard.suit;
                        return false;
                    }

                    // add card to player's hand
                    player.hand.Add(checkCard);
                }
            }

            // TODO: check for duplicates

            return true;
        }

        // decide what to do at start of program based on user input
        public static void Input_Decision(char charInput)
        {
            // randomly generate cards
            if (charInput == 'R' || charInput == 'r')
                Randomly_Generate_Cards();
            // manually enter cards
            else if (charInput == 'M' || charInput == 'm')
                Manually_Enter_Cards();
            // play again
            else if (charInput == 'P' || charInput == 'p')
                Display_Play_Again();
            // quit program
            else if (charInput == 'Q' || charInput == 'q')
                Display_Quit_Program();
            // should never reach here (if using input validation)
            else
                Console.WriteLine("Invalid character detected: {0}", charInput);
        }

        // randomly generates cards for each hand
        public static void Randomly_Generate_Cards()
        {
            Random rand = new Random();
            int count = 0;
            List<Card> cardsToBeDealt = new List<Card>(Deck);
            int cardNumber;

            Console.WriteLine("\nRandomly generate cards");

            Initialize_Players();

            // deals cards to each player (simulates the fact that cards are dealt one at a time to each player)
            while (count < (NumberOfPlayers * NumberOfCardsPerHand) && count < Deck.Count)
            {
                Player player;
                int num = (count % 2) + 1;
                Players.TryGetValue(PlayerName + num, out player);

                cardNumber = rand.Next(0, 52 - count);
                // count modulus two will only work with two players
                player.hand.Add(cardsToBeDealt[cardNumber]);
                // remove card from cardsToBeDealt
                cardsToBeDealt.RemoveAt(cardNumber);

                ++count;
            }

            Order_Hands();
            Display_Hands();

            Display_Result();
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
            isHandValid = Validate_Input_Hand(hand);
            while (!isHandValid)
            {
                Console.WriteLine(ErrorHand);
                Console.Write(">Dealer ");
                hand = Console.ReadLine();

                isHandValid = Validate_Input_Hand(hand);
            }

            Order_Hands();
            //Display_Hands();

            Display_Result();
        }

        // order each player's hand in ascending order using selection sort algorithm [O(N^2)]
        public static void Order_Hands()
        {
            // iterate through each player
            foreach (var player in Players)
            {
                List<Card> hand = player.Value.hand;

                // order the hand in ascending order
                for (int i = 0; i < NumberOfCardsPerHand - 1; ++i)
                {
                    int minimum = i;

                    // iterate through list
                    for (int j = i + 1; j < NumberOfCardsPerHand; ++j)
                    {
                        int first, second;
                        Values.TryGetValue(hand[j].value, out first);
                        Values.TryGetValue(hand[minimum].value, out second);

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
        private static void Swap(List<Card> hand, int index1, int index2)
        {
            Card temp = hand[index1];
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
                List<Card> hand = player.Value.hand;

                foreach (Card card in hand)
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
                List<Card> hand = player.Value.hand;
                Dictionary<string, List<Card>> allHandStrengths = new Dictionary<string, List<Card>>(player.Value.allHandStrengths);
                List<Card> cards = new List<Card>();
                List<Card> tempCard_List = new List<Card>();
                List<Card> tempOnePair_List = new List<Card>();
                List<Card> tempThreeOf_List = new List<Card>();
                List<char> typesOfSuitsinHand = new List<char>();
                
                Card card0, card1, card2, card3, card4;
                // **********************************************************************************
                // **************** high card (requires last card of organized hand) ****************
                // **********************************************************************************
                card0.value = hand[NumberOfCardsPerHand - 1].value;
                card0.suit = hand[NumberOfCardsPerHand - 1].suit;
                cards.Add(card0);

                allHandStrengths.TryGetValue("High Card", out tempCard_List);
                player.Value.bestHandStrength["High Card"] = new List<Card>(cards);
                player.Value.valueOfBestHand = HandStrengths["High Card"];

                tempCard_List.Clear();
                tempCard_List.Add(card0);

                int i = 0;

                // determine number of suits in hand
                foreach (Card card in hand)
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

                    player.Value.bestHandStrength["Full House"] = new List<Card>(cards);
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

                    player.Value.bestHandStrength["Full House"] = new List<Card>(cards);
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

                    player.Value.bestHandStrength[tempName] = new List<Card>(cards);
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

                    player.Value.bestHandStrength["Flush"] = new List<Card>(cards);
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

                        player.Value.bestHandStrength["Four of a Kind"] = new List<Card>(cards);
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

                        player.Value.bestHandStrength["Four of a Kind"] = new List<Card>(cards);
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

                            player.Value.bestHandStrength["Three of a Kind"] = new List<Card>(cards);
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

                            player.Value.bestHandStrength["Three of a Kind"] = new List<Card>(cards);
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

                            player.Value.bestHandStrength["Three of a Kind"] = new List<Card>(cards);
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
                                        tempOnePair_List = new List<Card>(tempCard_List);

                                        player.Value.bestHandStrength["One Pair"] = new List<Card>(cards);
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

                                                player.Value.bestHandStrength["Two Pair"] = new List<Card>(cards);
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

                                                    player.Value.bestHandStrength["Two Pair"] = new List<Card>(cards);
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

            // ********** determine winner **********
            Player winningPlayer = new Player();
            List<Player> players = new List<Player>();
            List<Card> tempCard1 = new List<Card>();
            List<Card> tempCard2 = new List<Card>();
            winningPlayer.name = "error";
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
            if (winningPlayer.name == "error")
            {
                // same type of hand: need to verify highest value card
                if (players[0].valueOfBestHand == players[1].valueOfBestHand)
                {
                    // return the type of best hand each player had
                    typeOfHand = HandStrengths.FirstOrDefault(x => x.Value == players[0].valueOfBestHand).Key;
                    tempCard1 = new List<Card>();
                    tempCard2 = new List<Card>();
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
}