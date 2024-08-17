using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Connection {

    [SerializeField]
    public TrackPiece Piece;

    public Compass PreviousPieceDirection;
}

[System.Serializable]
public class Route {

    [SerializeField]
    public List<Connection> TrackPieces = new List<Connection>();

    // TODO: happiness, etc

    private void Start() {
    }

    private void Update() {
    }

    private float TotalHappiness() {
        // TODO
        return 0.0f;
    }

    public void AddConnection(
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
