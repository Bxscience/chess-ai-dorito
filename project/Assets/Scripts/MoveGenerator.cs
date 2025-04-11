using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMove {
    public int startX, startY;
    public int endX, endY;

    public ChessMove(int startX, int startY, int endX, int endY) {
        this.startX = startX;
        this.startY = startY;
        this.endX = endX;
        this.endY = endY;
    }
}

public static class MoveGenerator {

    public static List<ChessMove> GenerateMoves(ChessPiece[,] pieces, PieceColor pieceColor) {
        List<ChessMove> moves = new List<ChessMove>();

        // Iterate through each cell on the board
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                if (pieces[x, y].color != pieceColor) continue;
                List<Vector2Int> pieceMoves = new List<Vector2Int>();

                // Call function to generate move for correct piece
                switch (pieces[x, y].type) {
                    case PieceType.Pawn:
                        pieceMoves = PieceData.instance.pawn.MoveLocations(new Vector2Int(x, y), pieces);
                        break;
                    case PieceType.Knight:
                        pieceMoves = PieceData.instance.knight.MoveLocations(new Vector2Int(x, y), pieces);
                        break;
                    case PieceType.Bishop:
                        pieceMoves = PieceData.instance.bishop.MoveLocations(new Vector2Int(x, y), pieces);
                        break;
                    case PieceType.Rook:
                        pieceMoves = PieceData.instance.rook.MoveLocations(new Vector2Int(x, y), pieces);
                        break;
                    case PieceType.Queen:
                        pieceMoves = PieceData.instance.queen.MoveLocations(new Vector2Int(x, y), pieces);
                        break;
                    case PieceType.King:
                        pieceMoves = PieceData.instance.king.MoveLocations(new Vector2Int(x, y), pieces);
                        break;
                }

                // Filter out invalid tiles
                pieceMoves.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
                pieceMoves.RemoveAll(gp => FriendlyPieceAt(pieces, gp, pieceColor));

                // Add moves to list of possible moves
                foreach (Vector2Int move in pieceMoves) {
                    moves.Add(new ChessMove(x, y, move.x, move.y));
                }
            }
        }

        return moves;
    }

    public static bool FriendlyPieceAt(ChessPiece[,] pieces, Vector2Int gridPoint, PieceColor color) {
        // Identify if a piece can be captured
        ChessPiece piece = pieces[gridPoint.x, gridPoint.y];

        return piece.color == color;
    }

    // Helper method: Create a deep copy of the chess board.
    public static ChessPiece[,] CloneBoard(ChessPiece[,] original) {
        ChessPiece[,] copy = new ChessPiece[8, 8];
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                copy[x, y] = original[x, y];
            }
        }
        return copy;
    }

    // Helper method: Execute a move on the board.
    public static ChessPiece[,] MakeMove(ChessPiece[,] board, ChessMove move) {
        ChessPiece movingPiece = board[move.startX, move.startY];
        if (movingPiece.type == PieceType.Pawn && (move.endY == 0 || move.endY == 7))
            board[move.endX, move.endY] = new ChessPiece(PieceType.Queen, movingPiece.color, true);
        else
            board[move.endX, move.endY] = new ChessPiece(movingPiece.type, movingPiece.color, true);
        board[move.startX, move.startY] = new ChessPiece(PieceType.None, PieceColor.None, false);

        if (movingPiece.type == PieceType.King && Mathf.Abs(move.endX - move.startX) == 2) {
            if (move.endX == 6) board = MakeMove(board, new ChessMove(7, move.endY, 5, move.endY));
            else if (move.endX == 2) board = MakeMove(board, new ChessMove(0, move.endY, 3, move.endY));
        }
        return board;
    }
}