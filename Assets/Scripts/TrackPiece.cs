using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation {
    Deg90 = 90,
    Deg180 = 180,
    Deg270 = 270,
}

[System.Serializable]
public class TrackPiece
{
    // visuals
    [SerializeField]
    public Rotation Rotation = Rotation.Deg90;

    [SerializeField]
    public TrackTemplate Template;

    void Start()
    {
    }

    void Update()
    {
    }
}
