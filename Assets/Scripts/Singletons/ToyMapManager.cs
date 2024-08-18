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

    private void OnStationAdded(TrackPiece station) {
        TrackPieceController stationController = Instantiate(_stationPrefab, transform);
        stationController.TrackPiece = station;
    }

    private void OnRouteAdded(Route route) {
        route.TrackPieces.ForEach(connection => {
            TrackPieceController newTrack = Instantiate(_trackPiecePrefab, transform);
            newTrack.TrackPiece = connection.Piece;
            newTrack.GetComponentInChildren<SpriteRenderer>().sprite = TrackPieceConfig[connection.Piece.Template.TrackPieceType].sprite;
        });
    }

    private void OnValidate() {
        TrackPieceConfig = _trackPieceConfigList.ToDictionary(
            _trackPrefab => _trackPrefab.template.TrackPieceType,
            _trackPrefab => _trackPrefab
        );
    }
}
