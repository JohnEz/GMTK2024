using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSMapManager : Singleton<OSMapManager> {

    [SerializeField]
    private OSTrackController _trackPrefab;

    private List<OSTrackController> _tracks = new List<OSTrackController>();

    // Start is called before the first frame update
    private void Start() {
        SpawnRoutes(RouteManager.Instance.Routes);
    }

    private void SpawnRoutes(List<Route> routes) {
        routes.ForEach(SpawnRoute);
    }

    private void SpawnRoute(Route route) {
        route.TrackPieces.ForEach(trackPiece => {
            OSTrackController newTrack = Instantiate(_trackPrefab);
            newTrack.SetTrackPiece(trackPiece.Piece);

            _tracks.Add(newTrack);
        });
    }
}