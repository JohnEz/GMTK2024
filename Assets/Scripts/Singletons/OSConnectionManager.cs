using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class StationConnection {

    public StationConnection(Color lineColor, OSStation station) {
        LineColor = lineColor;
        Station = station;
    }

    public Color LineColor;
    public OSStation Station;
}

public class PassengerPath {
    public OSStation Start;
    public OSStation End;

    public List<PassengerPathConnection> Connections = new List<PassengerPathConnection>();
}

public class PassengerPathConnection {
    public OSStation NextStation;
    public Color ColorTrainToCatch;
}

public class OSConnectionManager : MonoBehaviour {
    public Dictionary<OSStation, List<StationConnection>> ConnectionMap { get; private set; } = new();

    private void OnEnable() {
        LineManager.Instance.OnRouteLineChange += GenerateConnectionMap;
    }

    private void OnDisable() {
        if (LineManager.Instance != null) {
            LineManager.Instance.OnRouteLineChange -= GenerateConnectionMap;
        }
    }

    public void GenerateConnectionMap() {
        ConnectionMap.Clear();

        OSMapManager.Instance.Stations.ForEach(station => {
            List<StationConnection> connections = new List<StationConnection>();

            LineManager.Instance.GetLinesForStation(station.TrackPieceController.TrackPiece).ForEach(line => {
                line.Stations.ForEach(connectedStationPiece => {
                    OSStation connectedStation = OSMapManager.Instance.GetStationFromTrackPiece(connectedStationPiece);
                    bool alreadyAdded = connections.Where(connection => connection.Station == connectedStation).Any();

                    if (connectedStation != station && !alreadyAdded) {
                        connections.Add(new StationConnection(line.Color, connectedStation));
                    }
                });
            });

            ConnectionMap.Add(station, connections);
        });

        //Debug.Log("---Connections Start----");

        //foreach (var item in ConnectionMap) {
        //    string connectionsString = "";

        //    item.Value.ForEach(connection => connectionsString += connection.Station.name + " ");

        //    Debug.Log($"{item.Key.name} connections: {connectionsString}");
        //}

        //Debug.Log("---Connections End----");
    }

    public PassengerPath CalculatePath(OSStation start, OSStation end) {
        PassengerPath path = new PassengerPath();
        path.Start = start;
        path.End = end;
        path.Connections = new();

        return path;
    }
}