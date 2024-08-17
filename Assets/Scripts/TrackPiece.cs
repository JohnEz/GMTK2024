using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation {
    Deg90,
    Deg180,
    Deg270,
}

public class TrackPiece : MonoBehaviour
{
    public int X = 0;

    public int Y = 0;

    public Rotation Rotation;

    [SerializeField]
    public TrackTemplate Template;

    void Start()
    {

    }

    void Update()
    {

    }
}
