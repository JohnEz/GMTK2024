using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [TextAreaAttribute]
    public string introduction;

    [TextAreaAttribute]
    public string buildTutorial;

    [TextAreaAttribute]
    public string scaleUpTutorial;

    [TextAreaAttribute]
    public string lineTutorial;

    [TextAreaAttribute]
    public string returnToKidTutorial;

    [TextAreaAttribute]
    public string connectStationTutorial;

    [TextAreaAttribute]
    public string returnToAdultTutorial;

    [TextAreaAttribute]
    public string secondLineTutorial;

    [TextAreaAttribute]
    public string changeLineTutorial;

    [TextAreaAttribute]
    public string coolPointsTutorial;

    [TextAreaAttribute]
    public string moneyTutorial;

    [TextAreaAttribute]
    public string addTrainsTutorial;

    private Route _firstRoute;

    // Start is called before the first frame update
    void Start()
    {
        bool tutorialEnabled = SettingsManager.Instance.tutorialEnabled;
        if (tutorialEnabled) {
            StartTutorial();
        } else {
            PresetStationScheduler.Instance.StartSpawning();
        }

    }

    void StartTutorial() {
        DisplayIntroduction();
        PassengerManager.Instance.PauseSpawning();
        GameStateManager.Instance.OnStateChange += HandleStateChange;
    }

    void DisplayDialog(Speaker speaker, string dialog) {
        DialogManager.Instance.DisplayDialog(speaker, dialog);
    }

    void HideDialog() {
        DialogManager.Instance.HideDialog();
    }

    void HandleStateChange(GameState state) {
        if (state == GameState.TransitionToWork || state == GameState.TransitionToPlay) {
            HideDialog();
        }
    }

    void DisplayIntroduction() {
        DisplayDialog(Speaker.Lenny, introduction);
        GameStateManager.Instance.OnStateChange += DisplayBuildTutorial;
    }

    void DisplayBuildTutorial(GameState state) {
        if (state == GameState.KidEditing) {
            GameStateManager.Instance.OnStateChange -= DisplayBuildTutorial;
            DisplayDialog(Speaker.Lenny, buildTutorial);
            RouteManager.Instance.OnRouteAdded += DisplayScaleUpTutorial;
        }
    }

    void DisplayScaleUpTutorial(Route route) {
        _firstRoute = route;
        RouteManager.Instance.OnRouteAdded -= DisplayScaleUpTutorial;
        DisplayDialog(Speaker.Lenny, scaleUpTutorial);
        GameStateManager.Instance.OnStateChange += DisplayLineTutorial;
    }

    void DisplayLineTutorial(GameState state) {
        if (state == GameState.Adult) {
            GameStateManager.Instance.OnStateChange -= DisplayLineTutorial;
            DisplayDialog(Speaker.Terrence, lineTutorial);
            LineManager.Instance.OnLineAdded += DisplayReturnToKidTutorial;
        }
    }

    void DisplayReturnToKidTutorial(Color color, Line line) {
        LineManager.Instance.OnLineAdded -= DisplayReturnToKidTutorial;
        PresetStationScheduler.Instance.StartSpawning();
        StationManager.Instance.OnStationAdded += StopStationsSpawning;      
        DisplayDialog(Speaker.Terrence, returnToKidTutorial);
        GameStateManager.Instance.OnStateChange += ConnectStationTutorial;
    }

    void StopStationsSpawning(TrackPiece track) {
        StationManager.Instance.OnStationAdded -= StopStationsSpawning;     
        PresetStationScheduler.Instance.Pause();
    }

    void ConnectStationTutorial(GameState state) {
        if (state == GameState.Kid) {
            GameStateManager.Instance.OnStateChange -= ConnectStationTutorial;
            BankManager.Instance.Spend(-1);
            DisplayDialog(Speaker.Lenny, connectStationTutorial);
            RouteManager.Instance.OnRouteAdded += ReturnToAdultTutorial;
        }
    }

    void ReturnToAdultTutorial(Route route) {
        RouteManager.Instance.OnRouteAdded -= ReturnToAdultTutorial;
        DisplayDialog(Speaker.Lenny, returnToAdultTutorial);
        GameStateManager.Instance.OnStateChange += SecondLineTutorial;
    }

    void SecondLineTutorial(GameState state) {
        if (state == GameState.Adult) {
            // GAME STARTS
            PassengerManager.Instance.ResumeSpawning();
            PresetStationScheduler.Instance.Resume();

            GameStateManager.Instance.OnStateChange -= SecondLineTutorial;
            DisplayDialog(Speaker.Terrence, secondLineTutorial);
            LineManager.Instance.OnRouteLineChange += ChangeLineTutorial;
        }
    }

    void ChangeLineTutorial() {
            LineManager.Instance.OnRouteLineChange -= ChangeLineTutorial;
            DisplayDialog(Speaker.Terrence, changeLineTutorial);
            LineManager.Instance.OnRouteLineChange += Break;
    }

    void Break() {
        LineManager.Instance.OnRouteLineChange -= Break;
        HideDialog();
        GameStateManager.Instance.OnStateChange += DisplayCoolPointsTutorial;
    }

    void DisplayCoolPointsTutorial(GameState state) {
        if (state == GameState.Kid) {
            GameStateManager.Instance.OnStateChange -= DisplayCoolPointsTutorial;
            DisplayDialog(Speaker.Lenny, coolPointsTutorial);
            RouteManager.Instance.OnRouteAdded += DisplayMoneyTutorial;
        }
    }

    void DisplayMoneyTutorial(Route route) {
        RouteManager.Instance.OnRouteAdded -= DisplayMoneyTutorial;
        DisplayDialog(Speaker.Lenny, moneyTutorial);
        GameStateManager.Instance.OnStateChange += DisplayAddTrainsTutorial;
    }

    void DisplayAddTrainsTutorial(GameState state) {
        if (state == GameState.Adult) {
            GameStateManager.Instance.OnStateChange -= DisplayAddTrainsTutorial;
            DisplayDialog(Speaker.Terrence, addTrainsTutorial);
        }
    }
}
