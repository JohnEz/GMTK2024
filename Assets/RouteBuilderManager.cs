using System.Collections.Generic;
using UnityEngine;

public class RouteBuilderManager : Singleton<RouteBuilderManager> {
    [SerializeField]
    private TrackPieceController _trackPreviewPrefab;

    private TrackPiece EditFromTrackPiece;

    private Route NewRoute;

    private readonly List<TrackPieceController> PreviewTrackPieces = new();

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


        PreviewTrackPieces.ForEach(previewTrack => Destroy(previewTrack.gameObject));
        PreviewTrackPieces.Clear();
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

        TrackPieceController newTrack = Instantiate(_trackPreviewPrefab, transform);
        newTrack.TrackPiece = piece;
        newTrack.GetComponentInChildren<SpriteRenderer>().sprite = ToyMapManager.Instance.TrackPieceConfig[piece.Template.TrackPieceType].sprite;
        PreviewTrackPieces.Add(newTrack);

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass nextDirection = piece.Template.ConnectionPoints[1];
        EditFromTrackPiece = piece;
        GhostTrackPiece.SetPosition(nextDirection, piece);
    }
}