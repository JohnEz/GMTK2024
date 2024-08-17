using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation {
    None = 0,
    Deg90 = 90,
    Deg180 = 180,
    Deg270 = 270,
}

[System.Serializable]
public class TrackPiece {
    public float X;

    public float Y;

    // visuals
    [SerializeField]
    public Rotation Rotation = Rotation.None;

    [SerializeField]
    public TrackTemplate Template;

    private void Start() {
    }

    private void Update() {
    }
}