using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Connection {
    public TrackPiece Piece;
    public Compass PreviousPieceDirection;
}

public class Route : MonoBehaviour
{
    [SerializeField]
    public List<Connection> TrackPieces = new List<Connection>();

    // TODO: happiness, etc

    void Start()
    {
    }

    void Update()
    {

    }

    float TotalHappiness() {
        // TODO
        return 0.0f;
    }

    void AddConnection(
        TrackPiece piece,
        Compass previousPieceDirection
    ) {
        Connection conn = new Connection() {
            Piece = piece,
            PreviousPieceDirection = previousPieceDirection,
        };

        TrackPieces.Add(conn);
    }
}
