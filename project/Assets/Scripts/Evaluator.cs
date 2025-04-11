using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static int EvaluateBoard(ChessPiece[,] board, PieceColor aiColor) {
        int score = 0;
        int unmovedWhite = 0;
        int unmovedBlack = 0;

        int endgameScoreWhite = 0;
        int endgameScoreBlack = 0;
        bool inEndgame = false;

        Vector2Int whiteKingPos = new Vector2Int(0, 0);
        Vector2Int blackKingPos = new Vector2Int(0, 0);

        // Determining opening and endgame states
        for (int x = 0; x < 8; x++) {
            if (board[x, 0].color == PieceColor.White) unmovedWhite++;
            if (board[x, 1].color == PieceColor.White) unmovedWhite++;
            if (board[x, 6].color == PieceColor.Black) unmovedBlack++;
            if (board[x, 7].color == PieceColor.Black) unmovedBlack++;
            for (int y = 0; y < 8; y++) {
                if (board[x, y].type == PieceType.None) continue;
                if (!(board[x, y].type == PieceType.Pawn) && !(board[x, y].type == PieceType.King)) {
                    if (board[x, y].color == PieceColor.White) endgameScoreWhite += GetPieceValue(board[x, y]);
                    if (board[x, y].color == PieceColor.Black) endgameScoreBlack += GetPieceValue(board[x, y]);
                }
                else if (board[x, y].type == PieceType.Pawn) {
                    if (board[x, y].color == PieceColor.White) endgameScoreWhite += 1;
                    if (board[x, y].color == PieceColor.Black) endgameScoreBlack += 1;
                }
                else if (board[x, y].type == PieceType.King) {
                    if (board[x, y].color == PieceColor.White) whiteKingPos = new Vector2Int(x, y);
                    if (board[x, y].color == PieceColor.Black) blackKingPos = new Vector2Int(x, y);
                }
            }
        }
        inEndgame = (endgameScoreWhite < 1000) && (endgameScoreBlack < 1000);

        if (!board[4, 0].moved) {
            if (board[5, 0].type == PieceType.None && board[6, 0].type == PieceType.None) score += 10;
            else if (board[1, 0].type == PieceType.None && board[2, 0].type == PieceType.None) score += 10;
        }
        else if (!board[4, 7].moved) {
            if (board[5, 7].type == PieceType.None && board[6, 7].type == PieceType.None) score -= 10;
            else if (board[1, 7].type == PieceType.None && board[2, 7].type == PieceType.None) score -= 10;

        }

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                ChessPiece piece = board[x, y];

                // Add to score for white pieces
                if (piece.color == PieceColor.White) {
                    score += GetPieceValue(piece);

                    // Opening Game: Encourage developing pieces in the center of the board
                    if (unmovedWhite >= 10) {
                        if (y > 1 && piece.type != PieceType.Queen) {
                            score += (piece.type == PieceType.Pawn) ? 5 : 0;
                            score += (y < 4) ? 10 : -35;
                            score += (x >= 2 && x <= 5) ? 5 : 0;
                            score += ((y == 3 || y == 4) && (x == 3 || x == 4)) ? 25 : 0;
                        }
                        else if (y > 1 && unmovedWhite >= 12 && piece.type == PieceType.Queen) {
                            score -= 30;
                        }
                    }
                    else if (piece.type == PieceType.Queen && !inEndgame) {
                        List<Vector2Int> moves = PieceData.instance.queen.MoveLocations(new Vector2Int(x, y), board);
                        moves.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
                        score += moves.Count / 5;
                    }
                    else if (piece.type == PieceType.Rook && board[4, 0].moved && !inEndgame) {
                        List<Vector2Int> moves = PieceData.instance.rook.MoveLocations(new Vector2Int(x, y), board);
                        moves.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
                        score += moves.Count / 5;
                    }

                    // Encourage pawn development
                    if (piece.type == PieceType.Pawn) {
                        if (unmovedWhite <= 11) score += 10 * (y - 1);
                        if (y > 2 && unmovedWhite >= 10) score += 15;
                        if (unmovedWhite >= 8 && y > 1 && (x == 2 || x == 5)) score -= 15;
                        else if (unmovedWhite >= 8 && y > 1 && (x < 2 || x > 5)) score -= 25;
                    }

                    if (piece.type == PieceType.Knight) {
                        score += (y > 1 && x >= 2 && x <= 5) ? 10 : -10;
                    }

                    // Ensure king safety
                    if (piece.type == PieceType.King) {
                        if (y == 0 && piece.moved) {
                            if (x == 6 && board[5, 0].type == PieceType.Rook) score += 10;
                            else if (x == 2 && board[3, 0].type == PieceType.Rook) score += 10;
                            
                            if (x < 2 || x > 5) {
                                score += (x > 0 && board[x - 1, y + 1].type != PieceType.None) ? 15 : 0;
                                score += (board[x, y + 1].type != PieceType.None) ? 15 : 0;
                                score += (x < 7 && board[x + 1, y + 1].type != PieceType.None) ? 15 : 0;
                            }

                        }
                        if (inEndgame && endgameScoreWhite > 0 && aiColor == PieceColor.White) {
                            score -= 10 * (Mathf.Abs(x - blackKingPos.x) + Mathf.Abs(y - blackKingPos.y));

                            List<Vector2Int> moves = PieceData.instance.king.MoveLocations(blackKingPos, board);
                            moves.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
                            moves.RemoveAll(gp => MoveGenerator.FriendlyPieceAt(board, gp, PieceColor.Black));
                            moves.RemoveAll(gp => GameManager.instance.InCheck(MoveGenerator.MakeMove(MoveGenerator.CloneBoard(board), new ChessMove(blackKingPos.x, blackKingPos.y, gp.x, gp.y)), PieceColor.Black));
                            score += 15 * (8 - moves.Count);
                        }
                        else if (inEndgame && endgameScoreWhite == 0 && aiColor == PieceColor.White) {
                            score -= 10 * (Mathf.Max(x, 7 - x) + Mathf.Max(y, 7 - y));
                        }
                        else if (inEndgame && endgameScoreBlack > 0 && aiColor == PieceColor.Black) {
                            score -= 15 * (Mathf.Max(x, 7 - x) + Mathf.Max(y, 7 - y));
                        }
                        else {
                            if (piece.moved) score -= 30 * y;
                            else score -= 30 * (y + Mathf.Abs(4 - x));
                        }
                    }
                }
                // Subtract from score for black pieces
                else if (piece.color == PieceColor.Black) {
                    score -= GetPieceValue(piece);
                    if (unmovedBlack >= 10) {
                        if (y < 6 && piece.type != PieceType.Queen) {
                            score -= (piece.type == PieceType.Pawn) ? 5 : 0;
                            score -= (y > 3) ? 10 : -35;
                            score -= (x >= 2 && x <= 5) ? 5 : 0;
                            score -= ((y == 3 || y == 4) && (x == 3 || x == 4)) ? 25 : 0;
                        }
                        else if (y < 6 && unmovedBlack >= 12 && piece.type == PieceType.Queen) {
                            score += 30;
                        }
                    }
                    else if (piece.type == PieceType.Queen && !inEndgame) {
                        List<Vector2Int> moves = PieceData.instance.queen.MoveLocations(new Vector2Int(x, y), board);
                        moves.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
                        score -= moves.Count / 5;
                    }
                    else if (piece.type == PieceType.Rook && !board[4, 7].moved && !inEndgame) {
                        List<Vector2Int> moves = PieceData.instance.rook.MoveLocations(new Vector2Int(x, y), board);
                        moves.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
                        score -= moves.Count / 5;
                    }
                    if (piece.type == PieceType.Pawn) {
                        if (unmovedBlack <= 11) score -= 10 * (6 - y);
                        if (y < 5 && unmovedBlack >= 10) score -= 15;
                        if (unmovedBlack >= 8 && y < 6 && (x == 2 || x == 5)) score += 15;
                        else if (unmovedBlack >= 8 && y < 6 && (x < 2 || x > 5)) score += 25;
                    }

                    if (piece.type == PieceType.Knight) {
                        score -= (y < 6 && x >= 2 && x <= 5) ? 10 : -10;
                    }

                    if (piece.type == PieceType.King) {
                        if (y == 7 && piece.moved) {
                            if (x == 6 && board[5, 7].type == PieceType.Rook) score -= 10;
                            else if (x == 2 && board[3, 7].type == PieceType.Rook) score -= 10;

                            if (x < 2 || x > 5) {
                                score -= (x > 0 && board[x - 1, y - 1].type != PieceType.None) ? 15 : 0;
                                score -= (board[x, y - 1].type != PieceType.None) ? 15 : 0;
                                score -= (x < 7 && board[x + 1, y - 1].type != PieceType.None) ? 15 : 0;
                            }  
                        }
                        if (inEndgame && endgameScoreBlack > 0 && aiColor == PieceColor.Black) {
                            score += 10 * (Mathf.Abs(x - whiteKingPos.x) + Mathf.Abs(y - whiteKingPos.y));

                            List<Vector2Int> moves = PieceData.instance.king.MoveLocations(whiteKingPos, board);
                            moves.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);
                            moves.RemoveAll(gp => MoveGenerator.FriendlyPieceAt(board, gp, PieceColor.White));
                            moves.RemoveAll(gp => GameManager.instance.InCheck(MoveGenerator.MakeMove(MoveGenerator.CloneBoard(board), new ChessMove(whiteKingPos.x, whiteKingPos.y, gp.x, gp.y)), PieceColor.White));
                            score -= 15 * (8 - moves.Count);
                        }
                        else if (inEndgame && endgameScoreBlack == 0 && aiColor == PieceColor.Black) {
                            score += 10 * (Mathf.Max(x, 7 - x) + Mathf.Max(y, 7 - y));
                        }
                        else if (inEndgame && endgameScoreWhite > 0 && aiColor == PieceColor.White) {
                            score += 15 * (Mathf.Max(x, 7 - x) + Mathf.Max(y, 7 - y));
                        }
                        else {
                            if (piece.moved) score += 30 * (7 - y);
                            else score += 30 * ((7 - y) + Mathf.Abs(4 - x));
                        }
                    }
                }
            }
        }

        // Flip score if AI plays black
        return (score) * (aiColor == PieceColor.White ? 1 : -1);
    }
}
