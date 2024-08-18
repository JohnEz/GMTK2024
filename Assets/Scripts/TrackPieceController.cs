using UnityEngine;
using UnityEngine.Events;

public class TrackPieceController : MonoBehaviour
{
    private TrackPiece _trackPiece;

    public UnityEvent<TrackPiece> OnTrackPieceSet = new();

    public TrackPiece TrackPiece
    {
        get => _trackPiece;
        set
        {
            _trackPiece = value;
            Setup();
            OnTrackPieceSet.Invoke(_trackPiece);
        }
    }

    void Awake() {
        Setup();
    }

    private void Setup() {
        if (_trackPiece == null) {
            return;
        }

        UpdatePosition();
        ApplyRotation();
    }

    private void UpdatePosition() {
        transform.localPosition = new Vector3(_trackPiece.X, _trackPiece.Y, 0);
    }

    private void ApplyRotation() {
        transform.eulerAngles = new Vector3(0, 0, -(int)TrackPiece.Rotation);
    }
}
