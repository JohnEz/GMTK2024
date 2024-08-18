using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Line {

    public Line(Color color) {
        Routes = new List<Route>();
        Trains = new List<TrainController>();
        Color = color;
    }

    public Color Color { get; private set; }

    public List<Route> Routes { get; private set; }

    public List<TrainController> Trains { get; private set; }

    public void AddTrain(string startingStation, TrainController newTrain) {
        Trains.Add(newTrain);
    }

    public void RemoveTrain(TrainController trainToRemove) {
        Trains.Remove(trainToRemove);
    }

    public void RemoveAllTrains() {
        Trains.ForEach(train => {
            RemoveTrain(train);
        });
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

    // returns true when a new line was created
    private bool CreateNewLine(Route initialRoute) {
        Color newLineColor = Lines.Where(kvp => kvp.Value.Routes.Count > 0).First().Key;

        if (newLineColor == null) {
            return false;
        }

        Lines[newLineColor].Routes.Add(initialRoute);

        return true;
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
            //Lines[lineColor].AddTrain("temp");
            //
        }
    }

    private void RemoveRouteFromLine(Color lineColor, OSRouteController routeController) {
        Lines[lineColor].Routes.Remove(routeController.Route);
        routeController.UpdateRouteColor(Color.white);

        if (Lines[lineColor].Routes.Count == 0) {
            OnLineRemoved?.Invoke(lineColor);
            //TEMP
            //Lines[lineColor].RemoveTrain();
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