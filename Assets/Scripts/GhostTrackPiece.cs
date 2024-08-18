using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof (TrackPieceController))]
public class GhostTrackPiece : MonoBehaviour {
    [SerializeField]
    private TrackPieceController _trackPieceController;

    [SerializeField]
    private GameObject _confirmCanvas;

    [SerializeField]
    private Button _confirmButton;

    [SerializeField]
    private Button _cancelButton;

    [SerializeField]
    private GameObject _typeSwitchCanvas;

    [SerializeField]
    private Button _nextTypeButton;

    [SerializeField]
    private Button _previousTypeButton;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private TrackPieceType _trackPieceType;

    public TrackPieceType TrackPieceType {
        get => _trackPieceType;
        private set {
            _trackPieceType = value;
            OnSetTrackPieceType();
        }
    }

    private Compass _Direction;

    public Compass Direction => _Direction;

    public TrackPiece Position => _trackPieceController.TrackPiece.Copy();

    public Action OnOk;

    void Awake() {
        TrackPieceType = TrackPieceType.Straight;
    }

    void Start() {
        _confirmButton.onClick.AddListener(() => OnOk?.Invoke());
        _cancelButton.onClick.AddListener(OnCancel);
        _previousTypeButton.onClick.AddListener(OnPreviousType);
        _nextTypeButton.onClick.AddListener(OnNextType);
    }

    private void OnPreviousType() {
        List<TrackPieceType> types =  ToyMapManager.Instance.TrackPiecePrefabs.Keys.ToList();
        int currentIndex = types.IndexOf(_trackPieceType);
        int nextIndex = ((currentIndex == 0 ? types.Count : currentIndex) - 1) % types.Count;
        TrackPieceType = types[nextIndex];
    }

    private void OnNextType() {
        List<TrackPieceType> types =  ToyMapManager.Instance.TrackPiecePrefabs.Keys.ToList();
        int currentIndex = types.IndexOf(_trackPieceType);
        int nextIndex = (currentIndex + 1) % types.Count;
        TrackPieceType = types[nextIndex];
    }

    void OnCancel() {
        Debug.Log("TODO: cancel: pop a tile or what?");
    }

    private void OnSetTrackPieceType() {
        ToyTrackPiecePrefab prefab = ToyMapManager.Instance.TrackPiecePrefabs[_trackPieceType];
        _spriteRenderer.sprite = prefab.sprite;

        Compass[] connections = prefab.template.ConnectionPoints;

        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass forwardDirection = connections[1];
        Rotation rotation = forwardDirection.ToRotation();

        // Upon reading this line, now you can be sure to your very heart of hearts, that you have seen the face of evil.
        _confirmCanvas.transform.localEulerAngles = new Vector3(0, 0, -(int)rotation);

        // Bit inefficient but let's not worry about it
        _trackPieceController.TrackPiece = new TrackPiece() {
            X = _trackPieceController.TrackPiece?.X ?? 0,
            Y = _trackPieceController.TrackPiece?.Y ?? 0,
            Rotation = _trackPieceController.TrackPiece?.Rotation ?? Rotation.None,
            Template = ToyMapManager.Instance.TrackPiecePrefabs[_trackPieceType].template
        };
    }

    public void SetPosition(Compass direction, TrackPiece fromTrackPiece) {
        _Direction = direction;

        TrackPiece trackPiece = Connection.GetNextTrackPiece(fromTrackPiece, direction);
        trackPiece.Template = ToyMapManager.Instance.TrackPiecePrefabs[_trackPieceType].template;
        _trackPieceController.TrackPiece = trackPiece;

        // Avert thine eyes, lest your soul forever be scarred from what you see here today.
        _typeSwitchCanvas.transform.localEulerAngles = new Vector3(0, 0, (int)trackPiece.Rotation);
    }
}
