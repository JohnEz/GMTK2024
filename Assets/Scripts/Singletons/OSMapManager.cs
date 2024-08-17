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
                        -(int)trackPiece.Piece.Rotation
                    ) * pos;

                    // pos += new Vector3(
                    //     trackPiece.Piece.X,
                    //     trackPiece.Piece.Y,
                    //     0
                    // );
                }
                newKnot.Position = new Unity.Mathematics.float3(pos.x, pos.y, pos.z);


                _currentSpline.Spline.Add(newKnot);
            }

            //_currentSpline.Spline.Add(trackPiece.Piece.Template.Spline);

            _tracks.Add(newTrack);
        });
    }
}
