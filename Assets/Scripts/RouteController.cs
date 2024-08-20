using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class RouteController : MonoBehaviour {
    private Route _route;

    private TrainManager _trainController;

    public SplineContainer SplineContainer { get; private set; }

    private bool _isClickable;

    public Route Route {
        get { return _route; }
        set { SetRoute(value); }
    }

    private void SetRoute(Route route, bool isClickable = true) {
        _route = route;
        SetSpline(route.RouteSpline);
        _isClickable = isClickable;
    }

    public void Awake() {
        _trainController = GetComponent<TrainManager>();
        SplineContainer = GetComponent<SplineContainer>();
    }

    public void SetSpline(Spline spline) {
        SplineContainer.Spline = spline;
    }

    public void HandleClick() {
        if (!_isClickable) {
            return;
        }

        LineManager.Instance.HandleRouteClicked(this);
    }

    public void UpdateRouteColor(Color color) {
        List<OSTrackController> controllerList = GetComponentsInChildren<OSTrackController>().ToList();

        controllerList.ForEach(trackController => {
            trackController.UpdateTrackColor(color);
        });
    }
}