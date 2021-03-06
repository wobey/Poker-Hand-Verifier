﻿// *****************************************************************
// Title: Validation.cs
// Author: John Fitzgerald
// Date: 1-29-17
// Description: Validation class determine if user input is valid,
// and if the cards in the Players' hands are valid.
// *****************************************************************
using System;
using System.Linq;

class Validation : PokerGame
{
    public static char[] MenuOptionsStart = { 'r', 'm', 'q' };
    public static char[] MenuOptionsEnd = { 'p', 'q' };

    /// <summary>
    /// validate user input for menus
    /// </summary>
    /// <param name="key"></param>
    /// <param name="menuType"></param>
    /// <returns></returns>
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
        while (!menuOptions.Contains(Char.ToLower(charInput)))
        {
            Console.WriteLine("\n{0} is not valid. Please enter a valid character: ", key.KeyChar);
            charInput = Console.ReadKey().KeyChar;
        }

        if (Char.ToLower(charInput) == 'm')
            IsManual = true;

        return charInput;
    }

    /// <summary>
    /// validate what to do at start of program based on user input
    /// </summary>
    /// <param name="charInput"></param>
    public static void Validate_Input_Decision(char charInput)
    {
        char lowerCharInput = Char.ToLower(charInput);

        // randomly generate cards
        if (lowerCharInput == 'r')
        {
            IsManual = false;
            IsRandom = true;
        }
        // manually enter cards
        else if (lowerCharInput == 'm')
        {
            IsRandom = false;
            IsManual = true;
        }
        // play again
        else if (lowerCharInput == 'p')
            Display.Display_Play_Again();
        // quit program
        else if (lowerCharInput == 'q')
        {
            Display.Display_Quit_Program();
            IsPlay = false;
        }
        // should never reach here (if using input validation)
        else
            Display.Display_Invalid_Character(charInput);
    }

    /// <summary>
    /// validate the cards in Players' hands
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
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
            int handNum = i + 1;
            Players.TryGetValue(PlayerName + handNum, out Player player);

            // check contents of hand
            for (int j = 0; j < NumberOfCardsPerHand * 2; ++j)
            {
                Deck.Card checkCard = new Deck.Card(hands[i][j], hands[i][++j]);
                //checkCard.value = hands[i][j];
                //checkCard.suit = hands[i][++j];
                if (!CardDeck.FullDeck.Contains(checkCard))
                {
                    // clear each player's hand (in-case user changes previous players' cards)
                    foreach (var _player in Players)
                        _player.Value.Hand.Clear();

                    ErrorHand = "Invalid characters for card detected: " + checkCard.value + checkCard.suit;
                    return false;
                }

                // add card to player's hand
                player.Hand.Add(checkCard);
            }
        }

        // TODO: check for duplicates

        return true;
    }
}