using System;
using System.Collections.Generic;
using System.Linq;

public class CoolManager : Singleton<CoolManager> {
    private int _Coolness = 0;

    public int Coolness {
        get => _Coolness;
        set {
            _Coolness = value;
            OnCoolnessUpdate?.Invoke(value);
        }
    }

    public event Action<int> OnCoolnessUpdate;

    public (int, List<(TrackPiece piece, int coolness)>) ScoreRoute(List<TrackPiece> route) {
        List<(TrackPiece, int)> coolnesses = new();
        int totalCool = route.Aggregate(0, (acc, trackPiece) => {
            // It's cool to be happy, OK?
            int value = trackPiece.Template.Happiness;
            coolnesses.Add((trackPiece, value));
            return acc + value;
        });

        Coolness += totalCool;
        return (totalCool, coolnesses);
    }
}