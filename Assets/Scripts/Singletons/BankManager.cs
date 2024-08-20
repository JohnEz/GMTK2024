using System;
using UnityEngine;

public class BankManager : Singleton<BankManager> {

    // How much adult money is worth one kid money
    [SerializeField]
    private float exchangeRate;

    // How much kid money one unit of direct distance costs
    [SerializeField]
    private float distanceRate;

    public float StartingCash;

    public decimal Cash {
        get;
        private set;
    } = 0;

    public decimal TotalCash {
        get;
        private set;
    } = 0;

    public event Action<decimal> OnCashInit;

    public event Action<decimal, decimal> OnCashUpdate;

    private void Awake() {
        InitCash((decimal)StartingCash);
    }

    public bool Spend(float amount) {
        // Unity can't serialize decimals, so we expect callers want work with floats
        // Hope we don't lose precision LOL
        decimal decAmount = (decimal)amount;

        if (decAmount > Cash) {
            return false;
        }

        UpdateCash(-decAmount);
        return true;
    }

    public void Reclaim(float amount) {
        // Unity can't serialize decimals, so we expect callers want work with floats
        // Hope we don't lose precision LOL
        decimal decAmount = (decimal)amount;
        UpdateCash(decAmount);
    }

    public decimal OnJourneyComplete(TrackPiece start, TrackPiece end) {
        Vector2 startLocation = new(start.X, start.Y);
        Vector2 endLocation = new(end.X, end.Y);
        float directDistance = (endLocation - startLocation).magnitude;
        decimal routePrice = (decimal)Math.Round(directDistance * distanceRate, 2);
        UpdateCash(routePrice);
        return routePrice;
    }

    public decimal GetAdultValue(decimal value) {
        return value * (decimal)exchangeRate;
    }

    private void UpdateCash(decimal diff) {
        if (diff == 0) {
            return;
        }

        Cash += diff;

        if (!GameStateManager.Instance.IsGameOver() && diff > 0) {
            TotalCash += diff;
        }

        OnCashUpdate?.Invoke(Cash, diff);
    }

    private void InitCash(decimal value) {
        Cash = value;

        OnCashInit?.Invoke(Cash);
    }
}