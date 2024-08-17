using System.Linq;
using UnityEngine;

// Unused, maybe it's useful later
[RequireComponent(typeof (StationManager))]
public class RandomStationScheduler : MonoBehaviour
{
    private static Compass[] Directions = new Compass[]{
        Compass.East,
        Compass.North,
        Compass.West,
        Compass.South,
    };

    public float SpawnTime = 5;

    public int SpawnDistance = 5;

    private StationManager stationManager;

    private bool _spawning = true;

    private float _timeSinceLastSpawn = 0;

    void Awake() {
        stationManager = GetComponent<StationManager>();
    }

    void Update() { 
        if (_spawning) {
            _timeSinceLastSpawn += Time.deltaTime;
            if (_timeSinceLastSpawn >= SpawnTime) {
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
        TrackPiece station;

        if (stationManager.Stations.Count == 0) {
            station = new()
            {
                X = 0,
                Y = 0,
            };
        } else {
            Compass direction = GetRandomDirection();
            TrackPiece refStation = GetOutlier(direction);
            Vector3 spawnPosition = GetSpawnVector(direction);

            station = new()
            {
                X = refStation.X + spawnPosition.x,
                Y = refStation.Y + spawnPosition.y,
            };
        }

        stationManager.AddStation(station);
    }

    private Compass GetRandomDirection() {
        return Directions[Random.Range(0, Directions.Length)];
    }

    private TrackPiece GetOutlier(Compass direction) {
        return stationManager.Stations.Aggregate((s1, s2) =>
            direction == Compass.North
                ? (s1.Y > s2.Y ? s1 : s2)
                : direction == Compass.East
                ? (s1.X > s2.X ? s1 : s2)
                : direction == Compass.South
                ? (s1.Y < s2.Y ? s1 : s2)
                : (s1.X < s2.X ? s1 : s2)
        );
    }

    private Vector3 GetSpawnVector(Compass direction) {
        int crossOffset = Random.Range(-2, 3);
        int mainOffset = SpawnDistance - System.Math.Abs(crossOffset);
        return direction == Compass.North
            ? new Vector3(crossOffset, mainOffset, 0)
            : direction == Compass.East
            ? new Vector3(mainOffset, crossOffset, 0)
            : direction == Compass.South
            ? new Vector3(crossOffset, -mainOffset, 0)
            : new Vector3(-mainOffset, crossOffset, 0);
    }
}
