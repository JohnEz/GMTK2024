using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation {
    Deg90 = 90,
    Deg180 = 180,
    Deg270 = 270,
}

public class TrackPiece
{
    // visuals
    [SerializeField]
    public Rotation Rotation;

    [SerializeField]
    public TrackTemplate Template;

    [SerializeField]
    public int X = 0;

    [SerializeField]
    public int Y = 0;

    void Start()
    {
    }

    void Update()
    {
        // TODO: maybe move this to just occuring once when the TrackPiece is placed
        var pos = this.transform.position;
        pos.x = this.X;
        pos.y = this.Y;
        this.transform.position = pos;

        this.transform.rotation = Quaternion.Euler(0, 0, (float)(int)this.Rotation);
    }
}
