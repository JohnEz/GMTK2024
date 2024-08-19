using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainManager : Singleton<TrainManager> {
    public TrainController trainPrefab;

    private List<TrainController> _trains = new();

    public List<string> trainNames = new() {
        "Jamie",
        "Toby",
        "Thomas",
        "Andy",
        "Micky",
        "Elliot",
        "Jim",
        "Jimbob",
        "James",
        "Chris Morrison",
        "Akbo"
    };

    public TrainController SpawnTrain(string startingStation, Line line) {
        OSLineController lineController = OSMapManager.Instance.Lines[line.Color];

        TrainController train = Instantiate(trainPrefab);
        train.transform.SetParent(lineController.transform, false);

        int randomIndex = Random.Range(0, trainNames.Count);
        train.name = trainNames[randomIndex];
        trainNames.RemoveAt(randomIndex);

        train.SetLine(lineController);
        _trains.Add(train);

        return train;
    }

    public void DestroyTrain(TrainController trainToDestroy) {
        _trains.Remove(trainToDestroy);

        trainNames.Add(trainToDestroy.name);

        Destroy(trainToDestroy.gameObject);
    }

    public void DestroyTrains(List<TrainController> trainsToDestroy) {
        trainsToDestroy.ForEach(trainToDestroy => {
            DestroyTrain(trainToDestroy);
        });
    }
}