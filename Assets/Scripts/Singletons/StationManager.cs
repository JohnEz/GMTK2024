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

    public TrackPiece GetConnectingStation(TrackPiece piece) {
        // Big assumption that there's only ever two connections, and the second one is the "exit" or "forward" connector
        Compass nextDirection = piece.Template.ConnectionPoints[1];
        TrackPiece nextTrackPiece = Connection.GetNextTrackPiece(piece, nextDirection);

        return Stations.Find(station => station.X == nextTrackPiece.X && station.Y == nextTrackPiece.Y);
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