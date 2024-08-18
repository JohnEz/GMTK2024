using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

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

    public TrackPiece Copy()
    {
        return new TrackPiece() {
            X = X,
            Y = Y,
            Rotation = Rotation,
            Template = Template,
        };
    }
}