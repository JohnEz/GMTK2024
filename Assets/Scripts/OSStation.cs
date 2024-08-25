using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OSStation : MonoBehaviour {
    private const int MAX_PASSENGERS = 20;
    private const float PASSENGER_SPAWN_DELAY = 5f;
    private const float PASSENGER_SPAWN_DELAY_DECAY = .05f;
    private const float MIN_PASSENGER_SPAWN_DELAY = .2f;
    private const float SPAWN_DECAY_INTERVAL = 5f;

    private const float FAILURE_TIMER = 20f;

    private Color _defaultColor = Color.black;

    [SerializeField]
    private Color _failingColor = Color.red;

    public List<OSPassenger> Passengers { get; private set; } = new List<OSPassenger>();

    public TrackPieceController TrackPieceController { get; private set; }

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text passengerCountText;

    [SerializeField]
    private Image _failureCircle;

    [SerializeField]
    private SpriteRenderer _highlightCircle;

    private float _currentSpawnDelay;
    private float _timeSincePassengerSpawn = 0f;
    private float _timeSinceSpawnDecay = 0f;

    private float _timeFailing = 0f;

    private void Awake() {
        TrackPieceController = GetComponent<TrackPieceController>();
        _defaultColor = passengerCountText.color;

        Unhighlight();
    }

    public void Setup(string name) {
        gameObject.name = name;
        titleText.text = name;

        _currentSpawnDelay = PASSENGER_SPAWN_DELAY;
        _timeSincePassengerSpawn = Random.Range(0, _currentSpawnDelay);
        _timeFailing = 0;

        UpdatePassengerCount();
    }

    private void Update() {
        _timeSincePassengerSpawn += Time.deltaTime;
        if (_timeSincePassengerSpawn > _currentSpawnDelay) {
            _timeSincePassengerSpawn -= _currentSpawnDelay;
            PassengerManager.Instance.SpawnPassenger(this);
        }

        _timeSinceSpawnDecay += Time.deltaTime;
        if (_timeSinceSpawnDecay > SPAWN_DECAY_INTERVAL) {
            _timeSinceSpawnDecay -= SPAWN_DECAY_INTERVAL;
            _currentSpawnDelay = Mathf.Max(_currentSpawnDelay - PASSENGER_SPAWN_DELAY_DECAY, MIN_PASSENGER_SPAWN_DELAY);
        }

        if (Passengers.Count > MAX_PASSENGERS) {
            _timeFailing += Time.deltaTime;

            if (_timeFailing >= FAILURE_TIMER) {
                GameStateManager.Instance.GameOver(this);
            }
        } else if (_timeFailing > 0) {
            _timeFailing -= Time.deltaTime;
        }

        UpdateFailureCircle();
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

        bool isOverCrowded = Passengers.Count > MAX_PASSENGERS;

        passengerCountText.fontStyle = isOverCrowded ? FontStyles.Bold : FontStyles.Normal;
        passengerCountText.color = isOverCrowded ? _failingColor : _defaultColor;
    }

    public void UpdateFailureCircle() {
        _failureCircle.fillAmount = _timeFailing / FAILURE_TIMER;
    }

    public void Highlight() {
        _highlightCircle.enabled = true;
    }

    public void Unhighlight() {
        _highlightCircle.enabled = false;
    }
}