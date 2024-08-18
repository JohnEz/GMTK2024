using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[CreateAssetMenu(fileName = "Track", menuName = "Track Template")]
public class TrackTemplate : ScriptableObject {

    [SerializeField]
    public string ChildArt = "child0";

    [SerializeField]
    public Sprite AdultArt;

    [SerializeField]
    public Compass[] ConnectionPoints;

    // ^ length = 2

    [SerializeField]
    public float Happiness = 0;

    [SerializeField]
    private Spline spline;

    public Spline Spline { get { return spline; } }

    [SerializeField]
    public int Time = 0;

    private void Start() {
    }

    private void Update() {
    }
}