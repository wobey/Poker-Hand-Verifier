﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Display : Dealer
{
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
    }

    // display invalid character
    public static void Display_Invalid_Character(char charInput)
    {
        Console.WriteLine("Invalid character detected: {0}", charInput);
    }
}