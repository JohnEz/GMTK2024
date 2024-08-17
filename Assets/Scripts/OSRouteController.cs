using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSRouteController : MonoBehaviour {
    private Route _route;

    public Route Route {
        get { return _route; }
        set { SetRoute(value); }
    }

    private void SetRoute(Route route) {
        _route = route;
    }
}