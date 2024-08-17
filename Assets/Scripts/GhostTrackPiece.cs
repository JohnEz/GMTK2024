using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GhostTrackPiece : MonoBehaviour {
    void Start() {
        OnButton("ButtonOk", OnOk);
        OnButton("ButtonCancel", OnCancel);
    }

    void Update() {
    }

    void OnOk() {
        Debug.Log("OnOk()");
    }

    void OnCancel() {
        Debug.Log("OnCancel()");
    }

    void OnButton(string name, UnityAction callback) {
        var canvas = transform.Find("Canvas");
        var btn = canvas.Find(name).GetComponent<Button>();
        btn.onClick.AddListener(callback);
    }
}
