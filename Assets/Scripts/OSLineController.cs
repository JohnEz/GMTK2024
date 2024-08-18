using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class OSLineController : MonoBehaviour {
    public Line Line { get; private set; }

    public SplineContainer SplineContainer { get; private set; }

    public event Action OnChange;

    private void Awake() {
        SplineContainer = GetComponent<SplineContainer>();
    }

    public void SetLine(Line line) {
        if (Line != null) {
            Line.OnSplineChange -= HandleLineSplineChange;
        }

        Line = line;
        HandleLineSplineChange(Line.Spline);
        Line.OnSplineChange += HandleLineSplineChange;
    }

    private void HandleLineSplineChange(Spline newSpline) {
        SplineContainer.Spline = newSpline;
        OnChange?.Invoke();
    }
}