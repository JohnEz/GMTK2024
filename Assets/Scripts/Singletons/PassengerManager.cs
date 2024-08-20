using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : Singleton<PassengerManager> {

    [SerializeField]
    private OSPassenger _passengerPrefab;

    public List<OSPassenger> PassengerList { get; private set; } = new List<OSPassenger>();

    private float timePassed = 0;

    private bool _isSpawningAllowed = true;

    public int CompletedJournyCount {
        get;
        private set;
    } = 0;

    private void Update() {
        //timePassed += Time.deltaTime;
        //if (timePassed > 1f) {
        //    OSStation station = OSMapManager.Instance.Stations[Random.Range(0, OSMapManager.Instance.Stations.Count)];
        //    SpawnPassenger(station);
        //    timePassed = 0f;
        //}
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

        WorldFloatingTextManager.Instance.Show($"+£{adultPrice}", passenger.FinalStation.gameObject);

        if (!GameStateManager.Instance.IsGameOver()) {
            CompletedJournyCount++;
        }

        // Debug.Log($"Passenger completed their journey from {passenger.StartingStation} to {passenger.FinalStation}.");
    }
}