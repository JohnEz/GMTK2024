using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteManager : Singleton<RouteManager>
{
    private Station EditStation = null;

    [SerializeField]
    public List<Route> Routes;

    private bool IsEditing { get { return EditStation != null; } }

    void Start() {
    }

    void Update() {
        if (IsEditing && Input.GetKeyDown(KeyCode.Escape)) {
            StopEditing();
        }
    }

    public void StartEditing(Station fromStation) {
        EditStation = fromStation;
        Routes = new List<Route>();
    }

    void StopEditing() {
        EditStation = null;
    }
}
