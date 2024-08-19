using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StationSpawn {

    [SerializeField]
    public Transform locationTransform;

    [SerializeField]
    public float delay;
}

[RequireComponent(typeof(StationManager))]
public class PresetStationScheduler : MonoBehaviour {
    public List<StationSpawn> Spawns = new();

    private StationManager stationManager;

    [SerializeField]
    private TrackTemplate stationTemplate;

    private bool _spawning = true;

    private int _nextIndex = 0;

    private float _timeSinceLastSpawn = 0.0f;

    private void Awake() {
        stationManager = GetComponent<StationManager>();
    }

    private void Update() {
        if (_spawning && _nextIndex < Spawns.Count) {
            _timeSinceLastSpawn += Time.deltaTime;
            if (_timeSinceLastSpawn >= Spawns[_nextIndex].delay) {
                Spawn();
                _timeSinceLastSpawn = 0;
            }
        }
    }

    public void Resume() {
        _spawning = true;
    }

    public void Pause() {
        _spawning = false;
    }

    private void Spawn() {
        StationSpawn nextSpawn = Spawns[_nextIndex];

        TrackPiece station = new() {
            X = (int)nextSpawn.locationTransform.position.x,
            Y = (int)nextSpawn.locationTransform.position.y,
            Template = stationTemplate,
        };

        stationManager.AddStation(station);

        _nextIndex++;
    }
}