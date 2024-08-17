using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTrackController : MonoBehaviour {
    private TrackPiece _trackPiece;

    [SerializeField]
    private SpriteRenderer _tileRenderer;

    public TrackPiece TrackPiece {
        get { return _trackPiece; }
        set { SetTrackPiece(value); }
    }

    private void Start() {
        Setup();
    }

    public void SetTrackPiece(TrackPiece trackPiece) {
        _trackPiece = trackPiece;

        Setup();
    }

    private void Setup() {
        if (TrackPiece == null) {
            _tileRenderer.sprite = null;
            return;
        }

        transform.position = new Vector3(TrackPiece.X, TrackPiece.Y, 0);
        _tileRenderer.sprite = TrackPiece.Template.AdultArt;
        ApplyRotation(TrackPiece.Rotation);
    }

    private void ApplyRotation(Rotation rotation) {
        transform.eulerAngles = new Vector3(0, 0, -(int)rotation);
    }
}