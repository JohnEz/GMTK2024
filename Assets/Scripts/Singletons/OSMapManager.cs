using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class OSMapManager : Singleton<OSMapManager> {

    [SerializeField]
    private OSTrackController _trackPrefab;

    [SerializeField]
    private OSRouteController _routePrefab;

    //public List<List<OSTrackController>> Routes { get; private set; }

    public List<OSTrackController> Tracks { get; private set; }

    //TEMP
    public SplineContainer _currentSpline = null;

    public SplineAnimate _train;
    //

    private void Awake() {
        //Routes = new List<List<OSTrackController>>();
        Tracks = new List<OSTrackController>();
    }

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
        //List<OSTrackController> newRoute = new List<OSTrackController>();

        OSRouteController newRouteObject = Instantiate(_routePrefab);
        newRouteObject.Route = route;

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

            foreach (var knot in knots) {
                _currentSpline.Spline.Add(knot);
            }

            Tracks.Add(newTrack);
            newTrack.transform.parent = newRouteObject.transform;
            //newRoute.Add(newTrack);
        });

        //Routes.Add(newRoute);
    }

    public void UpdateRouteColor(List<OSTrackController> route, Color color) {
        route.ForEach(trackController => {
            trackController.UpdateTrackColor(color);
        });
    }
}