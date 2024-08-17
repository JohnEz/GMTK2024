using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Track", menuName = "Track Template")]
public class TrackTemplate : ScriptableObject
{
    [SerializeField]
    public string ChildArt = "child0";

    [SerializeField]
    public string AdultArt = "adult0";

    [SerializeField]
    public Compass[] ConnectionPoint;
    // ^ length = 2

    [SerializeField]
    public float Happiness = 0;

    [SerializeField]
    public int Time = 0;

    void Start()
    {

    }

    void Update()
    {

    }
}
