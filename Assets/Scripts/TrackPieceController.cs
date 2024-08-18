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
            UpdatePosition();
            OnTrackPieceSet.Invoke(_trackPiece);
        }
    }

    private void UpdatePosition() {
        transform.position = new Vector3(_trackPiece.X, _trackPiece.Y, transform.position.z);
    }
}
