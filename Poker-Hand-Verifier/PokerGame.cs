// *****************************************************************
// Title: PokerHandVerifier.cs
// Author: John Fitzgerald
// Date: 1-29-17
// Description: This Poker game sim displays the winning hand 
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

public class PokerGame
{
    public static string PlayerName, DefaultName, ErrorHand;
    public static int NumberOfPlayers, NumberOfCardsPerHand;
    public static bool IsPlay, IsManual, IsRandom;

    public static Deck CardDeck;
    public static Dictionary<string, Player> Players;
    public static Dealer Dealer_Single;

    /// <summary>
    /// driver logic of program
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Display.Display_Title();
        Initialize_Game();

        while (IsPlay)
        {
            Display.Display_Start_Menu();
            Validation.Validate_Input_Decision(Validation.Validate_Input_Menu(Console.ReadKey(), "Start"));

            if (IsPlay)
            {
                Create_Players();
                if (IsManual)
                    Dealer_Single.Manually_Enter_Cards();
                else
                    Dealer_Single.Randomly_Generate_Cards();

                Dealer_Single.Rank_Players_Hands();
                Display.Display_Game_Results(Dealer_Single.Get_Winner());
            }
        }
    }

    /// <summary>
    /// initializes deck and players (aka the hands)
    /// </summary>
    public static void Initialize_Game()
    {
        PlayerName = "Hand ";
        DefaultName = "error";

        NumberOfPlayers = 2;
        NumberOfCardsPerHand = 5;

        IsPlay = true;
        IsManual = false;
        IsRandom = false;

        CardDeck = new Deck();
        Players = new Dictionary<string, Player>();
        Dealer_Single = Dealer.Instance;
    }

    /// <summary>
    /// initializes the players (aka the hands)
    /// </summary>
    public static void Create_Players()
    {
        Players.Clear();

        foreach (var player in Player.Populate_Players(NumberOfPlayers))
            Players.Add(player.Name, player);
    }
}