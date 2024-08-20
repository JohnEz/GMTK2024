using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class OSTrackPiecePrefab {
    public TrackPieceType pieceType;

    public TrackPieceController prefab;
}

public class OSMapManager : Singleton<OSMapManager> {

    [SerializeField]
    private List<OSTrackPiecePrefab> _trackPiecePrefabs = new();

    private Dictionary<TrackPieceType, TrackPieceController> _trackPiecePrefabMap;

    public List<TrackPieceController> Tracks { get; private set; }

    public List<OSStation> Stations { get; private set; }

    private HashSet<OSStation> _connectedStations;

    private MapManager _mapManager;

    private List<string> StationNames = new List<string>() {
        "Stoke on trent",
        "Belper",
        "Stafford",
        "Leek",
        "Warrington",
        "Derby",
        "Nottingham",
        "Stone",
        "Uttoxeter",
        "Burton",
        "Cannock",
        "Telford",
        "Shrewsbury",
        "Coalbrookdale",
        "Walsall",
        "Tamworth",
        "Dudley",
        "Birmingham",
        "Nuneaton",
        "Coventry",
        "Kidderminster",
        "Redditch",
        "Worcester",
        "Gloucester",
        "Oxford"
    };

    private void Awake() {
        Tracks = new List<TrackPieceController>();
        Stations = new List<OSStation>();
        _connectedStations = new HashSet<OSStation>();
        _mapManager = GetComponent<MapManager>();

        _trackPiecePrefabMap = _trackPiecePrefabs.ToDictionary(
            _trackPrefab => _trackPrefab.pieceType,
            _trackPrefab => _trackPrefab.prefab
        );
    }

    private void Start() {
        StationManager.Instance.OnStationAdded += SpawnStation;
        SpawnStations(StationManager.Instance.Stations);

        RouteManager.Instance.OnRouteAdded += SpawnRoute;
        SpawnRoutes(RouteManager.Instance.Routes);
    }

    private void SpawnStations(List<TrackPiece> stations) {
        stations.ForEach(SpawnStation);
    }

    public LineController GetLine(Color lineColor) {
        return _mapManager.Lines[lineColor];
    }

    private void SpawnStation(TrackPiece station) {
        TrackPieceController newStation = Instantiate(_trackPiecePrefabMap[station.Template.TrackPieceType]);
        newStation.TrackPiece = station;

        newStation.transform.SetParent(transform, false);

        int randomIndex = UnityEngine.Random.Range(0, StationNames.Count);

        OSStation newStationController = newStation.GetComponent<OSStation>();
        newStationController.Setup(StationNames[randomIndex]);

        StationNames.RemoveAt(randomIndex);

        Stations.Add(newStationController);
    }

    private void SpawnRoutes(List<Route> routes) {
        routes.ForEach(SpawnRoute);
    }

    private void SpawnRoute(Route route) {
        RouteController newRouteObject = _mapManager.CreateRoute(route);

        route.TrackPieces.ForEach(connection => {
            TrackPieceController newTrack = Instantiate(_trackPiecePrefabMap[connection.Piece.Template.TrackPieceType]);
            newTrack.TrackPiece = connection.Piece;

            Tracks.Add(newTrack);
            newTrack.transform.SetParent(newRouteObject.transform, false);
        });

        //Color lineColor = LineManager.Instance.GetLineByRoute(newRouteObject);

        //newRouteObject.UpdateRouteColor(lineColor);

        OSStation startStation = GetStationFromTrackPiece(route.StartStation);
        OSStation endStation = GetStationFromTrackPiece(route.EndStation);
        OSStation newStation = _connectedStations.Contains(startStation) ? endStation : startStation;

        _connectedStations.Add(startStation);
        _connectedStations.Add(endStation);

        if (PresetStationScheduler.Instance.SpawningPhase == SpawningPhase.EndGame) {
            CheckGameWon(newStation);
        }
    }

    private void CheckGameWon(OSStation newStation) {
        Debug.Log($"Did we win? (Spawned stations: {Stations.Count}, Connected stations: {_connectedStations.Count})");
        if (Stations.Count == _connectedStations.Count) {
            GameStateManager.Instance.GameWon(newStation);
        }
    }

    public OSStation GetStationFromTrackPiece(TrackPiece _stationPiece) {
        return Stations.Where(station =>
            station.TrackPieceController.TrackPiece.X == _stationPiece.X &&
            station.TrackPieceController.TrackPiece.Y == _stationPiece.Y
        ).FirstOrDefault();
    }
}