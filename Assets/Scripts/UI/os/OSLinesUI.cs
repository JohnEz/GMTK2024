using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OSLinesUI : MonoBehaviour {

    [SerializeField]
    private OSLineUIController _linePrefab;

    private Dictionary<Color, OSLineUIController> lines;

    private void Awake() {
        lines = new Dictionary<Color, OSLineUIController>();
    }

    private void Start() {
        CreateLineData();
    }

    private void OnEnable() {
        LineManager.Instance.OnLineAdded += AddLine;
        LineManager.Instance.OnLineRemoved += RemoveLine;
    }

    private void OnDisable() {
        if (LineManager.Instance != null) {
            LineManager.Instance.OnLineAdded -= AddLine;
            LineManager.Instance.OnLineRemoved -= RemoveLine;
        }
    }

    private void CreateLineData() {
        LineManager.Instance.Lines.Keys.ToList().ForEach(color => {
            Line line = LineManager.Instance.Lines[color];

            if (line.Routes.Count > 0) {
                AddLine(color, line);
            }
        });
    }

    private void AddLine(Color key, Line line) {
        OSLineUIController newLineUI = Instantiate(_linePrefab);
        newLineUI.transform.SetParent(transform, true);
        newLineUI.SetLine(key, line);

        lines.Add(key, newLineUI);
    }

    private void RemoveLine(Color key) {
        GameObject uiToDestroy = lines[key].gameObject;

        Destroy(uiToDestroy);

        lines.Remove(key);
    }
}