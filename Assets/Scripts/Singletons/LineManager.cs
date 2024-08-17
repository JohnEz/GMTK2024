using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Line {

    public Line() {
        Routes = new List<Route>();
    }

    public List<Route> Routes { get; private set; }

    public Color Color { get; private set; }
}

public class LineManager : Singleton<LineManager> {

    public static List<Color> LINE_COLORS = new List<Color> {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    public Dictionary<Color, Line> Lines { get; private set; }

    private void Awake() {
        Lines = new Dictionary<Color, Line>();

        LINE_COLORS.ForEach(color => {
            Line newLine = new Line();
            Lines.Add(color, newLine);
        });
    }

    public void HandleRouteClicked(OSRouteController routeController) {
        // is it already in a line?
        bool isAlreadyInALine = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).Count() > 0;

        // is it clicked for the first time?
        if (!isAlreadyInALine) {
            Color lineColor = LINE_COLORS[0];
            Lines[lineColor].Routes.Add(routeController.Route);
            routeController.UpdateRouteColor(lineColor);
            return;
        }

        Color existingColor = Lines.Where(kvp => kvp.Value.Routes.Contains(routeController.Route)).FirstOrDefault().Key;
        int currentIndex = LINE_COLORS.IndexOf(existingColor);
        int newIndex = (currentIndex + 1) % LINE_COLORS.Count;
        Color newColor = LINE_COLORS[newIndex];

        Lines[existingColor].Routes.Remove(routeController.Route);
        Lines[newColor].Routes.Add(routeController.Route);
        routeController.UpdateRouteColor(newColor);
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
}