using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StationManager : MonoBehaviour
{
    public List<TrackPiece> Stations => _Stations;

    public UnityEvent<TrackPiece> OnStationAdded = new();

    private readonly List<TrackPiece> _Stations = new();

    public void AddStation(TrackPiece station) {
        _Stations.Add(station);
        OnStationAdded.Invoke(station);
    }

    public (TrackPiece, TrackPiece) ProposeJourney() {
        int start = Random.Range(0, _Stations.Count);
        int end;
        do {
            end = Random.Range(0, _Stations.Count);
        } while (start == end);

        return (_Stations[start], _Stations[end]);
    } 
}
