using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class StationSpawn {

    [SerializeField]
    public Transform locationTransform;

    [SerializeField]
    public float delay;
}

public enum SpawningPhase {
    Tutorial,
    Easy,
    Hard
}

[RequireComponent(typeof(StationManager))]
public class PresetStationScheduler : Singleton<PresetStationScheduler> {
    public List<Transform> TutorialSpawns = new();
    public List<Transform> EasySpawns = new();
    public List<Transform> HardSpawns = new();

    public List<Transform> CurrentStationsToSpawn = new();

    public SpawningPhase SpawningPhase { get; private set; } = SpawningPhase.Tutorial;

    private const float SPAWN_DELAY = 30f;

    private StationManager stationManager;

    [SerializeField]
    private TrackTemplate stationTemplate;

    private bool _spawning = true;

    private int _nextIndex = 0;

    private float _timeSinceLastSpawn = 0.0f;

    private void Awake() {
        stationManager = GetComponent<StationManager>();
    }

    public void Start() {
        TutorialSpawns.ForEach(spawn => {
            SpawnStation(spawn);
        });

        SpawningPhase = SpawningPhase.Tutorial;
        _spawning = false;
    }

    public void StartSpawning() {
        SpawningPhase = SpawningPhase.Easy;
        CurrentStationsToSpawn = new List<Transform>(EasySpawns);
        CameraManager.Instance.UpdateCameraDefaultZoom(CameraManager.DEFAULT_ZOOM_2);
        _timeSinceLastSpawn = SPAWN_DELAY - 2f;
        Resume();
    }

    private void Update() {
        if (SpawningPhase == SpawningPhase.Easy && CurrentStationsToSpawn.Count < 1) {
            CurrentStationsToSpawn = new List<Transform>(HardSpawns);
            SpawningPhase = SpawningPhase.Hard;
            CameraManager.Instance.UpdateCameraDefaultZoom(CameraManager.DEFAULT_ZOOM_3);
        }

        if (_spawning && CurrentStationsToSpawn.Count > 0) {
            _timeSinceLastSpawn += Time.deltaTime;

            if (_timeSinceLastSpawn >= SPAWN_DELAY) {
                int randomIndex = UnityEngine.Random.Range(0, CurrentStationsToSpawn.Count);
                Transform stationSpawn = CurrentStationsToSpawn[randomIndex];
                CurrentStationsToSpawn.RemoveAt(randomIndex);

                SpawnStation(stationSpawn);
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
        //StationSpawn nextSpawn = Spawns[_nextIndex];

        //TrackPiece station = new() {
        //    X = (int)nextSpawn.locationTransform.position.x,
        //    Y = (int)nextSpawn.locationTransform.position.y,
        //    Template = stationTemplate,
        //};

        //stationManager.AddStation(station);

        //_nextIndex++;
    }

    private void SpawnStation(Transform transform) {
        TrackPiece station = new() {
            X = (int)transform.position.x,
            Y = (int)transform.position.y,
            Template = stationTemplate,
        };

        stationManager.AddStation(station);
    }
}