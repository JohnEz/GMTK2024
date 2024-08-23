using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainManager : Singleton<TrainManager> {
    public TrainController _osTrainPrefab;

    private List<TrainController> _trains = new();

    private List<TrainController> _toyTrains = new();

    private List<string> _initialTrainNames = new() {
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

    private int trainNamesCount = -1;

    private List<string> trainNames = new();

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

        string trainName = GetTrainName();
        train.name = trainName;
        toyTrain.name = trainName;

        line.AddTrain("temp", train);

        return train;
    }

    private string GetTrainName() {
        if (trainNames.Count <= 0) {
            trainNames = new(_initialTrainNames);
            trainNamesCount++;
        }

        int randomIndex = UnityEngine.Random.Range(0, trainNames.Count);
        string trainName = trainNames[randomIndex];
        trainNames.RemoveAt(randomIndex);

        trainName = $"{trainName} {ToRoman(trainNamesCount)}";

        return trainName;
    }

    private static string ToRoman(int number) {
        if ((number < 0) || (number > 3999))
            throw new ArgumentOutOfRangeException(nameof(number), "insert value between 1 and 3999");
        if (number < 1)
            return string.Empty;
        if (number >= 1000)
            return "M" + ToRoman(number - 1000);
        if (number >= 900)
            return "CM" + ToRoman(number - 900);
        if (number >= 500)
            return "D" + ToRoman(number - 500);
        if (number >= 400)
            return "CD" + ToRoman(number - 400);
        if (number >= 100)
            return "C" + ToRoman(number - 100);
        if (number >= 90)
            return "XC" + ToRoman(number - 90);
        if (number >= 50)
            return "L" + ToRoman(number - 50);
        if (number >= 40)
            return "XL" + ToRoman(number - 40);
        if (number >= 10)
            return "X" + ToRoman(number - 10);
        if (number >= 9)
            return "IX" + ToRoman(number - 9);
        if (number >= 5)
            return "V" + ToRoman(number - 5);
        if (number >= 4)
            return "IV" + ToRoman(number - 4);
        if (number >= 1)
            return "I" + ToRoman(number - 1);

        return "";
    }

    public void DestroyTrain(TrainController trainToDestroy) {
        TrainController toyTrainToDestroy = _toyTrains.Find(toyTrain => toyTrain.name == trainToDestroy.name);

        _trains.Remove(trainToDestroy);
        _toyTrains.Remove(toyTrainToDestroy);

        trainNames.Add(trainToDestroy.name);

        OSTrainCollisionController collisionController = trainToDestroy.GetComponent<OSTrainCollisionController>();

        if (collisionController.Passengers.Count > 0) {
            collisionController.Passengers.ToList().ForEach(passenger => {
                collisionController.LastStation.AddPassenger(passenger);
                passenger.TeleportToStation(collisionController.LastStation);
            });
        }

        Destroy(trainToDestroy.gameObject);
        Destroy(toyTrainToDestroy.gameObject);
    }

    public void DestroyTrains(List<TrainController> trainsToDestroy) {
        trainsToDestroy.ForEach(trainToDestroy => {
            DestroyTrain(trainToDestroy);
        });
    }
}