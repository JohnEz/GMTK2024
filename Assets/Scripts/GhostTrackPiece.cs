using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;

[RequireComponent(typeof (TrackPieceController))]
public class GhostTrackPiece : MonoBehaviour {
    private TrackPieceController _trackPieceController;

    private Compass _Direction;

    public Compass Direction => _Direction;

    public TrackPiece Position => _trackPieceController.TrackPiece.Copy();

    public Action OnOk;

    void Awake() {
        _trackPieceController = GetComponent<TrackPieceController>();
    }

    void Start() {
        OnButton("ButtonOk", () => OnOk?.Invoke());
        OnButton("ButtonCancel", OnCancel);
    }

    void Update() {
    }

    void OnCancel() {
        Debug.Log("TODO: cancel: pop a tile or what?");
    }

    void OnButton(string name, UnityAction callback) {
        var canvas = transform.Find("Canvas");
        var btn = canvas.Find(name).GetComponent<Button>();
        btn.onClick.AddListener(callback);
    }

    public void SetPosition(Compass direction, TrackPiece fromTrackPiece) {
        _Direction = direction;

        switch (direction) {
            case Compass.North:
                _trackPieceController.TrackPiece = new TrackPiece() {
                    X = fromTrackPiece.X,
                    Y = fromTrackPiece.Y + 1,
                    Rotation = Rotation.Deg270
                };
                break;
            case Compass.East:
                _trackPieceController.TrackPiece = new TrackPiece() {
                    X = fromTrackPiece.X + 1,
                    Y = fromTrackPiece.Y,
                    Rotation = Rotation.None
                };
                break;
            case Compass.South:
                _trackPieceController.TrackPiece = new TrackPiece() {
                    X = fromTrackPiece.X,
                    Y = fromTrackPiece.Y - 1,
                    Rotation = Rotation.Deg90
                };
                break;
            case Compass.West:
                _trackPieceController.TrackPiece = new TrackPiece() {
                    X = fromTrackPiece.X - 1,
                    Y = fromTrackPiece.Y,
                    Rotation = Rotation.Deg180
                };
                break;
        }
    }
}
