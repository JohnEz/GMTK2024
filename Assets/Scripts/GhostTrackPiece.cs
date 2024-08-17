using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GhostTrackPiece : MonoBehaviour {
    public Action OnOk;

    void Start() {
        OnButton("ButtonOk", () => OnOk?.Invoke());
        OnButton("ButtonCancel", OnCancel);
    }

    void Update() {
    }

    void OnCancel() {
        Debug.Log("TODO: cancel: pop a tile or what?");
    }

    void OnButton(string name, UnityAction callback) {
        var canvas = transform.Find("Canvas");
        var btn = canvas.Find(name).GetComponent<Button>();
        btn.onClick.AddListener(callback);
    }

    public TrackPiece IntoTrackPiece() {
        // TODO: move this to an instance variable,
        // and toggle properties on it (Rotation, Template) as the user presses buttons
        var newPiece = new TrackPiece() {
            X = this.transform.position.x,
            Y = this.transform.position.y,
            Rotation = Rotation.None,
        };

        // TODO: when newPiece is an instance variable, we return it here and
        // create a fresh one in its place for the next tile
        return newPiece;
    }
}
