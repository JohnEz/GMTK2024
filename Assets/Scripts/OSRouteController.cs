using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OSRouteController : MonoBehaviour {
    private Route _route;

    private TrainController _trainController;

    public Route Route {
        get { return _route; }
        set { SetRoute(value); }
    }

    private void SetRoute(Route route) {
        _route = route;
        _trainController.Spawn(route);
    }

    public void Awake() {
        _trainController = GetComponent<TrainController>();
    }

    public void HandleClick() {
        LineManager.Instance.HandleRouteClicked(this);
    }

    public void UpdateRouteColor(Color color) {
        List<OSTrackController> controllerList = GetComponentsInChildren<OSTrackController>().ToList();

        controllerList.ForEach(trackController => {
            trackController.UpdateTrackColor(color);
        });
    }

    public void AddTrain() {
        _trainController.AddTrain(transform);
    }
}