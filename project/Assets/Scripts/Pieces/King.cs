using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint, ChessPiece[,] board)
    {
        List<Vector2Int> locations = new List<Vector2Int>();
        List<Vector2Int> directions = new List<Vector2Int>(BishopDirections);
        directions.AddRange(RookDirections);

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nextGridPoint = new Vector2Int(gridPoint.x + dir.x, gridPoint.y + dir.y);
            locations.Add(nextGridPoint);
        }

        PieceColor pieceColor = board[gridPoint.x, gridPoint.y].color;
        if (board[gridPoint.x, gridPoint.y].type == PieceType.King && !board[gridPoint.x, gridPoint.y].moved && !InCheck(board, pieceColor)) {
            if (board[0, gridPoint.y].type == PieceType.Rook && !board[0, gridPoint.y].moved) {
                ChessMove leftOne = new ChessMove(4, gridPoint.y, 3, gridPoint.y);
                ChessPiece[,] copyOne = MoveGenerator.MakeMove(MoveGenerator.CloneBoard(board), leftOne);

                ChessMove leftTwo = new ChessMove(4, gridPoint.y, 2, gridPoint.y);
                ChessPiece[,] copyTwo = MoveGenerator.MakeMove(MoveGenerator.CloneBoard(board), leftTwo);

                if (!PieceAt(new Vector2Int(3, gridPoint.y), board) && !InCheck(copyOne, pieceColor) && !PieceAt(new Vector2Int(2, gridPoint.y), board) && !InCheck(copyTwo, pieceColor) && !PieceAt(new Vector2Int(1, gridPoint.y), board)) {
                    locations.Add(new Vector2Int(2, gridPoint.y));
                }
            }
            if (board[7, gridPoint.y].type == PieceType.Rook && !board[7, gridPoint.y].moved) {
                ChessMove rightOne = new ChessMove(4, gridPoint.y, 5, gridPoint.y);
                ChessPiece[,] copyOne = MoveGenerator.MakeMove(MoveGenerator.CloneBoard(board), rightOne);

                ChessMove rightTwo = new ChessMove(4, gridPoint.y, 6, gridPoint.y);
                ChessPiece[,] copyTwo = MoveGenerator.MakeMove(MoveGenerator.CloneBoard(board), rightTwo);

                if (!PieceAt(new Vector2Int(5, gridPoint.y), board) && !InCheck(copyOne, pieceColor) && !PieceAt(new Vector2Int(6, gridPoint.y), board) && !InCheck(copyTwo, pieceColor)) {
                    locations.Add(new Vector2Int(6, gridPoint.y));
                }
            }
        }

        return locations;
    }
}