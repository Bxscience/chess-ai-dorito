public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }

public enum PieceColor { None, White, Black }

public struct ChessPiece {
    public PieceType type;
    public PieceColor color;
    public bool moved;

    public ChessPiece(PieceType type, PieceColor color, bool moved) {
        this.type = type;
        this.color = color;
        this.moved = moved;
    }
}