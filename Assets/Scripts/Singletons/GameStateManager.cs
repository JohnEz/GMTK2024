using System;
using UnityEngine;
using UnityEngine.Events;

public enum GameState {
    Menu,
    Kid,
    KidEditing,
    TransitionToWork,
    Adult,
    TransitionToPlay,
    GameOver,
}

public class GameStateManager : Singleton<GameStateManager> {
    public GameState InitialState = GameState.Kid;

    private GameState _State;

    public GameState State => _State;

    public event Action<GameState> OnStateChange;

    [SerializeField]
    private GameOverMenu _gameOverMenu;

    [SerializeField]
    private GameOverMenu _gameWonMenu;

    private void Awake() {
        _State = InitialState;
    }

    public bool IsGameOver() {
        return _State == GameState.GameOver;
    }

    public bool TrySetState(GameState state) {
        if (state == GameState.GameOver) {
            return false;
        }

        if (state == _State) {
            return false;
        }

        if ((state == GameState.KidEditing || state == GameState.TransitionToWork) && State != GameState.Kid) {
            return false;
        }

        if (state == GameState.TransitionToPlay && State != GameState.Adult) {
            return false;
        }

        GameState previousState = _State;
        _State = state;
        OnStateChange.Invoke(state);

        return true;
    }

    public void GameOver(OSStation station) {
        if (_State == GameState.GameOver) {
            return;
        }

        _State = GameState.GameOver;

        _gameOverMenu.Show(false, station.name, BankManager.Instance.TotalCash, PassengerManager.Instance.CompletedJournyCount, CoolManager.Instance.Coolness);
    }

    public void GameWon(OSStation station) {
        if (_State == GameState.GameOver) {
            return;
        }

        _State = GameState.GameOver;

        _gameOverMenu.Show(true, station.name, BankManager.Instance.TotalCash, PassengerManager.Instance.CompletedJournyCount, CoolManager.Instance.Coolness);
    }
}