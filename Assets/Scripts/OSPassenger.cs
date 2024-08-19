using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSPassenger : MonoBehaviour {
    public OSStation TargetStation { get; private set; }

    public void Setup(OSStation startingStation) {
        // kill self?

        CalculateRandomEndStation(startingStation);

        //Debug.Log($"I spawned at {startingStation.name} and i want to go to {TargetStation.name}");
    }

    private void CalculateRandomEndStation(OSStation startingStation) {
        OSStation targetStation;
        int iterations = 0;

        do {
            iterations++;
            int targetIndex = Random.Range(0, OSMapManager.Instance.Stations.Count);

            targetStation = OSMapManager.Instance.Stations[targetIndex];
        } while (targetStation == startingStation && iterations < 1000);

        TargetStation = targetStation;
    }
}