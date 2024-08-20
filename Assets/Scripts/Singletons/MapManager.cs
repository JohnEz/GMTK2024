using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    [SerializeField]
    private RouteController _routePrefab;

    [SerializeField]
    private LineController _linePrefab;

    public List<RouteController> RouteControllers { get; private set; }

    public Dictionary<Color, LineController> Lines { get; private set; }

    private void Awake() {
        RouteControllers = new List<RouteController>();
        Lines = new Dictionary<Color, LineController>();
    }

    private void OnEnable() {
        LineManager.Instance.OnLineAdded += HandleLineAdded;
        LineManager.Instance.OnLineRemoved += HandleLineRemoved;
    }

    private void OnDisable() {
        if (LineManager.Instance != null) {
            LineManager.Instance.OnLineAdded -= HandleLineAdded;
            LineManager.Instance.OnLineRemoved += HandleLineRemoved;
        }
    }

    private void HandleLineAdded(Color key, Line line) {
        LineController newLineObject = Instantiate(_linePrefab, transform);
        newLineObject.SetLine(line);

        Lines.Add(key, newLineObject);
    }

    private void HandleLineRemoved(Color key) {
        if (!Lines.ContainsKey(key)) {
            return;
        }

        Destroy(Lines[key].gameObject);
        Lines.Remove(key);
    }

    public RouteController CreateRoute(Route route) {
        RouteController newRouteObject = Instantiate(_routePrefab);
        newRouteObject.Route = route;
        newRouteObject.transform.SetParent(transform, false);

        RouteControllers.Add(newRouteObject);

        return newRouteObject;
    }
}