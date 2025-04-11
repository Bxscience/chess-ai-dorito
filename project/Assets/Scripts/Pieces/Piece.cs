using System.Collections.Generic;
using UnityEngine;

public enum PieceKind {King, Queen, Bishop, Knight, Rook, Pawn};

public abstract class Piece : MonoBehaviour
{
    public PieceKind type;

    protected Vector2Int[] RookDirections = {new Vector2Int(0,1), new Vector2Int(1, 0),
        new Vector2Int(0, -1), new Vector2Int(-1, 0)};
    protected Vector2Int[] BishopDirections = {new Vector2Int(1,1), new Vector2Int(1, -1),
        new Vector2Int(-1, -1), new Vector2Int(-1, 1)};

    protected bool PieceAt(Vector2Int gridPos, ChessPiece[,] board) {
        if (gridPos.x > 7 || gridPos.y > 7 || gridPos.x < 0 || gridPos.y < 0) {
            return true;
        }
        return board[gridPos.x, gridPos.y].type != PieceType.None;
    }

    protected bool InCheck(ChessPiece[,] board, PieceColor currentPlayer) {
        PieceColor color = (currentPlayer == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        ChessPiece[,] copy = MoveGenerator.CloneBoard(board);
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (copy[x, y].color == color && copy[x, y].type == PieceType.King)
                    copy[x, y] = new ChessPiece(PieceType.King, PieceColor.None, true);
        List<ChessMove> moves = MoveGenerator.GenerateMoves(copy, color);
        foreach (ChessMove move in moves) {
            if (board[move.endX, move.endY].type == PieceType.King && board[move.endX, move.endY].color != color) {
                return true;
            }
        }
        return false;
    }

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint, ChessPiece[,] board);
}
