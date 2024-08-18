using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Menu,
    Kid,
    KidEditing,
    TransitionToWork,
    Adult,
    TransitionToPlay,
}

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState InitialState = GameState.Adult;

    private GameState _State;

    public GameState State => _State;

    public UnityEvent<GameState> OnStateChange = new();

    void Awake()
    {
        _State = InitialState;
    }

    public bool TrySetState(GameState state)
    {
        if (state == _State)
        {
            return false;
        }

        if (state == GameState.KidEditing && State != GameState.Kid) {
            return false;
        }

        Debug.Log($"Switching game state: from {_State} to {state}");
        _State = state;
        OnStateChange.Invoke(state);
        return true;
    }
}
