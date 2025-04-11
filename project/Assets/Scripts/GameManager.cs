using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Board board;

    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    public GameObject[,] pieces;
    public ChessPiece[,] enumPieces;
    private List<GameObject> movedPawns;

    public GameObject[] whitePieces;
    public GameObject[] blackPieces;

    public Player white;
    public Player black;
    public Player currentPlayer;
    public Player otherPlayer;
    public Player aiPlayer;

    void Awake()
    {
        instance = this;
    }

    void Start() {
        // Initialize pieces and players
        whitePieces = new GameObject[] {whitePawn, whiteKnight, whiteBishop, whiteRook, whiteQueen, whiteKing};
        blackPieces = new GameObject[] {blackPawn, blackKnight, blackBishop, blackRook, blackQueen, blackKing};
        pieces = new GameObject[8, 8];
        enumPieces = new ChessPiece[8, 8];
        movedPawns = new List<GameObject>();

        white = new Player("white", true);
        black = new Player("black", false);
        aiPlayer = white;

        currentPlayer = white;
        otherPlayer = black;

        InitialSetup();
        if (aiPlayer == currentPlayer) ChessAI.MoveAI(aiPlayer == white ? PieceColor.White : PieceColor.Black);
    }

    private void InitialSetup() {
        // Add all initial pieces to the board
        AddPiece(PieceType.Rook, PieceColor.White, 0, 0);
        AddPiece(PieceType.Knight, PieceColor.White, 1, 0);
        AddPiece(PieceType.Bishop, PieceColor.White, 2, 0);
        AddPiece(PieceType.Queen, PieceColor.White, 3, 0);
        AddPiece(PieceType.King, PieceColor.White, 4, 0);
        AddPiece(PieceType.Bishop, PieceColor.White, 5, 0);
        AddPiece(PieceType.Knight, PieceColor.White, 6, 0);
        AddPiece(PieceType.Rook, PieceColor.White, 7, 0);

        for (int i = 0; i < 8; i++) {
            AddPiece(PieceType.Pawn, PieceColor.White, i, 1);
        }

        AddPiece(PieceType.Rook, PieceColor.Black, 0, 7);
        AddPiece(PieceType.Knight, PieceColor.Black, 1, 7);
        AddPiece(PieceType.Bishop, PieceColor.Black, 2, 7);
        AddPiece(PieceType.Queen, PieceColor.Black, 3, 7);
        AddPiece(PieceType.King, PieceColor.Black, 4, 7);
        AddPiece(PieceType.Bishop, PieceColor.Black, 5, 7);
        AddPiece(PieceType.Knight, PieceColor.Black, 6, 7);
        AddPiece(PieceType.Rook, PieceColor.Black, 7, 7);


        for (int i = 0; i < 8; i++) {
            AddPiece(PieceType.Pawn, PieceColor.Black, i, 6);
        }
    }

    public void AddPiece(PieceType pieceType, PieceColor pieceColor, int col, int row) {
        // Add piece to the board
        GameObject prefab;

        if (pieceColor == PieceColor.White)
            prefab = whitePieces[(int)pieceType - 1];
        else
            prefab = blackPieces[(int)pieceType - 1];
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        if (pieceColor == PieceColor.White)
            white.pieces.Add(pieceObject);
        else
            black.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
        enumPieces[col, row] = new ChessPiece(pieceType, pieceColor, false);
    }

    public void SelectPieceAtGrid(Vector2Int gridPoint) {
        // Select piece at a given cell on the grid
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
        {
            board.SelectPiece(selectedPiece);
        }
    }

    public List<Vector2Int> MovesForPiece(GameObject pieceObject) {
        // Get all moves a given piece can go to
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        List<Vector2Int> locations = piece.MoveLocations(gridPoint, enumPieces);

        // filter out offboard locations
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);

        // filter out locations with friendly piece
        locations.RemoveAll(gp => FriendlyPieceAt(gp));

        // filter out moves putting player in check
        locations.RemoveAll(gp => InCheck(MoveGenerator.MakeMove(MoveGenerator.CloneBoard(enumPieces), new ChessMove(gridPoint.x, gridPoint.y, gp.x, gp.y)), otherPlayer.color));
        return locations;
    }

    public void Move(GameObject piece, Vector2Int gridPoint) {
        // Move a piece on the board
        Piece pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent.type == PieceKind.Pawn && !HasPawnMoved(piece))
        {
            movedPawns.Add(piece);
        }

        Vector2Int startGridPoint = GridForPiece(piece);

        // En passant capturing
        if (pieceComponent.type == PieceKind.Pawn && (gridPoint.x - startGridPoint.x == -1) && !enumPieces[startGridPoint.x - 1, startGridPoint.y].moved && (gridPoint.y == 2 || gridPoint.y == 5))
            CapturePieceAt(new Vector2Int(startGridPoint.x - 1, startGridPoint.y));
        else if (pieceComponent.type == PieceKind.Pawn && (gridPoint.x - startGridPoint.x == 1) && !enumPieces[startGridPoint.x + 1, startGridPoint.y].moved && (gridPoint.y == 2 || gridPoint.y == 5))
            CapturePieceAt(new Vector2Int(startGridPoint.x + 1, startGridPoint.y));

        for (int x = 0; x < 8; x++) {
            enumPieces[x, 3] = new ChessPiece(enumPieces[x, 3].type, enumPieces[x, 3].color, true);
            enumPieces[x, 4] = new ChessPiece(enumPieces[x, 4].type, enumPieces[x, 4].color, true);
        }
        
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;

        enumPieces[gridPoint.x, gridPoint.y] = enumPieces[startGridPoint.x, startGridPoint.y];
        enumPieces[startGridPoint.x, startGridPoint.y] = new ChessPiece(PieceType.None, PieceColor.None, false);
        board.MovePiece(piece, gridPoint);

        // Castling
        if (pieceComponent.type == PieceKind.King && Mathf.Abs(gridPoint.x - startGridPoint.x) == 2) {
            if (gridPoint.x == 6) Move(PieceAtGrid(new Vector2Int(7, gridPoint.y)), new Vector2Int(5, gridPoint.y));
            else if (gridPoint.x == 2) Move(PieceAtGrid(new Vector2Int(0, gridPoint.y)), new Vector2Int(3, gridPoint.y));
        }
        // Pawn Promotion
        else if (pieceComponent.type == PieceKind.Pawn && (gridPoint.y == 0 || gridPoint.y == 7)) {
            CapturePieceAt(gridPoint);
            AddPiece(PieceType.Queen, gridPoint.y == 7 ? PieceColor.White : PieceColor.Black, gridPoint.x, gridPoint.y);
        }

        // Store pawns that move two spaces for en passant
        if (pieceComponent.type == PieceKind.Pawn && !enumPieces[gridPoint.x, gridPoint.y].moved && (gridPoint.y == 3 || gridPoint.y == 4))
            enumPieces[gridPoint.x, gridPoint.y].moved = false;
        else
            enumPieces[gridPoint.x, gridPoint.y].moved = true;
    }

    // public void Move(ChessPiece[,] pieces, ChessMove move) {
    //     piece = pieces[move.startX, move.startY];
    //     Piece pieceComponent = piece.GetComponent<Piece>();
    //     if (pieceComponent.type == PieceKind.Pawn && !HasPawnMoved(piece))
    //     {
    //         movedPawns.Add(piece);
    //     }

    //     Vector2Int startGridPoint = GridForPiece(piece);
    //     pieces[startGridPoint.x, startGridPoint.y] = null;
    //     pieces[gridPoint.x, gridPoint.y] = piece;

    //     enumPieces[gridPoint.x, gridPoint.y] = enumPieces[startGridPoint.x, startGridPoint.y];
    //     enumPieces[startGridPoint.x, startGridPoint.y] = new ChessPiece[PieceType.None, PieceColor.None];
    //     board.MovePiece(piece, gridPoint);
    // }

    public void PawnMoved(GameObject pawn) {
        movedPawns.Add(pawn);
    }

    public bool HasPawnMoved(GameObject pawn) {
        return movedPawns.Contains(pawn);
    }

    public void CapturePieceAt(Vector2Int gridPoint) {
        // Capture piece on the board
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        currentPlayer.capturedPieces.Add(pieceToCapture);
        pieces[gridPoint.x, gridPoint.y] = null;
        enumPieces[gridPoint.x, gridPoint.y] = new ChessPiece(PieceType.None, PieceColor.None, false);
        Destroy(pieceToCapture);
    }

    public bool InCheck(ChessPiece[,] board, PieceColor color) {
        List<ChessMove> moves = MoveGenerator.GenerateMoves(board, color);
        foreach (ChessMove move in moves) {
            if (board[move.endX, move.endY].type == PieceType.King && board[move.endX, move.endY].color != color) {
                return true;
            }
        }
        return false;
    }

    public bool IsCheckmate(ChessPiece[,] board, PieceColor color) {
        List<ChessMove> moves = MoveGenerator.GenerateMoves(board, color);
        foreach (ChessMove move in moves) {
            ChessPiece[,] copy = MoveGenerator.CloneBoard(board);
            copy = MoveGenerator.MakeMove(copy, move);
            if (!InCheck(copy, (color == PieceColor.White) ? PieceColor.Black : PieceColor.White)) {
                return false;
            }
        }
        return true;
    }

    public void SelectPiece(GameObject piece) {
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece) {
        board.DeselectPiece(piece);
    }

    public bool PiecePossession(GameObject piece) {
        return currentPlayer.pieces.Contains(piece);
    }

    public GameObject PieceAtGrid(Vector2Int gridPoint) {
        // Get piece on a given point on the board
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0) {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }

    public Vector2Int GridForPiece(GameObject piece) {
        // Find location of a given piece on the board
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (pieces[i, j] == piece) {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool FriendlyPieceAt(Vector2Int gridPoint) {
        // Identify if a piece can be captured
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null) {
            return false;
        }

        if (otherPlayer.pieces.Contains(piece)) {
            return false;
        }

        return true;
    }

    public void NextPlayer() {
        // Switch active player
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
        if (IsCheckmate(enumPieces, currentPlayer.color) && InCheck(enumPieces, otherPlayer.color)) {
            Debug.Log("checkmate");
            Destroy(board.GetComponent<TileSelector>());
            Destroy(board.GetComponent<MoveSelector>());
        }
        else if (IsCheckmate(enumPieces, currentPlayer.color)) {
            Debug.Log("stalemate");
            Destroy(board.GetComponent<TileSelector>());
            Destroy(board.GetComponent<MoveSelector>());
        }
        else if (InCheck(enumPieces, otherPlayer.color)) {
            Debug.Log("check");
            if (aiPlayer == currentPlayer) ChessAI.MoveAI(aiPlayer == white ? PieceColor.White : PieceColor.Black);
        }
        else if (aiPlayer == currentPlayer) ChessAI.MoveAI(aiPlayer == white ? PieceColor.White : PieceColor.Black);
    }
}
