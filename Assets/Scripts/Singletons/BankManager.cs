using System;
using UnityEngine;

public class BankManager : Singleton<BankManager> {
    public float StartingCash;

    public decimal Cash {
        get;
        private set;
    } = 0;

    public decimal TotalCash {
        get;
        private set;
    } = 0;

    public event Action<decimal, decimal> OnCashUpdate;

    void Awake() {
        Cash = (decimal) StartingCash;
    }

    public bool Spend(float amount) {
        // Unity can't serialize decimals, so we expect callers want work with floats
        // Hope we don't lose precision LOL
        decimal decAmount = (decimal) amount;

        if (decAmount > Cash) {
            return false;
        }

        UpdateCash(-decAmount);
        return true;
    }

    public void Reclaim(float amount) {
        // Unity can't serialize decimals, so we expect callers want work with floats
        // Hope we don't lose precision LOL
        decimal decAmount = (decimal) amount;
        UpdateCash(decAmount);
    }

    public void OnJourneyComplete(TrackPiece start, TrackPiece end) {
        Vector2 startLocation = new(start.X, start.Y);
        Vector2 endLocation = new(end.X, end.Y);
        float directDistance = (endLocation - startLocation).magnitude;
        decimal routePrice = (decimal)Math.Round(directDistance, 2);
        // Debug.Log($"Completed journey from {start.X}, {start.Y} to {end.X}, {end.Y} costing {routePrice}");
        UpdateCash(routePrice);
    }

    private void UpdateCash(decimal diff) {
        if (diff == 0) {
            return;
        }

        Cash += diff;

        if (!GameStateManager.Instance.IsGameOver()) {
            TotalCash += diff;
        }

        OnCashUpdate(Cash, diff);
    }
}