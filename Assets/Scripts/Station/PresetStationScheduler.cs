using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StationSpawn {
    [SerializeField]
    public int x;

    [SerializeField]
    public int y;

    [SerializeField]
    public float delay;
}

[RequireComponent(typeof (StationManager))]
public class PresetStationScheduler : MonoBehaviour
{
    public List<StationSpawn> Spawns = new();

    private StationManager stationManager;

    private bool _spawning = true;

    private int _nextIndex = 0;

    private float _timeSinceLastSpawn = 0.0f;

    void Awake() {
        stationManager = GetComponent<StationManager>();
    }

    void Update() { 
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

    private void Spawn()
    {
        StationSpawn nextSpawn = Spawns[_nextIndex];

        TrackPiece station = new()
        {
            X = nextSpawn.x,
            Y = nextSpawn.y,
        };

        stationManager.AddStation(station);

        _nextIndex++;
    }
}
