using UnityEngine;
using UnityEngine.Splines;

public enum TrackPieceType {
    CornerRight,
    CornerLeft,
    Straight,
    LoopDaLoop,
    Station,
    Jump
}

[CreateAssetMenu(fileName = "Track", menuName = "Track Template")]
public class TrackTemplate : ScriptableObject {
    public TrackPieceType TrackPieceType;

    [SerializeField]
    public Compass[] ConnectionPoints;

    // ^ length = 2
    public int Happiness = 0;

    public float Price;

    [SerializeField]
    private Spline spline;

    public Spline Spline { get { return spline; } }

    public int Time = 0;
}

public static class TrackPieceTypeExtensions {

    public static bool IsCorner(this TrackPieceType type) {
        switch (type) {
            case TrackPieceType.CornerRight:
            case TrackPieceType.CornerLeft:
            return true;

            case TrackPieceType.LoopDaLoop:
            case TrackPieceType.Straight:
            case TrackPieceType.Station:
            break;
        }
        return false;
    }
}