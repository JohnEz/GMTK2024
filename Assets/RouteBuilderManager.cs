using System.Collections.Generic;
using UnityEngine;

public class RouteBuilderManager : Singleton<RouteBuilderManager> {
    [SerializeField]
    private TrackPieceController _trackPreviewPrefab;

    private TrackPiece OriginTrackPiece;

    private TrackPiece EditFromTrackPiece;

    private readonly List<(TrackPieceController controller, Compass direction)> PreviewTrackPieces = new();

    [SerializeField]
    private GhostTrackPiece GhostTrackPiece;

    void Awake() {
        GhostTrackPiece.OnConfirm += () => PlacePiece();
        GhostTrackPiece.OnCancel += () => RemovePiece();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StopEditing();
        }
    }

    public void StartEditing(Compass direction, TrackPiece fromTrackPiece) {
        if (!GameStateManager.Instance.TrySetState(GameState.KidEditing)) {
            return;
        }

        OriginTrackPiece = fromTrackPiece;
        EditFromTrackPiece = OriginTrackPiece;
        GhostTrackPiece.gameObject.SetActive(true);
        GhostTrackPiece.SetPosition(direction, EditFromTrackPiece);
    }

    private void StopEditing() {
        if (!GameStateManager.Instance.TrySetState(GameState.Kid)) {
            return;
        }


        PreviewTrackPieces.ForEach(preview => Destroy(preview.controller.gameObject));
        PreviewTrackPieces.Clear();
        EditFromTrackPiece = null;
        OriginTrackPiece = null;
        GhostTrackPiece.gameObject.SetActive(false);
    }

    private void PlacePiece() {
        TrackPiece piece = GhostTrackPiece.Position;
        Compass direction = GhostTrackPiece.Direction;

        TrackPieceController newTrack = Instantiate(_trackPreviewPrefab, transform);
        newTrack.TrackPiece = piece;
        newTrack.GetComponentInChildren<SpriteRenderer>().sprite = ToyMapManager.Instance.TrackPieceConfig[piece.Template.TrackPieceType].sprite;
        PreviewTrackPieces.Add((newTrack, direction));

        TrackPiece connectingStation = StationManager.Instance.GetConnectingStation(piece);
        if (connectingStation != null) {
            CommitRoute();
            StopEditing();
            Debug.Log("Route made!");
            return;
        }

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass nextDirection = piece.Template.ConnectionPoints[1];
        EditFromTrackPiece = piece;
        GhostTrackPiece.SetPosition(nextDirection, piece);
    }

    private void CommitRoute() {
        Route route = new();
        PreviewTrackPieces.ForEach(preview => {
            route.AddConnection(preview.controller.TrackPiece, preview.direction);
        });
        RouteManager.Instance.AddRoute(route);
    }

    private void RemovePiece() {
        if (PreviewTrackPieces.Count == 0) {
            StopEditing();
            Debug.Log("Route cancelled!");
            return;
        }

        Destroy(PreviewTrackPieces[^1].controller.gameObject);
        PreviewTrackPieces.RemoveAt(PreviewTrackPieces.Count - 1);

        EditFromTrackPiece = PreviewTrackPieces.Count == 0 ? OriginTrackPiece : PreviewTrackPieces[^1].controller.TrackPiece;

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass nextDirection = EditFromTrackPiece.Template.ConnectionPoints[1];
        GhostTrackPiece.SetPosition(nextDirection, EditFromTrackPiece);
    }
}