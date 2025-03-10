using UnityEngine;
using System.Collections.Generic;

public class ChessAI : MonoBehaviour {
    public int searchDepth = 3; // You can experiment with different depths

    // Call this method to get the best move for the AI.
    public ChessMove GetBestMove(ChessPiece[,] board, PieceColor aiColor) {
        int bestScore = int.MinValue;
        ChessMove bestMove = null;
        List<ChessMove> moves = MoveGenerator.GenerateMoves(board, aiColor);

        // Recursively iterate through each move to find the best one
        foreach (ChessMove move in moves) {
            ChessPiece[,] boardCopy = CloneBoard(board);
            MakeMove(boardCopy, move);
            int score = Minimax(boardCopy, searchDepth - 1, int.MinValue, int.MaxValue, false, aiColor);
            if (score > bestScore) {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }

    int Minimax(ChessPiece[,] board, int depth, int alpha, int beta, bool maximizingPlayer, PieceColor aiColor) {
        if (depth == 0) {
            return Evaluator.EvaluateBoard(board);
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
    ChessPiece[,] CloneBoard(ChessPiece[,] original) {
        ChessPiece[,] copy = new ChessPiece[8, 8];
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                copy[x, y] = original[x, y];
            }
        }
        return copy;
    }

    // Helper method: Execute a move on the board.
    void MakeMove(ChessPiece[,] board, ChessMove move) {
        ChessPiece movingPiece = board[move.startX, move.startY];
        board[move.endX, move.endY] = movingPiece;
        board[move.startX, move.startY] = new ChessPiece(PieceType.None, PieceColor.None);
    }
}