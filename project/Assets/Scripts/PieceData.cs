using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceData : MonoBehaviour {
    // Assign pieces in the inspector
    public GameObject pawnPrefab;
    public GameObject knightPrefab;
    public GameObject bishopPrefab;
    public GameObject rookPrefab;
    public GameObject queenPrefab;
    public GameObject kingPrefab;

    public Piece pawn;
    public Piece knight;
    public Piece bishop;
    public Piece rook;
    public Piece queen;
    public Piece king;

    public static PieceData instance;

    void Awake() {
        instance = this;
    }

    void Start() {
        // Create instances of each piece for move generation
        pawn = Instantiate(pawnPrefab).GetComponent<Piece>();
        knight = Instantiate(knightPrefab).GetComponent<Piece>();
        bishop = Instantiate(bishopPrefab).GetComponent<Piece>();
        rook = Instantiate(rookPrefab).GetComponent<Piece>();
        queen = Instantiate(queenPrefab).GetComponent<Piece>();
        king = Instantiate(kingPrefab).GetComponent<Piece>();
    }
}