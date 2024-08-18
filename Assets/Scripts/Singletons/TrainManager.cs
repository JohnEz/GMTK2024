using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainManager : Singleton<TrainManager> {
    public GameObject trainPrefab;

    private List<SplineAnimate> _trains = new();

    public void AddTrain(string startingStation, Line lineToAddTo, Transform parent) {
        //GameObject train = Instantiate(trainPrefab);
        //train.transform.SetParent(parent, false);
        //SplineAnimate splineAnimate = train.GetComponent<SplineAnimate>();
        //splineAnimate.Container = lineToAddTo.Routes[0];
        //splineAnimate.Restart(true);
        //_trains.Add(splineAnimate);
    }
}