using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class Line {
    public Color Color { get; private set; }

    public List<Route> Routes { get; private set; }

    public List<TrainController> Trains { get; private set; }

    public Spline Spline { get; private set; }

    public bool IsLoop { get; private set; } = false;

    public event Action<int> OnTrainCountChange;

    public event Action<Spline> OnSplineChange;

    public TrackPiece StartStation { get; private set; }

    public TrackPiece EndStation { get; private set; }

    private Dictionary<Route, bool> _isRouteFlipped = new Dictionary<Route, bool>();

    public Line(Color color) {
        Routes = new List<Route>();
        Trains = new List<TrainController>();
        Color = color;
    }

    public bool CanAddRoute(Route newRoute) {
        if (Routes.Count == 0) {
            return true;
        }

        bool areStartStationsSame = newRoute.StartStation.X == StartStation.X && newRoute.StartStation.Y == StartStation.Y;
        bool areEndStationsSame = newRoute.EndStation.X == EndStation.X && newRoute.EndStation.Y == EndStation.Y;
        bool isNewStartOldEnd = newRoute.StartStation.X == EndStation.X && newRoute.StartStation.Y == EndStation.Y;
        bool isNewEndOldStart = newRoute.EndStation.X == StartStation.X && newRoute.EndStation.Y == StartStation.Y;

        return areStartStationsSame || areEndStationsSame || isNewStartOldEnd || isNewEndOldStart;
    }

    public void AddRoute(Route route) {
        if (Routes.Count == 0) {
            StartStation = route.StartStation;
            EndStation = route.EndStation;
            _isRouteFlipped[route] = false;
            Routes.Add(route);

            Debug.Log($"START: X: {StartStation.X} Y: {StartStation.Y}");
            Debug.Log($"END: X: {EndStation.X} Y: {EndStation.Y}");
        } else {
            AddRouteAndCalculateStations(route);
        }

        CalculateSpline();
    }

    public bool CanRemoveRoute(Route routeToRemove) {
        int index = Routes.IndexOf(routeToRemove);
        if (index == -1) {
            return false;
        }

        return index == 0 || index == Routes.Count - 1;
    }

    public void RemoveRoute(Route routeToRemove) {
        if (Routes.Count == 1) {
            StartStation = null;
            EndStation = null;
        } else {
            int index = Routes.IndexOf(routeToRemove);

            if (index == 0) {
                int nextIndex = index + 1;
                Route nextRoute = Routes[nextIndex];

                StartStation = _isRouteFlipped[nextRoute] ? nextRoute.EndStation : nextRoute.StartStation;

                Debug.Log($"NEW START: X: {StartStation.X} Y: {StartStation.Y}");
            } else if (index == Routes.Count - 1) {
                int previousIndex = index - 1;
                Route previousRoute = Routes[previousIndex];

                EndStation = _isRouteFlipped[previousRoute] ? previousRoute.StartStation : previousRoute.EndStation;

                Debug.Log($"NEW END: X: {EndStation.X} Y: {EndStation.Y}");
            } else {
                Debug.LogError("Tried to remove a middle track!");
                return;
            }
        }

        _isRouteFlipped.Remove(routeToRemove);
        Routes.Remove(routeToRemove);
        CalculateSpline();
    }

    private void AddRouteAndCalculateStations(Route newRoute) {
        bool areStartStationsSame = newRoute.StartStation.X == StartStation.X && newRoute.StartStation.Y == StartStation.Y;
        bool areEndStationsSame = newRoute.EndStation.X == EndStation.X && newRoute.EndStation.Y == EndStation.Y;
        bool isNewStartOldEnd = newRoute.StartStation.X == EndStation.X && newRoute.StartStation.Y == EndStation.Y;
        bool isNewEndOldStart = newRoute.EndStation.X == StartStation.X && newRoute.EndStation.Y == StartStation.Y;

        if (!areStartStationsSame && !areEndStationsSame && !isNewStartOldEnd && !isNewEndOldStart) {
            Debug.LogError("Tried to add a route that didnt connect to the line");
            return;
        }

        // if they both start at the same station
        if (areStartStationsSame) {
            Debug.Log("areStartStationsSame");
            Routes.Insert(0, newRoute);

            // and flip its spline
            _isRouteFlipped.Add(newRoute, true);

            // since we added at the start and flipped, its end is the new start
            StartStation = newRoute.EndStation;

            return;
        }

        if (areEndStationsSame) {
            Debug.Log("areEndStationsSame");
            Routes.Add(newRoute);

            // and flip its spline
            _isRouteFlipped.Add(newRoute, true);

            // since we added at the end and flipped, its end is the new start
            EndStation = newRoute.StartStation;

            return;
        }

        if (isNewStartOldEnd) {
            Debug.Log("isNewStartOldEnd");

            // make the new route the last route
            Routes.Add(newRoute);

            _isRouteFlipped.Add(newRoute, false);

            EndStation = newRoute.EndStation;

            return;
        }

        if (isNewEndOldStart) {
            Debug.Log("isNewEndOldStart");

            // make the new route the first route
            Routes.Insert(0, newRoute);

            _isRouteFlipped.Add(newRoute, false);

            StartStation = newRoute.StartStation;

            return;
        }
    }

    public void CalculateSpline() {
        Spline newSpline = new Spline();

        Routes.ForEach(route => {
            newSpline.Add(_isRouteFlipped[route] ? route.RouteSplineReversed : route.RouteSpline);
        });

        Spline = newSpline;

        OnSplineChange?.Invoke(newSpline);
    }

    public void AddTrain(string startingStation, TrainController newTrain) {
        Trains.Add(newTrain);
        OnTrainCountChange?.Invoke(Trains.Count);
    }

    public void RemoveTrain(TrainController trainToRemove) {
        Trains.Remove(trainToRemove);
        OnTrainCountChange?.Invoke(Trains.Count);
    }

    public List<TrainController> RemoveAllTrains() {
        List<TrainController> removedTrains = new List<TrainController>(Trains);

        removedTrains.ForEach(train => {
            RemoveTrain(train);
        });

        return removedTrains;
    }
}

