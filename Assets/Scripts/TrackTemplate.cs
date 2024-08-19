using UnityEngine;
using UnityEngine.Splines;

public enum TrackPieceType {
    CornerRight,
    CornerLeft,
    Straight,
    LoopDaLoop,
    Station
}

[CreateAssetMenu(fileName = "Track", menuName = "Track Template")]
public class TrackTemplate : ScriptableObject {
    public TrackPieceType TrackPieceType;

    [SerializeField]
    public Compass[] ConnectionPoints;

    // ^ length = 2
    public int Happiness = 0;

    [SerializeField]
    private Spline spline;

    public Spline Spline { get { return spline; } }

    public int Time = 0;
}