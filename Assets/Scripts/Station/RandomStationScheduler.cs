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

    [SerializeField]
    private TrackTemplate stationTemplate;

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
                Template = stationTemplate,
            };
        } else {
            Compass direction = GetRandomDirection();
            TrackPiece refStation = GetOutlier(direction);
            (int x, int y) = GetSpawnVector(direction);

            station = new()
            {
                X = refStation.X + x,
                Y = refStation.Y + y,
                Template = stationTemplate,
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

    private (int x, int y) GetSpawnVector(Compass direction) {
        int crossOffset = Random.Range(-2, 3);
        int mainOffset = SpawnDistance - System.Math.Abs(crossOffset);
        return direction == Compass.North
            ? (crossOffset, mainOffset)
            : direction == Compass.East
            ? (mainOffset, crossOffset)
            : direction == Compass.South
            ? (crossOffset, -mainOffset)
            : (-mainOffset, crossOffset);
    }
}
