using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class Connection {

    [SerializeField]
    public TrackPiece Piece;

    public Compass PreviousPieceDirection;

    public static TrackPiece GetNextTrackPiece(TrackPiece fromTrackPiece, Compass direction) {
        return direction switch
        {
            Compass.North => new TrackPiece()
            {
                X = fromTrackPiece.X,
                Y = fromTrackPiece.Y + 1,
                Rotation = Rotation.Deg270,
            },
            Compass.East => new TrackPiece()
            {
                X = fromTrackPiece.X + 1,
                Y = fromTrackPiece.Y,
                Rotation = Rotation.None,
            },
            Compass.South => new TrackPiece()
            {
                X = fromTrackPiece.X,
                Y = fromTrackPiece.Y - 1,
                Rotation = Rotation.Deg90,
            },
            Compass.West => new TrackPiece()
            {
                X = fromTrackPiece.X - 1,
                Y = fromTrackPiece.Y,
                Rotation = Rotation.Deg180,
            },
            _ => null,
        };
    }
}

[System.Serializable]
public class Route {

    [SerializeField]
    public List<Connection> TrackPieces = new List<Connection>();

    public Spline RouteSpline { get; private set; } = new Spline();
    public Spline RouteSplineReversed { get; private set; } = new Spline();

    [SerializeField]
    private TrackPiece _startStation;

    public TrackPiece StartStation { get { return _startStation; } private set { _startStation = value; } }

    [SerializeField]
    private TrackPiece _endStation;

    public TrackPiece EndStation { get { return _endStation; } private set { _endStation = value; } }

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

    private void SetSpline(Spline spline) {
        RouteSpline = spline;
        RouteSplineReversed = new Spline();

        var reversedList = RouteSpline.Knots.Reverse();

        foreach (var knot in reversedList) {
            RouteSplineReversed.Add(knot);
        }
    }

    public void CalculateSpline() {
        Spline newRouteSpline = new Spline();

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
                newRouteSpline.Add(knot);
            }
        });

        SetSpline(newRouteSpline);
    }
}