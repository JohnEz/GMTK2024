using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Line {
    public Color Color { get; private set; }

    public List<Route> Routes { get; private set; }

    public List<TrainController> Trains { get; private set; }

    public event Action<int> OnTrainCountChange;

    public Line(Color color) {
        Routes = new List<Route>();
        Trains = new List<TrainController>();
        Color = color;
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
        bool isAlreadyInALine = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).Count() > 0;
        Color newColor = LINE_COLORS[0];

        // is it clicked for the first time?
        if (isAlreadyInALine) {
            Color existingColor = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).FirstOrDefault().Key;
            int currentIndex = LINE_COLORS.IndexOf(existingColor);
            int newIndex = (currentIndex + 1) % LINE_COLORS.Count;
            newColor = LINE_COLORS[newIndex];
        }

        AddRouteToLine(newColor, routeController);
    }

    private void AddRouteToLine(Color lineColor, OSRouteController routeController) {
        bool isAlreadyInALine = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).Count() > 0;

        if (isAlreadyInALine) {
            Color existingColor = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).FirstOrDefault().Key;
            RemoveRouteFromLine(existingColor, routeController);
        }

        Lines[lineColor].Routes.Add(routeController.Route);
        routeController.UpdateRouteColor(lineColor);

        if (Lines[lineColor].Routes.Count == 1) {
            OnLineAdded?.Invoke(lineColor, Lines[lineColor]);
            //TEMP
            TrainController train = TrainManager.Instance.SpawnTrain("startingStation", routeController, transform);
            Lines[lineColor].AddTrain("temp", train);
            //
        }
    }

    private void RemoveRouteFromLine(Color lineColor, OSRouteController routeController) {
        Lines[lineColor].Routes.Remove(routeController.Route);
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