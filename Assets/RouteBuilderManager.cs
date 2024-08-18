using UnityEngine;

public class RouteBuilderManager : Singleton<RouteBuilderManager> {
    private TrackPiece EditFromTrackPiece;

    private Route NewRoute;

    private GhostTrackPiece GhostTrackPiece;

    [SerializeField]
    public GameObject GhostTrackPiecePrefab;

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
            RouteManager.Instance.AddRoute(NewRoute);
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