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
        Routes.Add(NewRoute); // TODO: drop this out if we cancel the route
        CreateGhostPiece();
        GhostTrackPiece.SetPosition(direction, EditFromTrackPiece);
    }

    private void CreateGhostPiece() {
        if (GhostTrackPiece) {
            Destroy(GhostTrackPiece);
        }

        GhostTrackPiece = Instantiate(GhostTrackPiecePrefab).GetComponent<GhostTrackPiece>();
        GhostTrackPiece.transform.SetParent(transform, false);
        GhostTrackPiece.OnOk += () => PlacePiece();
    }

    private void StopEditing() {
        EditFromTrackPiece = null;
        Destroy(GhostTrackPiece);
        GhostTrackPiece = null;
        NewRoute = null;
    }

    private void PlacePiece() {
        TrackPiece piece = GhostTrackPiece.Position;
        Compass direction = GhostTrackPiece.Direction;
        NewRoute.AddConnection(piece, direction);

        // TODO: Use templates to determine the piece being placed, so there's actually a `Template` here
        Compass nextDirection = piece.Template.ConnectionPoints.First(
            connection => connection != direction.Reversed()
        );
        GhostTrackPiece.SetPosition(nextDirection, piece);
    }
}