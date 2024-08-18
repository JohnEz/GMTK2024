using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ToyTrackPiecePrefab {
    public TrackTemplate template;

    public Sprite sprite;
}

public class ToyMapManager : Singleton<ToyMapManager>
{
    [SerializeField]
    private TrackPieceController _stationPrefab;

    [SerializeField]
    private List<ToyTrackPiecePrefab> _trackPiecePrefabs = new();

    public Dictionary<TrackPieceType, ToyTrackPiecePrefab> TrackPiecePrefabs {
        private set;
        get;
    }

    void Awake() {
        StationManager.Instance.OnStationAdded += OnStationAdded;
        StationManager.Instance.Stations.ForEach(station => OnStationAdded(station));
    }

    private void OnStationAdded(TrackPiece station) {
        TrackPieceController stationController = Instantiate(_stationPrefab, transform);
        stationController.TrackPiece = station;
    }

    private void OnValidate() {
        TrackPiecePrefabs = _trackPiecePrefabs.ToDictionary(
            _trackPrefab => _trackPrefab.template.TrackPieceType,
            _trackPrefab => _trackPrefab
        );
    }
}
