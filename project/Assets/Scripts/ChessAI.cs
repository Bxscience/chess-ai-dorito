using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ChessAI {
    public static int searchDepth = 4; // You can experiment with different depths
    private static ChessMove twoMovesAgo = new ChessMove(0, 0, 0, 0);
    private static ChessMove oneMoveAgo = new ChessMove(0, 0, 0, 0);
    private static Vector2Int[] KingDirections = {new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
        new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1)};

    // Call this method to get the best move for the AI.
    public static ChessMove GetBestMove(ChessPiece[,] board, PieceColor aiColor) {
        int bestScore = int.MinValue;
        ChessMove bestMove = null;
        List<ChessMove> moves = MoveGenerator.GenerateMoves(board, aiColor);

        // Recursively iterate through each move to find the best one
        foreach (ChessMove move in moves) {
            ChessPiece[,] boardCopy = CloneBoard(board);
            MakeMove(boardCopy, move);
            if (!KingsTouching(boardCopy) && !InStalemate(boardCopy, (aiColor == PieceColor.White) ? PieceColor.Black : PieceColor.White)) {
                int score = Minimax(boardCopy, searchDepth - 1, int.MinValue, int.MaxValue, false, aiColor);
                if ((move.startX == twoMovesAgo.startX) && (move.startY == twoMovesAgo.startY) && (move.endX == twoMovesAgo.endX) && (move.endY == twoMovesAgo.endY)) {
                    score -= 75;
                }
                if (score > bestScore || (bestScore == score && score == int.MinValue && !GameManager.instance.InCheck(boardCopy, (aiColor == PieceColor.White) ? PieceColor.Black : PieceColor.White))) {
                    bestScore = score;
                    bestMove = move;
                }
            }
        }
        return bestMove;
    }

    public static void MoveAI(PieceColor aiColor) {
        // Get list of possible moves
        // List<ChessMove> allMoves = MoveGenerator.GenerateMoves(GameManager.instance.enumPieces, PieceColor.Black);

        // Choose a random move and make it
        // System.Random random = new System.Random();
        // ChessMove randomMove = allMoves[random.Next(allMoves.Count)];
        PieceColor playerColor = (aiColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        ChessMove bestMove = GetBestMove(GameManager.instance.enumPieces, aiColor);

        twoMovesAgo = oneMoveAgo;
        oneMoveAgo = bestMove;

        if (GameManager.instance.enumPieces[bestMove.endX, bestMove.endY].color == playerColor) 
            GameManager.instance.CapturePieceAt(new Vector2Int(bestMove.endX, bestMove.endY));
        GameManager.instance.Move(GameManager.instance.pieces[bestMove.startX, bestMove.startY], new Vector2Int(bestMove.endX, bestMove.endY));
        GameManager.instance.NextPlayer();
    }

    static int Minimax(ChessPiece[,] board, int depth, int alpha, int beta, bool maximizingPlayer, PieceColor aiColor) {
        if (depth == 0) {
            return Evaluator.EvaluateBoard(board, aiColor);
        }

        PieceColor currentPlayer = maximizingPlayer ? aiColor : (aiColor == PieceColor.White ? PieceColor.Black : PieceColor.White);
        List<ChessMove> moves = MoveGenerator.GenerateMoves(board, currentPlayer);

        // Minimax and alpha-beta pruning for maximizing score
        if (maximizingPlayer) {
            int maxEval = int.MinValue;

            foreach (ChessMove move in moves) {
                ChessPiece[,] boardCopy = CloneBoard(board);
                MakeMove(boardCopy, move);
                int eval = Minimax(boardCopy, depth - 1, alpha, beta, false, aiColor);
                maxEval = Mathf.Max(maxEval, eval);

                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha)
                    break; // Beta cut-off
            }
            return maxEval;
        }
        else {
            // Minimax and alpha-beta pruning for minimizing score
            int minEval = int.MaxValue;
            foreach (ChessMove move in moves) {
                ChessPiece[,] boardCopy = CloneBoard(board);
                MakeMove(boardCopy, move);
                int eval = Minimax(boardCopy, depth - 1, alpha, beta, true, aiColor);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);

                if (beta <= alpha)
                    break; // Alpha cut-off
            }
            return minEval;
        }
    }

    // Helper method: Create a deep copy of the chess board.
    static ChessPiece[,] CloneBoard(ChessPiece[,] original) {
        ChessPiece[,] copy = new ChessPiece[8, 8];
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                copy[x, y] = original[x, y];
            }
        }
        return copy;
    }

    // Helper method: Execute a move on the board.
    static void MakeMove(ChessPiece[,] board, ChessMove move) {
        ChessPiece movingPiece = board[move.startX, move.startY];
        if (movingPiece.type == PieceType.Pawn && (move.endY == 0 || move.endY == 7))
            board[move.endX, move.endY] = new ChessPiece(PieceType.Queen, movingPiece.color, true);
        else
            board[move.endX, move.endY] = new ChessPiece(movingPiece.type, movingPiece.color, true);
        board[move.startX, move.startY] = new ChessPiece(PieceType.None, PieceColor.None, true);

        if (movingPiece.type == PieceType.King && Mathf.Abs(move.endX - move.startX) == 2) {
            if (move.endX == 6) MakeMove(board, new ChessMove(7, move.endY, 5, move.endY));
            else if (move.endX == 2) MakeMove(board, new ChessMove(0, move.endY, 3, move.endY));
        }
    }

    static bool KingsTouching(ChessPiece[,] board) {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                if (board[x, y].type == PieceType.King) {
                    foreach (Vector2Int dir in KingDirections) {
                        if ((x + dir.x >= 0) && (x + dir.x <= 7) && (y + dir.y >= 0) && (y + dir.y <= 7) && board[x + dir.x, y + dir.y].type == PieceType.King) return true;
                    }
                    return false;
                }
            }
        }
        return false;
    }

    static bool InStalemate(ChessPiece[,] board, PieceColor playerColor) {
        ChessPiece[,] boardCopy = CloneBoard(board);

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                if (board[x, y].color == playerColor && !(board[x, y].type == PieceType.Pawn) && !(board[x, y].type == PieceType.King)) {
                    return false;
                }
            }
        }
        return !GameManager.instance.InCheck(boardCopy, (playerColor == PieceColor.White) ? PieceColor.Black : PieceColor.White) && GameManager.instance.IsCheckmate(boardCopy, playerColor);
    }
}