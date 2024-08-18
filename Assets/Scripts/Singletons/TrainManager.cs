using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainManager : Singleton<TrainManager> {
    public TrainController trainPrefab;

    private List<TrainController> _trains = new();

    public TrainController SpawnTrain(string startingStation, Line line) {
        OSLineController lineController = OSMapManager.Instance.Lines[line.Color];

        TrainController train = Instantiate(trainPrefab);
        train.transform.SetParent(lineController.transform, false);
        train.SetLine(lineController);
        _trains.Add(train);

        return train;
    }

    public void DestroyTrain(TrainController trainToDestroy) {
        _trains.Remove(trainToDestroy);

        Destroy(trainToDestroy.gameObject);
    }

    public void DestroyTrains(List<TrainController> trainsToDestroy) {
        trainsToDestroy.ForEach(trainToDestroy => {
            DestroyTrain(trainToDestroy);
        });
    }
}