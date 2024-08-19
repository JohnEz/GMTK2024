using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CoolManager : Singleton<CoolManager> {
    private const int ADJACENT_TOY_DISTANCE = 3; // 3 roughly means we've gone next to a toy
    private const int ADJACENCY_MULTIPLIER = 2;

    private int _Coolness = 0;

    public int Coolness {
        get => _Coolness;
        set {
            _Coolness = value;
            OnCoolnessUpdate?.Invoke(value);
        }
    }

    public event Action<int> OnCoolnessUpdate;

    public (
        int,
        List<(TrackPiece piece, int value)>,
        List<(TrackPiece piece, int bonus)>
    ) ScoreRoute(
        List<TrackPiece> route,
        List<Vector2> toys
    ) {
        List<(TrackPiece, int)> coolnesses = new();
        List<(TrackPiece, int)> bonuses = new();

        int totalCool = route.Aggregate(0, (acc, trackPiece) => {
            // It's cool to be happy, OK?
            int value = trackPiece.Template.Happiness;

            var nearestToyDist = NearestToyDistance(trackPiece.X, trackPiece.Y, toys);
            if (nearestToyDist != -1 && nearestToyDist < ADJACENT_TOY_DISTANCE) {
                value *= ADJACENCY_MULTIPLIER;
                bonuses.Add((trackPiece, ADJACENCY_MULTIPLIER));
            }

            coolnesses.Add((trackPiece, value));
            return acc + value;
        });

        Coolness += totalCool;
        return (totalCool, coolnesses, bonuses);
    }

    float NearestToyDistance(int x, int y, List<Vector2> toys) {
        float minDistance = Mathf.Infinity;
        bool found = false;

        Vector2 xy = new Vector2(x, y);

        foreach (var toy in toys) {
            float distance = Vector2.Distance(xy, toy);

            if (distance < minDistance) {
                minDistance = distance;
                found = true;
            }
        }

        return found ? minDistance : -1;
    }

}
