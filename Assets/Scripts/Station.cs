using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour {

    void Start() {
        foreach (Button button in Buttons()) {
            button.onClick.AddListener(() => StartEditing());
        }
    }

    void Update() {
    }

    void StartEditing() {
        RouteManager.Instance.StartEditing(this);
    }

    UnityEngine.UI.Button[] Buttons() {
        var canvas = transform.Find("StationCanvas");
        return canvas.GetComponentsInChildren<Button>();
    }
}
