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

        var stations = GameObject.FindObjectsOfType<Station>();

        Station closestStation = null;
        float closestDistance = Mathf.Infinity;

        foreach (Station station in stations) {
            float distance = Vector3.Distance(
                station.transform.position,
                cursorPos
            );

            if (distance < closestDistance) {
                closestDistance = distance;
                closestStation = station;
            }
        }

        if (closestStation) {
            Vector3 stationPosition = closestStation.transform.position;

            var dx = cursorPos.x - stationPosition.x;
            var dy = cursorPos.y - stationPosition.y;
            var junctionOffset = 0.15f;

            Vector3 offset;
            if (Mathf.Abs(dx) > Mathf.Abs(dy)) {
                offset = new Vector3(dx > 0 ? junctionOffset : -junctionOffset, 0, 0);
            } else {
                offset = new Vector3(0, dy > 0 ? junctionOffset : -junctionOffset, 0);
            }

            GhostTile.transform.position = stationPosition + offset;
        }
    }
}
