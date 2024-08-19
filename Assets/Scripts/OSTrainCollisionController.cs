using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTrainCollisionController : MonoBehaviour {
    private OSStation _lastStation;

    private void OnCollisionEnter2D(Collision2D collision) {
        OSStation station = collision.gameObject.GetComponent<OSStation>();

        if (station == null || station == _lastStation) {
            return;
        }

        Debug.Log($"{name} arrived at station {station.name}");
        _lastStation = station;
    }
}