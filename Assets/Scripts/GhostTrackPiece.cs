using UnityEngine;

[RequireComponent(typeof (TrackPieceController))]
public class GhostTrackPiece : MonoBehaviour {
    [SerializeField]
    private TrackPieceController _trackPieceController;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Color _validHighlight;

    [SerializeField]
    private Color _invalidHighlight;

    [SerializeField]
    private Collider2D _collider;

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

    private bool _isValidPosition = true;

    public bool IsValidPosition {
        get => _isValidPosition && _nextStepIsValidPosition;
        private set {
            _isValidPosition = value;
            OnValidityChange();
        }
    }

    private bool _nextStepIsValidPosition = true;

    public bool NextStepIsValidPosition {
        set {
            _nextStepIsValidPosition = value;
            OnValidityChange();
        }
    }

    void OnValidityChange() {
        _spriteRenderer.color = IsValidPosition ? _validHighlight : _invalidHighlight;
    }

    void Awake() {
        IsValidPosition = true;
        TrackPieceType = TrackPieceType.Straight;
    }

    private void OnSetTrackPieceType() {
        ToyTrackPieceConfig config = ToyMapManager.Instance.TrackPieceConfig[_trackPieceType];
        _spriteRenderer.sprite = config.sprite;

        // Bit inefficient but let's not worry about it
        _trackPieceController.TrackPiece = new TrackPiece() {
            X = _trackPieceController.TrackPiece?.X ?? 0,
            Y = _trackPieceController.TrackPiece?.Y ?? 0,
            Rotation = _trackPieceController.TrackPiece?.Rotation ?? Rotation.None,
            Template = config.template
        };
    }

    public void SetPosition(Compass direction, TrackPiece fromTrackPiece) {
        _Direction = direction;

        TrackPiece trackPiece = Connection.GetNextTrackPiece(fromTrackPiece, direction);
        trackPiece.Template = ToyMapManager.Instance.TrackPieceConfig[_trackPieceType].template;
        _trackPieceController.TrackPiece = trackPiece;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IsValidPosition = false;
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        IsValidPosition = true;
    }
}
