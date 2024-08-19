using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OSStation : MonoBehaviour {
    private const int MAX_PASSENGERS = 20;

    public List<OSPassenger> Passengers { get; private set; } = new List<OSPassenger>();

    public TrackPieceController TrackPieceController { get; private set; }

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text passengerCountText;

    private void Awake() {
        TrackPieceController = GetComponent<TrackPieceController>();
    }

    public void Setup(string name) {
        gameObject.name = name;
        titleText.text = name;

        UpdatePassengerCount();
    }

    public void AddPassenger(OSPassenger newPassenger) {
        Passengers.Add(newPassenger);
        newPassenger.transform.SetParent(transform, false);
        newPassenger.transform.localPosition = Vector3.zero;

        UpdatePassengerCount();
    }

    public void RemovePassenger(OSPassenger newPassenger) {
        Passengers.Remove(newPassenger);

        UpdatePassengerCount();
    }

    public void UpdatePassengerCount() {
        passengerCountText.text = $"{Passengers.Count}/{MAX_PASSENGERS}";
    }
}