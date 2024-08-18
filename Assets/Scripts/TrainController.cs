using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainController : MonoBehaviour {
    public GameObject trainPrefab;

    private List<SplineAnimate> _trains = new();

    private SplineContainer _trackSpline = null;

    public void Spawn(Route route) {
        _trackSpline = gameObject.AddComponent<SplineContainer>();

        route.TrackPieces.ForEach(connection => {
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
                _trackSpline.Spline.Add(knot);
            }
        });
    }

    public void AddTrain(Transform parent) {
        GameObject train = Instantiate(trainPrefab);
        train.transform.SetParent(parent, false);
        SplineAnimate splineAnimate = train.GetComponent<SplineAnimate>();
        splineAnimate.Container = _trackSpline;
        splineAnimate.Restart(true);
        _trains.Add(splineAnimate);
    }
}