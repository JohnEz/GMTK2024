using System;
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
    public GameState InitialState = GameState.Kid;

    private GameState _State;

    [SerializeField]
    private AudioClip _kidMusic;

    [SerializeField]
    private AudioClip _adultMusic;

    public GameState State => _State;

    public event Action<GameState> OnStateChange;

    void Awake()
    {
        _State = InitialState;
        AudioClipOptions options = new AudioClipOptions();
        options.Delay = 1.5f;
        if (InitialState == GameState.Kid || InitialState == GameState.KidEditing)
        {
            AudioManager.Instance.PlaySound(_kidMusic, options);
        }
        else if (InitialState == GameState.Adult)
        {
            AudioManager.Instance.PlaySound(_adultMusic, options);
        }
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

        GameState previousState = _State;
        _State = state;
        OnStateChange.Invoke(state);

        AudioClipOptions options = new AudioClipOptions();
        options.Delay = 1.5f;
        if (previousState == GameState.TransitionToPlay) {
            AudioManager.Instance.StopSound(_adultMusic);
            AudioManager.Instance.PlaySound(_kidMusic, options);
        }
        else if (previousState == GameState.TransitionToWork)
        {
            AudioManager.Instance.StopSound(_kidMusic);
            AudioManager.Instance.PlaySound(_adultMusic, options);
        }
        return true;
    }
}
