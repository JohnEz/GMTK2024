using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

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

    public Spline RouteSpline { get; private set; } = new Spline();

    // TODO: happiness, etc

    private float TotalHappiness() {
        // TODO
        return 0.0f;
    }

    public void AddConnection(TrackPiece trackPiece, Compass direction) {
        TrackPieces.Add(new Connection() {
            Piece = trackPiece,
            PreviousPieceDirection = direction.Reversed()
        });
    }

    public void CalculateSpline() {
        RouteSpline = new Spline();

        TrackPieces.ForEach(connection => {
            List<BezierKnot> knots = new();

            foreach (BezierKnot knot in connection.Piece.Template.Spline) {
                BezierKnot newKnot = knot; // ok: knot is a struct

                Vector3 pos = new Vector3(
                    newKnot.Position[0],
                    newKnot.Position[1],
                    newKnot.Position[2]
                );

                {
                    pos = Quaternion.Euler(
                        0,
                        0,
                        -(int)connection.Piece.Rotation
                    ) * pos;

                    pos = new Vector3(
                        pos[0] + connection.Piece.X,
                        pos[1] + connection.Piece.Y,
                        0
                    );
                }
                newKnot.Position = new Unity.Mathematics.float3(pos.x, pos.y, pos.z);

                knots.Add(newKnot);
            }

            var tileStartPos = Compass.South; // hardcoded fact about tiles (for now)

            var neighbourDirection = connection.PreviousPieceDirection;
            var newStartDirection = tileStartPos.Rotate(connection.Piece.Rotation);

            if (neighbourDirection != newStartDirection) {
                // flip / reverse newTrack
                knots.Reverse();
            }

            foreach (var knot in knots) {
                RouteSpline.Add(knot);
            }
        });
    }
}