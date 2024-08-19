using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OSStation : MonoBehaviour {
    private const int MAX_PASSENGERS = 20;
    private const float PASSENGER_SPAWN_DELAY = 5f;
    private const float PASSENGER_SPAWN_DELAY_DECAY = .1f;
    private const float MIN_PASSENGER_SPAWN_DELAY = .2f;

    public List<OSPassenger> Passengers { get; private set; } = new List<OSPassenger>();

    public TrackPieceController TrackPieceController { get; private set; }

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text passengerCountText;

    private float _currentSpawnDelay;
    private float _timeSincePassengerSpawn = 0f;

    private void Awake() {
        TrackPieceController = GetComponent<TrackPieceController>();
    }

    public void Setup(string name) {
        gameObject.name = name;
        titleText.text = name;

        _currentSpawnDelay = PASSENGER_SPAWN_DELAY;
        _timeSincePassengerSpawn = Random.Range(0, _currentSpawnDelay);

        UpdatePassengerCount();
    }

    private void Update() {
        _timeSincePassengerSpawn += Time.deltaTime;
        if (_timeSincePassengerSpawn > _currentSpawnDelay) {
            PassengerManager.Instance.SpawnPassenger(this);
            _timeSincePassengerSpawn -= _currentSpawnDelay;

            _currentSpawnDelay = Mathf.Max(_currentSpawnDelay - PASSENGER_SPAWN_DELAY_DECAY, MIN_PASSENGER_SPAWN_DELAY);
        }
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