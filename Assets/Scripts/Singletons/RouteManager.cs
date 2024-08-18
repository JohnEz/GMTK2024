using System.Collections.Generic;
using UnityEngine;

public class RouteManager : Singleton<RouteManager> {
    [SerializeField]
    public List<Route> Routes;

    public event System.Action<Route> OnRouteAdded;

    private void Awake() {
        // we need to calculate the route spline for any hardcoded routes
        Routes.ForEach(route => {
            route.CalculateSpline();
        });
    }

    public void AddRoute(Route route) {
        Routes.Add(route);
        route.CalculateSpline();
        OnRouteAdded.Invoke(route);
    }
}