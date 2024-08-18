using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainManager : Singleton<TrainManager> {
    public TrainController trainPrefab;

    private List<TrainController> _trains = new();

    public TrainController SpawnTrain(string startingStation, OSRouteController startingRoute, Transform parent) {
        TrainController train = Instantiate(trainPrefab);
        train.transform.SetParent(parent, false);
        SplineAnimate splineAnimate = train.GetComponent<SplineAnimate>();
        splineAnimate.Container = startingRoute.SplineContainer;
        splineAnimate.Restart(true);
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