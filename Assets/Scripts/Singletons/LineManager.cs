using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Line {
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

    public void RouteClicked() {
        // is it already in a line?

        // is it clicked for the first time?

        // should it join an existing line?

        // can it be removed from its line?
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