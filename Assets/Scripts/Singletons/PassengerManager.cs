using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : Singleton<PassengerManager> {

    [SerializeField]
    private OSPassenger _passengerPrefab;

    public List<OSPassenger> PassengerList { get; private set; } = new List<OSPassenger>();

    private float timePassed = 0;

    private void Update() {
        timePassed += Time.deltaTime;
        if (timePassed > 5f) {
            OSStation station = OSMapManager.Instance.Stations[Random.Range(0, OSMapManager.Instance.Stations.Count)];
            SpawnPassenger(station);
            timePassed = 0f;
        }
    }

    private void SpawnPassenger(OSStation startingStation) {
        OSPassenger newPassenger = Instantiate(_passengerPrefab);

        startingStation.AddPassenger(newPassenger);

        PassengerList.Add(newPassenger);
    }

    private void PassengerCompletedJourney(OSPassenger passenger) {
        PassengerList.Add(passenger);

        Destroy(passenger.gameObject);
    }
}