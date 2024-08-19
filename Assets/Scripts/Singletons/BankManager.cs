using System;
using UnityEngine;

public class BankManager : Singleton<BankManager>
{
    public decimal Cash {
        get;
        private set;
    } = 0;

    public event Action<decimal, decimal> OnCashUpdate;

    public bool Spend(decimal price) {
        if (price > Cash) {
            return false;
        }

        UpdateCash(-price);
        return true;
    }

    public void OnJourneyComplete(TrackPiece start, TrackPiece end) {
        Vector2 startLocation = new (start.X, start.Y);
        Vector2 endLocation = new(end.X, end.Y);
        float directDistance = (endLocation - startLocation).magnitude;
        decimal routePrice = (decimal)Math.Round(directDistance, 2);
        // Debug.Log($"Completed journey from {start.X}, {start.Y} to {end.X}, {end.Y} costing {routePrice}");
        UpdateCash(routePrice);
    }

    private void UpdateCash(decimal diff) {
        Cash += diff;
        OnCashUpdate(Cash, diff);
    }
}
