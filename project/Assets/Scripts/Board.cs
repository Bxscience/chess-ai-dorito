using UnityEngine;

public class Board : MonoBehaviour
{
    public Material defaultMaterial;
    public Material selectedMaterial;

    public GameObject AddPiece(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Geometry.GridPoint(col, row);
        GameObject newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint), piece.transform.rotation, gameObject.transform);
        if (piece.gameObject.name.Contains("Bishop"))
            newPiece.transform.position = new Vector3(newPiece.transform.position.x, 16.5f, newPiece.transform.position.z);
        else if (piece.gameObject.name.Contains("King"))
            newPiece.transform.position = new Vector3(newPiece.transform.position.x, 16.3f, newPiece.transform.position.z);
        return newPiece;
    }

    public void RemovePiece(GameObject piece)
    {
        Destroy(piece);
    }

    public void MovePiece(GameObject piece, Vector2Int gridPoint)
    {
        piece.transform.position = Geometry.PointFromGrid(gridPoint);
        if (piece.gameObject.name.Contains("Bishop"))
            piece.transform.position = new Vector3(piece.transform.position.x, 16.5f, piece.transform.position.z);
        else if (piece.gameObject.name.Contains("King"))
            piece.transform.position = new Vector3(piece.transform.position.x, 16.3f, piece.transform.position.z);
    }

    public void SelectPiece(GameObject piece)
    {
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
    }

    public void DeselectPiece(GameObject piece)
    {
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
    }
}
