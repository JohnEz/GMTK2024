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

    public List<OSTrackController> Tracks { get; private set; }

    private void Awake() {
        Tracks = new List<OSTrackController>();
    }

    private void Start() {
        SpawnRoutes(RouteManager.Instance.Routes);
    }

    private void SpawnRoutes(List<Route> routes) {
        routes.ForEach(SpawnRoute);
    }

    private void SpawnRoute(Route route) {
        OSRouteController newRouteObject = Instantiate(_routePrefab);
        newRouteObject.Route = route;
        newRouteObject.transform.SetParent(transform, false);

        route.TrackPieces.ForEach(connection => {
            OSTrackController newTrack = Instantiate(_trackPrefab);
            newTrack.SetTrackPiece(connection.Piece);

            Tracks.Add(newTrack);
            newTrack.transform.parent = newRouteObject.transform;
        });

        Color lineColor = LineManager.Instance.GetLineByRoute(newRouteObject);

        newRouteObject.UpdateRouteColor(lineColor);

        newRouteObject.AddTrain();
    }
}