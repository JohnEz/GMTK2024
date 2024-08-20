using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainManager : Singleton<TrainManager> {
    public TrainController _osTrainPrefab;

    private List<TrainController> _trains = new();

    private List<TrainController> _toyTrains = new();

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
        LineController osLineController = OSMapManager.Instance.GetLine(line.Color);

        TrainController train = Instantiate(_osTrainPrefab);
        train.transform.SetParent(osLineController.transform, false);

        train.SetLine(osLineController);
        _trains.Add(train);

        LineController toyLineController = ToyMapManager.Instance.GetLine(line.Color);

        TrainController toyTrain = Instantiate(_osTrainPrefab);
        toyTrain.transform.SetParent(toyLineController.transform, false);

        toyTrain.SetLine(toyLineController);
        toyTrain.SetTrainVisual(TrainVisual.Toy);
        _toyTrains.Add(toyTrain);

        int randomIndex = Random.Range(0, trainNames.Count);
        train.name = trainNames[randomIndex];
        toyTrain.name = trainNames[randomIndex];
        trainNames.RemoveAt(randomIndex);

        return train;
    }

    public void DestroyTrain(TrainController trainToDestroy) {
        TrainController toyTrainToDestroy = _toyTrains.Find(toyTrain => toyTrain.name == trainToDestroy.name);

        _trains.Remove(trainToDestroy);
        _toyTrains.Remove(toyTrainToDestroy);

        trainNames.Add(trainToDestroy.name);

        Destroy(trainToDestroy.gameObject);
        Destroy(toyTrainToDestroy.gameObject);
    }

    public void DestroyTrains(List<TrainController> trainsToDestroy) {
        trainsToDestroy.ForEach(trainToDestroy => {
            DestroyTrain(trainToDestroy);
        });
    }
}