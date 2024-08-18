using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StationManager : Singleton<StationManager> {

    [SerializeField]
    private List<TrackPiece> _stations;

    public List<TrackPiece> Stations { get { return _stations; } private set { _stations = value; } }

    public event System.Action<TrackPiece> OnStationAdded;

    public void AddStation(TrackPiece station) {
        Stations.Add(station);
        OnStationAdded?.Invoke(station);
    }

    public (TrackPiece, TrackPiece) ProposeJourney() {
        int start = Random.Range(0, Stations.Count);
        int end;
        do {
            end = Random.Range(0, Stations.Count);
        } while (start == end);

        return (Stations[start], Stations[end]);
    }
}