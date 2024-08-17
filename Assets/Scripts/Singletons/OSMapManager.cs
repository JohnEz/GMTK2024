using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class OSMapManager : Singleton<OSMapManager> {

    [SerializeField]
    private OSTrackController _trackPrefab;

    private List<OSTrackController> _tracks = new List<OSTrackController>();

    //TEMP
    public SplineContainer _currentSpline = null;

    public SplineAnimate _train;
    //

    // Start is called before the first frame update
    private void Start() {
        SpawnRoutes(RouteManager.Instance.Routes);
        _train.Container = _currentSpline;
        _train.Restart(true);
    }

    private void SpawnRoutes(List<Route> routes) {
        routes.ForEach(SpawnRoute);
    }

    private void SpawnRoute(Route route) {
        _currentSpline = gameObject.AddComponent<SplineContainer>();

        route.TrackPieces.ForEach(connection => {
            OSTrackController newTrack = Instantiate(_trackPrefab);
            newTrack.SetTrackPiece(connection.Piece);

            List<BezierKnot> knots = new List<BezierKnot>();

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

            foreach(var knot in knots)
                _currentSpline.Spline.Add(knot);

            _tracks.Add(newTrack);
        });
    }
}
