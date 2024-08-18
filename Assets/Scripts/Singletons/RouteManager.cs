using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouteManager : Singleton<RouteManager> {

    // this group of variables are all set together during an edit session:
    private TrackPiece EditFromTrackPiece;

    private GhostTrackPiece GhostTrackPiece;
    private Route NewRoute;

    [SerializeField]
    public List<Route> Routes;

    [SerializeField]
    public GameObject GhostTrackPiecePrefab;

    private void Awake() {
        // we need to calculate the route spline for any hardcoded routes
        Routes.ForEach(route => {
            route.CalculateSpline();
        });
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StopEditing();
        }
    }

    public void StartEditing(Compass direction, TrackPiece fromTrackPiece) {
        if (!GameStateManager.Instance.TrySetState(GameState.KidEditing)) {
            return;
        }

        EditFromTrackPiece = fromTrackPiece;
        NewRoute = new Route();
        CreateGhostPiece();
        GhostTrackPiece.SetPosition(direction, EditFromTrackPiece);
    }

    private void CreateGhostPiece() {
        if (GhostTrackPiece) {
            Destroy(GhostTrackPiece.gameObject);
        }

        GhostTrackPiece = Instantiate(GhostTrackPiecePrefab).GetComponent<GhostTrackPiece>();
        GhostTrackPiece.transform.SetParent(transform, false);
        GhostTrackPiece.OnOk += () => PlacePiece();
    }

    private void StopEditing() {
        if (!GameStateManager.Instance.TrySetState(GameState.Kid)) {
            return;
        }

        EditFromTrackPiece = null;
        Destroy(GhostTrackPiece.gameObject);
        GhostTrackPiece = null;
        NewRoute = null;
    }

    private void PlacePiece() {
        TrackPiece piece = GhostTrackPiece.Position;
        Compass direction = GhostTrackPiece.Direction;
        NewRoute.AddConnection(piece, direction);

        TrackPiece connectingStation = StationManager.Instance.GetConnectingStation(piece);
        if (connectingStation != null) {
            Routes.Add(NewRoute);
            NewRoute.CalculateSpline();
            StopEditing();
            Debug.Log("Route made!");
            return;
        }

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass nextDirection = piece.Template.ConnectionPoints[1];
        EditFromTrackPiece = piece;
        GhostTrackPiece.SetPosition(nextDirection, piece);
    }
}