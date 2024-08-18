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
    private List<ToyTrackPiecePrefab> _trackPiecePrefabs = new();

    public Dictionary<TrackPieceType, ToyTrackPiecePrefab> TrackPiecePrefabs {
        private set;
        get;
    }

    private void OnValidate() {
        TrackPiecePrefabs = _trackPiecePrefabs.ToDictionary(
            _trackPrefab => _trackPrefab.template.TrackPieceType,
            _trackPrefab => _trackPrefab
        );
    }
}
