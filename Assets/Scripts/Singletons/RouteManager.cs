using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteManager : Singleton<RouteManager>
{
    // this group of variables are all set together during an edit session:
    private Station EditStation;
    private GhostTrackPiece GhostTrackPiece;
    private Route NewRoute;

    [SerializeField]
    public List<Route> Routes;

    [SerializeField]
    public GameObject GhostTrackPiecePrefab;


    private bool IsEditing { get { return EditStation != null; } }

    void Start() {
    }

    void Update() {
        if (IsEditing && Input.GetKeyDown(KeyCode.Escape)) {
            StopEditing();
        }
        UpdateCursorPos();
    }

    public void StartEditing(Station fromStation) {
        if(IsEditing) return;

        EditStation = fromStation;
        NewRoute = new Route();
        Routes.Add(NewRoute); // TODO: drop this out if we cancel the route
        CreateNewPiece();
    }

    void CreateNewPiece() {
        if (GhostTrackPiece) {
            Destroy(GhostTrackPiece);
        }

        GhostTrackPiece = Instantiate(GhostTrackPiecePrefab).GetComponent<GhostTrackPiece>();
        GhostTrackPiece.OnOk += () => PlacePiece();
    }

    void StopEditing() {
        EditStation = null;
        Destroy(GhostTrackPiece);
        GhostTrackPiece = null;
        NewRoute = null;
    }

    void PlacePiece() {
        var cursorPos = GetCursorPos();
        (var closestStation, var compass) = FindClosestStationAndCompassDir(cursorPos);

        NewRoute.AddConnection(
            GhostTrackPiece.IntoTrackPiece(),
            compass.Reversed() // going from the point we're adding onto, to the point we're coming from
        );
    }

    void UpdateCursorPos() {
        if (!GhostTrackPiece) return;

        // maybe cache the last mouse pos:
        // if it's not changed exit early,
        // since this method might be expensive
        var cursorPos = GetCursorPos();

        (var closestStation, var compass) = FindClosestStationAndCompassDir(cursorPos);

        Vector3 offset;
        var junctionOffset = 0.15f;
        switch (compass) {
            case Compass.East:
                offset = new Vector3(junctionOffset, 0, 0);
                break;
            case Compass.West:
                offset = new Vector3(-junctionOffset, 0, 0);
                break;
            case Compass.North:
                offset = new Vector3(0, junctionOffset, 0);
                break;
            case Compass.South:
                offset = new Vector3(0, -junctionOffset, 0);
                break;
            default:
                Debug.Log("invalid compass");
                return;
        }

        Vector3 stationPosition = closestStation.transform.position;
        GhostTrackPiece.transform.position = stationPosition + offset;
    }

    Vector3 GetCursorPos() {
        Vector3 mouse = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(
            new Vector3(
                mouse.x,
                mouse.y,
                Camera.main.nearClipPlane
            )
        );
    }

    (Station, Compass) FindClosestStationAndCompassDir(Vector3 point) {
        var stations = GameObject.FindObjectsOfType<Station>();

        Station closestStation = null;
        float closestDistance = Mathf.Infinity;

        foreach (Station station in stations) {
            float distance = Vector3.Distance(
                station.transform.position,
                point
            );

            if (distance < closestDistance) {
                closestDistance = distance;
                closestStation = station;
            }
        }

        if (!closestStation) {
            return (null, Compass.North);
        }

        Vector3 stationPosition = closestStation.transform.position;

        var dx = point.x - stationPosition.x;
        var dy = point.y - stationPosition.y;

        Compass compass;
        if (Mathf.Abs(dx) > Mathf.Abs(dy)) {
            if(dx > 0){
                compass = Compass.East;
            }else{
                compass = Compass.West;
            }
        }else{
            if(dy > 0){
                compass = Compass.North;
            }else{
                compass = Compass.South;
            }
        }

        return (closestStation, compass);
    }
}
