using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteManager : Singleton<RouteManager>
{
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
            StopEditing(true);
        }
        UpdateCursorPos();
    }

    public void StartEditing(Station fromStation) {
        if(IsEditing) return;

        EditStation = fromStation;
        NewRoute = new Route();
        GhostTrackPiece = Instantiate(GhostTrackPiecePrefab).GetComponent<GhostTrackPiece>();
    }

    void StopEditing(bool cancel) {
        EditStation = null;

        if (cancel) {
            Destroy(GhostTrackPiece);
        } else {
            // TODO: GhostTrackPiece and NewRoute gubbins
        }
        GhostTrackPiece = null;
        NewRoute = null;
    }

    void UpdateCursorPos() {
        if (!GhostTrackPiece) return;

        Vector3 mouse = Input.mousePosition;

        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(
            new Vector3(
                mouse.x,
                mouse.y,
                Camera.main.nearClipPlane
            )
        );

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

    (Station, Compass?) FindClosestStationAndCompassDir(Vector3 point) {
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
            return (null, null);
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
