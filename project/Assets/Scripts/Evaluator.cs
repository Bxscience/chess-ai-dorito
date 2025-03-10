public static class Evaluator {
    // Assign simple values to each piece type
    public static int GetPieceValue(ChessPiece piece) {
        switch (piece.type) {
            case PieceType.Pawn:     return 100;
            case PieceType.Knight:   return 320;
            case PieceType.Bishop:   return 330;
            case PieceType.Rook:     return 500;
            case PieceType.Queen:    return 900;
            case PieceType.King:     return 20000; // Ensures king safety is prioritized

            default:            return 0;
        }
    }

    // Evaluate board from Black's perspective: positive is good for Black, negative is good for White.
    public static int EvaluateBoard(ChessPiece[,] board) {
        int score = 0;
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                ChessPiece piece = board[x, y];
                if (piece.color == PieceColor.Black) {
                    score += GetPieceValue(piece);
                } else if (piece.color == PieceColor.White) {
                    score -= GetPieceValue(piece);
                }
            }
        }
        return score;
    }
}
