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

    [SerializeField]
    private HideableUIComponent _editUI;

    public GameState State => _State;

    public event Action<GameState> OnStateChange;

    [SerializeField]
    private GameOverMenu _gameOverMenu;

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

        if (state == GameState.KidEditing && State != GameState.Kid) {
            return false;
        }
        

        GameState previousState = _State;
        _State = state;
        OnStateChange.Invoke(state);

        // showing and hiding the edit ui
        if (_State == GameState.KidEditing) {
            _editUI.Show();
        } else if (previousState == GameState.KidEditing) {
            _editUI.Hide();
        }

        return true;
    }

    public void GameOver(OSStation station) {
        if (_State == GameState.GameOver) {
            return;
        }

        _State = GameState.GameOver;

        _gameOverMenu?.Show(station.name, BankManager.Instance.TotalCash, PassengerManager.Instance.CompletedJournyCount, CoolManager.Instance.Coolness);
    }
}