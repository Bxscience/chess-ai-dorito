﻿using System.Collections.Generic;
using UnityEngine;

// Defines a player and its pieces and captired pieces
public class Player
{
    public List<GameObject> pieces;
    public List<GameObject> capturedPieces;

    public string name;
    public int forward;
    public PieceColor color;

    public Player(string name, bool positiveZMovement)
    {
        this.name = name;
        pieces = new List<GameObject>();
        capturedPieces = new List<GameObject>();

        if (positiveZMovement == true)
        {
            this.forward = 1;
            color = PieceColor.White;
        }
        else
        {
            this.forward = -1;
            color = PieceColor.Black;
        }
    }
}