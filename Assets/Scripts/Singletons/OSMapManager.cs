using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class TrackPiecePrefab {
    public TrackPieceType pieceType;

    public TrackPieceController prefab;
}

public class OSMapManager : Singleton<OSMapManager> {

    [SerializeField]
    private List<TrackPiecePrefab> _trackPiecePrefabs = new();

    private Dictionary<TrackPieceType, TrackPieceController> _trackPiecePrefabMap;

    [SerializeField]
    private OSRouteController _routePrefab;

    [SerializeField]
    private OSLineController _linePrefab;

    public List<TrackPieceController> Tracks { get; private set; }

    public List<OSRouteController> RouteControllers { get; private set; }

    public Dictionary<Color, OSLineController> Lines { get; private set; }

    private void Awake() {
        Tracks = new List<TrackPieceController>();
        RouteControllers = new List<OSRouteController>();
        Lines = new Dictionary<Color, OSLineController>();
    }

    private void OnEnable() {
        LineManager.Instance.OnLineAdded += HandleLineAdded;
        LineManager.Instance.OnLineRemoved += HandleLineRemoved;
    }

    private void OnDisable() {
        if (LineManager.Instance != null) {
            LineManager.Instance.OnLineAdded -= HandleLineAdded;
            LineManager.Instance.OnLineRemoved += HandleLineRemoved;
        }
    }

    private void HandleLineAdded(Color key, Line line) {
        OSLineController newLineObject = Instantiate(_linePrefab, transform);
        newLineObject.SetLine(line);

        Lines.Add(key, newLineObject);
    }

    private void HandleLineRemoved(Color key) {
        if (!Lines.ContainsKey(key)) {
            return;
        }

        Destroy(Lines[key]);
        Lines.Remove(key);
    }

    private void Start() {
        SpawnRoutes(RouteManager.Instance.Routes);
    }

    private void SpawnRoutes(List<Route> routes) {
        routes.ForEach(SpawnRoute);
    }

    private void SpawnRoute(Route route) {
        OSRouteController newRouteObject = Instantiate(_routePrefab);
        newRouteObject.Route = route;
        newRouteObject.transform.SetParent(transform, false);

        route.TrackPieces.ForEach(connection => {
            TrackPieceController newTrack = Instantiate(_trackPiecePrefabMap[connection.Piece.Template.TrackPieceType]);
            newTrack.TrackPiece = connection.Piece;

            Tracks.Add(newTrack);
            newTrack.transform.SetParent(newRouteObject.transform, false);
        });

        Color lineColor = LineManager.Instance.GetLineByRoute(newRouteObject);

        newRouteObject.UpdateRouteColor(lineColor);

        RouteControllers.Add(newRouteObject);
    }

    private void OnValidate() {
        _trackPiecePrefabMap = _trackPiecePrefabs.ToDictionary(
            _trackPrefab => _trackPrefab.pieceType,
            _trackPrefab => _trackPrefab.prefab
        );
    }
}