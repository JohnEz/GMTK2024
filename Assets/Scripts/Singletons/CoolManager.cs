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

    public (int, List<(TrackPiece piece, string coolMessage)>) ScoreRoute(
        List<TrackPiece> route,
        List<Vector2> toys
    ) {
        List<(TrackPiece, string)> coolnesses = new();
        int totalCool = route.Aggregate(0, (acc, trackPiece) => {
            // It's cool to be happy, OK?
            int value = trackPiece.Template.Happiness;

            var nearestToyDist = NearestToyDistance(trackPiece.X, trackPiece.Y, toys);
            string coolMessage;
            if (nearestToyDist != -1 && nearestToyDist < ADJACENT_TOY_DISTANCE) {
                value *= ADJACENCY_MULTIPLIER;
                coolMessage = $"+{value}!!";
            } else {
                coolMessage = $"+{value}";
            }

            coolnesses.Add((trackPiece, coolMessage));
            return acc + value;
        });

        UpdateCoolness(totalCool);
        return (totalCool, coolnesses);
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

    private void UpdateCoolness(int diff) {
        Coolness += diff;
        OnCoolnessUpdate(Coolness, diff);
    }
}
