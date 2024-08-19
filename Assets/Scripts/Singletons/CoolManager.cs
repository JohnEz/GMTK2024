using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CoolManager : Singleton<CoolManager> {
    private const int ADJACENT_TOY_DISTANCE = 3; // 3 roughly means we've gone next to a toy
    private const int ADJACENCY_MULTIPLIER = 2;

    public int Coolness {
        get;
        private set;
    } = 0;

    public event Action<int, int> OnCoolnessUpdate;

    public (int, List<(TrackPiece piece, int value, int toyIndex)>) ScoreRoute(
        List<TrackPiece> route,
        List<Vector2> toys
    ) {
        List<(TrackPiece, int, int)> coolnesses = new();
        int totalCool = 0;

        for (int i = 0; i < route.Count; i++) {
            var trackPiece = route[i];

            // It's cool to be happy, OK?
            int value = trackPiece.Template.Happiness;

            (var nearestToyDist, var toyIndex) = NearestToyDistance(trackPiece.X, trackPiece.Y, toys);
            var isAdjacent = nearestToyDist != -1 && nearestToyDist < ADJACENT_TOY_DISTANCE;

            if (isAdjacent) {
                value *= ADJACENCY_MULTIPLIER;
            }

            coolnesses.Add((trackPiece, value, isAdjacent ? toyIndex : -1));

            totalCool += value;
        }

        UpdateCoolness(totalCool);
        return (totalCool, coolnesses);
    }

    (float, int) NearestToyDistance(int x, int y, List<Vector2> toys) {
        float minDistance = Mathf.Infinity;
        int index = -1;

        Vector2 xy = new Vector2(x, y);

        for (int i = 0; i < toys.Count; i++) {
            var toy = toys[i];
            float distance = Vector2.Distance(xy, toy);

            if (distance < minDistance) {
                minDistance = distance;
                index = i;
            }
        }

        return index >= 0 ? (minDistance, index) : (-1, -1);
    }

    private void UpdateCoolness(int diff) {
        Coolness += diff;
        OnCoolnessUpdate(Coolness, diff);
    }
}
