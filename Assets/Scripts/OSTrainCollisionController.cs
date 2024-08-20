using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTrainCollisionController : MonoBehaviour {
    private const int MAX_PASSENGERS = 8;

    public OSStation LastStation { get; private set; }

    public TrainController MyTrainController { get; private set; }

    public List<OSPassenger> Passengers { get; private set; } = new List<OSPassenger>();

    [SerializeField]
    private List<GameObject> passengerSlots;

    private void Awake() {
        MyTrainController = GetComponent<TrainController>();
        ShowPassengers();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        OSStation station = collision.gameObject.GetComponent<OSStation>();

        if (station == null || station == LastStation) {
            return;
        }

        // Debug.Log($"{name} arrived at station {station.name}");
        LastStation = station;

        DropOffPassengers(station);
        CollectPassengers(station);
        ShowPassengers();
    }

    public void DropOffPassengers(OSStation station) {
        List<OSPassenger> droppedOffPassengers = new List<OSPassenger>();

        Passengers.ForEach(passenger => {
            if (passenger.ShouldGetOffAtStation(station)) {
                droppedOffPassengers.Add(passenger);
                passenger.ArriveAtStation(station);
            }
        });

        droppedOffPassengers.ForEach(passengerToRemove => {
            Passengers.Remove(passengerToRemove);
        });
    }

    public void CollectPassengers(OSStation station) {
        int remainingCapacity = MAX_PASSENGERS - Passengers.Count;
        int passengersToTake = Mathf.Min(station.Passengers.Count, remainingCapacity);
        int count = 0;

        List<OSPassenger> passengersToPickUp = new List<OSPassenger>();

        while (passengersToTake > 0 && count < station.Passengers.Count) {
            OSPassenger passenger = station.Passengers[count];

            if (passenger.ShouldGetOnTrain(MyTrainController.Line.Line.Color)) {
                passengersToPickUp.Add(passenger);
                passengersToTake--;
            }

            count++;
        }

        passengersToPickUp.ForEach(passengersToPickUp => {
            station.RemovePassenger(passengersToPickUp);
            passengersToPickUp.GetOnTrain();
            Passengers.Add(passengersToPickUp);
        });
    }

    private void ShowPassengers() {
        int count = 0;

        passengerSlots.ForEach(passengerSlot => {
            passengerSlot.gameObject.SetActive(count < Passengers.Count);
            count++;
        });
    }
}