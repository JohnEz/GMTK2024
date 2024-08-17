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
        route.TrackPieces.ForEach(trackPiece => {
            OSTrackController newTrack = Instantiate(_trackPrefab);
            newTrack.SetTrackPiece(trackPiece.Piece);

            foreach (BezierKnot knot in trackPiece.Piece.Template.Spline) {
                BezierKnot newKnot = knot;
                newKnot.Position += new Unity.Mathematics.float3(trackPiece.Piece.X, trackPiece.Piece.Y, 0);
                _currentSpline.Spline.Add(newKnot);
            }

            _tracks.Add(newTrack);
        });
    }
}