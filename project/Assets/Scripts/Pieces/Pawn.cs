using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint, ChessPiece[,] board)
    {
        List<Vector2Int> locations = new List<Vector2Int>();

        int forwardDirection = board[gridPoint.x, gridPoint.y].color == PieceColor.White ? 1 : -1;
        Vector2Int forwardOne = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection);
        if (!PieceAt(forwardOne, board)) {
            locations.Add(forwardOne);
        }

        Vector2Int forwardTwo = new Vector2Int(gridPoint.x, gridPoint.y + 2 * forwardDirection);
        bool whiteMoved = (gridPoint.y == 1) && (forwardDirection == 1);
        bool blackMoved = (gridPoint.y == 6) && (forwardDirection == -1);
        if ((whiteMoved || blackMoved) && !PieceAt(forwardTwo, board) && !PieceAt(forwardOne, board)) {
            locations.Add(forwardTwo);
        }

        Vector2Int forwardRight = new Vector2Int(gridPoint.x + 1, gridPoint.y + forwardDirection);
        if (PieceAt(forwardRight, board) || (!board[gridPoint.x + 1, gridPoint.y].moved && ((gridPoint.y == 3 && forwardDirection == -1) || (gridPoint.y == 4 && forwardDirection == 1)))) {
            locations.Add(forwardRight);
        }

        Vector2Int forwardLeft = new Vector2Int(gridPoint.x - 1, gridPoint.y + forwardDirection);
        if (PieceAt(forwardLeft, board) || (!board[gridPoint.x - 1, gridPoint.y].moved && ((gridPoint.y == 3 && forwardDirection == -1) || (gridPoint.y == 4 && forwardDirection == 1)))) {
            locations.Add(forwardLeft);
        }

        return locations;
    }
}
