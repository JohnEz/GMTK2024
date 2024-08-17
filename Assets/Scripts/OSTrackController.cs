using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OSTrackController : MonoBehaviour, IPointerDownHandler {
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

    public void UpdateTrackColor(Color newColor) {
        _tileRenderer.color = newColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
    }
}