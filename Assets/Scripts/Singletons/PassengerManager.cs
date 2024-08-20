using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassengerManager : Singleton<PassengerManager> {

    [SerializeField]
    private OSPassenger _passengerPrefab;

    public List<OSPassenger> PassengerList { get; private set; } = new List<OSPassenger>();

    private float _moneyFlushTimer = 0.1f;

    private float timePassedSincePassengers = 0;

    private bool _isSpawningAllowed = true;

    private Dictionary<OSStation, decimal> _moneyToAdd = new();

    public int CompletedJournyCount {
        get;
        private set;
    } = 0;

    private void Update() {
        timePassedSincePassengers += Time.deltaTime;
        if (timePassedSincePassengers > _moneyFlushTimer) {
            timePassedSincePassengers -= _moneyFlushTimer;

            _moneyToAdd.Keys.ToList().ForEach(key => {
                if (_moneyToAdd[key] > 0) {
                    WorldFloatingTextManager.Instance.Show($"+£{_moneyToAdd[key]:N0}", key.gameObject, isAdult: true);
                }
            });

            _moneyToAdd.Clear();
        }
    }

    public void ResumeSpawning() {
        _isSpawningAllowed = true;
    }

    public void PauseSpawning() {
        _isSpawningAllowed = false;
    }

    public void SpawnPassenger(OSStation startingStation) {
        if (!_isSpawningAllowed) {
            return;
        }

        OSPassenger newPassenger = Instantiate(_passengerPrefab);

        newPassenger.Setup(startingStation);

        startingStation.AddPassenger(newPassenger);

        PassengerList.Add(newPassenger);
    }

    public void PassengerCompletedJourney(OSPassenger passenger) {
        PassengerList.Remove(passenger);
        passenger.CurrentStation.RemovePassenger(passenger);

        Destroy(passenger.gameObject);

        decimal routePrice = BankManager.Instance.OnJourneyComplete(
            passenger.StartingStation.TrackPieceController.TrackPiece,
            passenger.FinalStation.TrackPieceController.TrackPiece
        );
        decimal adultPrice = BankManager.Instance.GetAdultValue(routePrice);

        //WorldFloatingTextManager.Instance.Show($"+£{adultPrice:N0}", passenger.FinalStation.gameObject, isAdult: true);

        if (!GameStateManager.Instance.IsGameOver()) {
            CompletedJournyCount++;
        }

        if (_moneyToAdd.ContainsKey(passenger.FinalStation)) {
            _moneyToAdd[passenger.FinalStation] += adultPrice;
        } else {
            _moneyToAdd[passenger.FinalStation] = adultPrice;
        }

        // Debug.Log($"Passenger completed their journey from {passenger.StartingStation} to {passenger.FinalStation}.");
    }
}