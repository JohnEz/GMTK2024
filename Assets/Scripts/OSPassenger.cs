using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSPassenger : MonoBehaviour {
    public OSStation CurrentStation { get; private set; }

    public OSStation FinalStation { get; private set; }

    public PassengerPath Path { get; private set; }

    public OSStation StationToGetOffAt { get; private set; }

    private bool isRidingTrain = false;

    public void OnDestroy() {
        OSConnectionManager.Instance.OnConnectionsChange -= HandleConnectionsChange;
    }

    public void Setup(OSStation startingStation) {
        // kill self?

        CalculateRandomEndStation(startingStation);

        CurrentStation = startingStation;

        HandleConnectionsChange();
        OSConnectionManager.Instance.OnConnectionsChange += HandleConnectionsChange;

        Debug.Log($"I spawned at {startingStation.name} and I want to go to {FinalStation.name}. It will be {Path.Connections.Count} stops.");
    }

    private void CalculateRandomEndStation(OSStation startingStation) {
        OSStation targetStation;
        int iterations = 0;

        do {
            iterations++;
            int targetIndex = Random.Range(0, OSMapManager.Instance.Stations.Count);

            targetStation = OSMapManager.Instance.Stations[targetIndex];
        } while (targetStation == startingStation && iterations < 1000);

        FinalStation = targetStation;
    }

    public void GetOnTrain() {
        isRidingTrain = true;
        StationToGetOffAt = Path.Connections[0].Station;
        Path.Connections.RemoveAt(0);
    }

    public void ArriveAtStation(OSStation newStation) {
        isRidingTrain = false;
        CurrentStation = newStation;
    }

    public bool ShouldGetOnTrain(Color lineColor) {
        return lineColor == Path.Connections[0].LineColor;
    }

    private void HandleConnectionsChange() {
        Path = OSConnectionManager.Instance.CalculatePath(isRidingTrain ? StationToGetOffAt : CurrentStation, FinalStation);
    }
}