public class LineManager : Singleton<LineManager> {

    public static List<Color> LINE_COLORS = new List<Color> {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    public Dictionary<Color, Line> Lines { get; private set; }

    public event Action<Color, Line> OnLineAdded;

    public event Action<Color> OnLineRemoved;

    private void Awake() {
        Lines = new Dictionary<Color, Line>();

        LINE_COLORS.ForEach(color => {
            Line newLine = new Line(color);
            Lines.Add(color, newLine);
        });
    }

    public void HandleRouteClicked(OSRouteController routeController) {
        // is it already in a line?
        Color newLineColor = FindNextPossibleLineForRoute(routeController.Route);

        if (newLineColor == Color.white) {
            Debug.LogError("Route cannot be added to any lines");
            return;
        }

        AddRouteToLine(newLineColor, routeController);
    }

    private Color FindNextPossibleLineForRoute(Route route) {
        int currentIndex = -1;
        int iterations = 0;
        // is it already in a line?
        bool isAlreadyInALine = Lines.Where(kvp => kvp.Value.Routes.Contains(route)).Count() > 0;

        // is it clicked for the first time?
        if (isAlreadyInALine) {
            Color existingColor = Lines.Where(kvp => kvp.Value.Routes.Contains(route)).FirstOrDefault().Key;
            currentIndex = LINE_COLORS.IndexOf(existingColor);
        }

        // start looking at the next one
        currentIndex = (currentIndex + 1) % LINE_COLORS.Count;

        Color newColor = Color.white;
        bool foundNewLine = false;

        while (iterations < LINE_COLORS.Count && foundNewLine == false) {
            Color currentColor = LINE_COLORS[currentIndex];

            if (Lines[currentColor].CanAddRoute(route)) {
                newColor = currentColor;
                foundNewLine = true;
            }

            iterations++;
            currentIndex++;
        }

        return newColor;
    }

    private void AddRouteToLine(Color lineColor, OSRouteController routeController) {
        bool isAlreadyInALine = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).Count() > 0;

        if (isAlreadyInALine) {
            Color existingColor = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).FirstOrDefault().Key;
            RemoveRouteFromLine(existingColor, routeController);
        }

        Lines[lineColor].AddRoute(routeController.Route);
        routeController.UpdateRouteColor(lineColor);

        if (Lines[lineColor].Routes.Count == 1) {
            OnLineAdded?.Invoke(lineColor, Lines[lineColor]);
            //TEMP
            TrainController train = TrainManager.Instance.SpawnTrain("startingStation", Lines[lineColor]);
            Lines[lineColor].AddTrain("temp", train);
            //
        }
    }

    private void RemoveRouteFromLine(Color lineColor, OSRouteController routeController) {
        Lines[lineColor].RemoveRoute(routeController.Route);
        routeController.UpdateRouteColor(Color.white);

        if (Lines[lineColor].Routes.Count == 0) {
            OnLineRemoved?.Invoke(lineColor);
            //TEMP
            var removedTrains = Lines[lineColor].RemoveAllTrains();
            TrainManager.Instance.DestroyTrains(removedTrains);
            //
        }
    }

    public Color GetLineByRoute(OSRouteController routeController) {
        bool isAlreadyInALine = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).Count() > 0;

        if (isAlreadyInALine) {
            Color existingColor = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).FirstOrDefault().Key;
            return existingColor;
        }

        return Color.white;
    }
}