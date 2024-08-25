using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class RouteController : MonoBehaviour {
    private Route _route;

    public SplineContainer SplineContainer { get; private set; }

    [SerializeField]
    private AudioClip _clickClip;

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
        SplineContainer = GetComponent<SplineContainer>();
    }

    public void SetSpline(Spline spline) {
        SplineContainer.Spline = spline;
    }

    public void HandleClick() {
        if (!_isClickable) {
            return;
        }

        AudioManager.Instance.PlaySound(_clickClip, transform);

        LineManager.Instance.HandleRouteClicked(this);
    }

    public void HandleRightClick() {
        if (!_isClickable) {
            return;
        }

        AudioManager.Instance.PlaySound(_clickClip, transform);

        LineManager.Instance.HandleRouteRightClicked(this);
    }

    public void UpdateRouteColor(Color color) {
        List<OSTrackController> controllerList = GetComponentsInChildren<OSTrackController>().ToList();

        controllerList.ForEach(trackController => {
            trackController.UpdateTrackColor(color);
        });
    }
}