using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    void Update() {
        if (GameManager.instance.currentPlayer.name == "black") {
            // Get list of possible moves
            List<ChessMove> allMoves = MoveGenerator.GenerateMoves(GameManager.instance.enumPieces, PieceColor.Black);

            // Choose a random move and make it
            System.Random random = new System.Random();
            ChessMove randomMove = allMoves[random.Next(allMoves.Count)];
            GameManager.instance.Move(GameManager.instance.pieces[randomMove.startX, randomMove.startY], new Vector2Int(randomMove.endX, randomMove.endY));
            GameManager.instance.NextPlayer();
        }
    }
}
