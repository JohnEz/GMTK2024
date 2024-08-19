using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor.MemoryProfiler;
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

    public List<StationConnection> Connections = new List<StationConnection>();
}

public class Node {

    public Node() {
    }

    public Node(int _x, int _y) {
        x = _x;
        y = _y;
    }

    public int x = -1;
    public int y = -1;

    public int cost = 0;
    public int dist = 0;
    public Node previous = null;

    // bad its here?
    public StationConnection connection = null;
}

public class OSConnectionManager : Singleton<OSConnectionManager> {
    public Dictionary<OSStation, List<StationConnection>> ConnectionMap { get; private set; } = new();

    public event Action OnConnectionsChange;

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

        OnConnectionsChange?.Invoke();
    }

    public static int ManhattanDistance(int x1, int x2, int y1, int y2) {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }

    public PassengerPath CalculatePath(OSStation start, OSStation end) {
        PassengerPath path = new PassengerPath();
        path.Start = start;
        path.End = end;
        path.Connections = new();

        if (start == null || end == null || ConnectionMap.Count == 0 || start == end) {
            return path;
        }

        Dictionary<OSStation, Node> nodes = new();

        List<Node> openList = new();

        Node source = new Node(start.TrackPieceController.TrackPiece.X, start.TrackPieceController.TrackPiece.Y);
        Node target = new Node(end.TrackPieceController.TrackPiece.X, end.TrackPieceController.TrackPiece.Y);

        nodes.Add(start, source);
        nodes.Add(end, target);

        nodes.Values.ToList().ForEach(key => {
            key.cost = int.MaxValue;
            key.previous = null;
            key.dist = ManhattanDistance(key.x, target.x, key.y, target.y);
        });

        source.cost = 0;
        source.previous = null;
        source.dist = 0;

        openList.Add(source);

        while (openList.Count > 0) {
            // find the current lowest cost node
            Node u = null;
            foreach (Node node in openList) {
                if (u == null || (node.cost + node.dist) < (u.cost + u.dist) || node == source || node == target) {
                    u = node;
                }
            }

            if (u == target || u == null) {
                break; //either no path or full path found
            }

            openList.Remove(u);

            OSStation currentStation = nodes.Where(kvp => kvp.Value == u).FirstOrDefault().Key;

            if (currentStation == null) {
                continue;
            }

            ConnectionMap[currentStation].ForEach(connection => {
                OSStation neighbour = connection.Station;
                bool nodeExists = nodes.ContainsKey(neighbour);
                Node neighbourNode;
                if (nodeExists) {
                    neighbourNode = nodes[neighbour];
                } else {
                    neighbourNode = new Node(neighbour.TrackPieceController.TrackPiece.X, neighbour.TrackPieceController.TrackPiece.Y);
                    nodes.Add(neighbour, neighbourNode);
                }

                int newCost = u.cost + 1;

                //if new cost is less than current cost or it isnt in dictionary
                if (!nodeExists || newCost < neighbourNode.cost) {
                    neighbourNode.cost = newCost;
                    neighbourNode.previous = u;
                    neighbourNode.connection = connection;
                    openList.Add(neighbourNode);
                }
            });
        }

        // Debug.Log($"-Generating path from: {start.name} to {end.name}-");

        if (target.previous == null) {
            // no path was found
            // Debug.Log("-----No Path Found------");
            return path;
        }

        List<Node> currentPath = new();
        Node currentNode = target;

        while (currentNode != null) {
            if (currentNode != source) {
                currentPath.Add(currentNode);
                path.Connections.Add(currentNode.connection);
            }
            currentNode = currentNode.previous;
        }

        currentPath.Reverse();
        path.Connections.Reverse();

        //Debug.Log("-----Path------");

        //path.Connections.ForEach(connection => {
        //    Debug.Log($"Take the {connection.LineColor} line to {connection.Station.name}");
        //});

        //Debug.Log("-----Path Complete------");

        return path;
    }
}