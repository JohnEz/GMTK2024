using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class GhostTrackPiece : MonoBehaviour {
    [SerializeField]
    private TrackPieceController _trackPieceController;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private TrackPieceType _trackPieceType;

    public TrackPieceType TrackPieceType {
        get => _trackPieceType;
        set {
            _trackPieceType = value;
            OnSetTrackPieceType();
        }
    }

    private Compass _Direction;

    public Compass Direction => _Direction;

    public TrackPiece Position => _trackPieceController.TrackPiece.Copy();

    void Awake() {
        TrackPieceType = TrackPieceType.Straight;
    }

    private void OnSetTrackPieceType() {
        ToyTrackPieceConfig prefab = ToyMapManager.Instance.TrackPieceConfig[_trackPieceType];
        _spriteRenderer.sprite = prefab.sprite;

        Compass[] connections = prefab.template.ConnectionPoints;

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass forwardDirection = connections[1];
        Rotation rotation = forwardDirection.ToRotation();

        // Bit inefficient but let's not worry about it
        _trackPieceController.TrackPiece = new TrackPiece() {
            X = _trackPieceController.TrackPiece?.X ?? 0,
            Y = _trackPieceController.TrackPiece?.Y ?? 0,
            Rotation = _trackPieceController.TrackPiece?.Rotation ?? Rotation.None,
            Template = ToyMapManager.Instance.TrackPieceConfig[_trackPieceType].template
        };
    }

    public void SetPosition(Compass direction, TrackPiece fromTrackPiece) {
        _Direction = direction;

        TrackPiece trackPiece = Connection.GetNextTrackPiece(fromTrackPiece, direction);
        trackPiece.Template = ToyMapManager.Instance.TrackPieceConfig[_trackPieceType].template;
        _trackPieceController.TrackPiece = trackPiece;
    }
}
