using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ToyTrackPieceConfig {
    public TrackTemplate template;

    public Sprite sprite;
}

public class ToyMapManager : Singleton<ToyMapManager>
{
    [SerializeField]
    private TrackPieceController _stationPrefab;

    [SerializeField]
    private TrackPieceController _trackPiecePrefab;

    [SerializeField]
    private List<ToyTrackPieceConfig> _trackPieceConfigList = new();

    [SerializeField]
    private GameObject _toyContainer;

    private List<Vector2> _toys = new();

    public List<Vector2> Toys {
        get {
            if (_toys.Count == 0) {
                _toys = _toyContainer
                    .GetComponentsInChildren<Collider2D>()
                    .Select(collider => (Vector2)collider.transform.position)
                    .ToList();
            }
            return _toys;
        }
    }

    private readonly Dictionary<int, Dictionary<int, TrackPieceController>> _trackPieces = new();

    public Dictionary<TrackPieceType, ToyTrackPieceConfig> TrackPieceConfig {
        private set;
        get;
    }

    void Start() {
        StationManager.Instance.OnStationAdded += OnStationAdded;
        StationManager.Instance.Stations.ForEach(station => OnStationAdded(station));

        RouteManager.Instance.OnRouteAdded += OnRouteAdded;
        RouteManager.Instance.Routes.ForEach(route => OnRouteAdded(route));
    }

    public TrackPieceController FindTrackPiece(TrackPiece piece) {
        return _trackPieces.ContainsKey(piece.X) && _trackPieces[piece.X].ContainsKey(piece.Y)
            ? _trackPieces[piece.X][piece.Y]
            : null;
    }

    private void OnStationAdded(TrackPiece station) {
        TrackPieceController stationController = Instantiate(_stationPrefab, transform);
        stationController.TrackPiece = station;
        StoreTrackPiece(stationController);
    }

    private void OnRouteAdded(Route route) {
        route.TrackPieces.ForEach(connection => {
            TrackPieceController newTrack = Instantiate(_trackPiecePrefab, transform);
            newTrack.TrackPiece = connection.Piece;
            newTrack.GetComponentInChildren<SpriteRenderer>().sprite = TrackPieceConfig[connection.Piece.Template.TrackPieceType].sprite;
            StoreTrackPiece(newTrack);
        });
    }

    private void StoreTrackPiece(TrackPieceController controller) {
        if (!_trackPieces.ContainsKey(controller.TrackPiece.X)) {
            _trackPieces[controller.TrackPiece.X] = new();
        }

        _trackPieces[controller.TrackPiece.X][controller.TrackPiece.Y] = controller;
    }

    private void OnValidate() {
        TrackPieceConfig = _trackPieceConfigList.ToDictionary(
            _trackPrefab => _trackPrefab.template.TrackPieceType,
            _trackPrefab => _trackPrefab
        );
    }
}
