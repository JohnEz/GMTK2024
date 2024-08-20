using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager> {

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
    private void Start() {
        bool tutorialEnabled = SettingsManager.Instance.tutorialEnabled;
        if (tutorialEnabled) {
            StartTutorial();
        } else {
            PresetStationScheduler.Instance.StartSpawning();
        }
    }

    private void StartTutorial() {
        DisplayIntroduction();
        PassengerManager.Instance.PauseSpawning();
        GameStateManager.Instance.OnStateChange += HandleStateChange;

        ZoomManager.Instance.IsZoomingEnabled = false;
    }

    private void DisplayDialog(Speaker speaker, string dialog) {
        DialogManager.Instance.DisplayDialog(speaker, dialog);
    }

    private void HideDialog() {
        DialogManager.Instance.HideDialog();
    }

    private void HandleStateChange(GameState state) {
        if (state == GameState.TransitionToWork || state == GameState.TransitionToPlay) {
            HideDialog();
        }
    }

    private void DisplayIntroduction() {
        DisplayDialog(Speaker.Lenny, introduction);
        GameStateManager.Instance.OnStateChange += DisplayBuildTutorial;
    }

    private void DisplayBuildTutorial(GameState state) {
        if (state == GameState.KidEditing) {
            GameStateManager.Instance.OnStateChange -= DisplayBuildTutorial;
            DisplayDialog(Speaker.Lenny, buildTutorial);
            RouteManager.Instance.OnRouteAdded += DisplayScaleUpTutorial;
        }
    }

    private void DisplayScaleUpTutorial(Route route) {
        _firstRoute = route;
        RouteManager.Instance.OnRouteAdded -= DisplayScaleUpTutorial;
        DisplayDialog(Speaker.Lenny, scaleUpTutorial);
        GameStateManager.Instance.OnStateChange += DisplayLineTutorial;

        ZoomManager.Instance.IsZoomingEnabled = true;
    }

    private void DisplayLineTutorial(GameState state) {
        if (state == GameState.Adult) {
            GameStateManager.Instance.OnStateChange -= DisplayLineTutorial;
            DisplayDialog(Speaker.Terrence, lineTutorial);
            LineManager.Instance.OnLineAdded += DisplayReturnToKidTutorial;
        }
    }

    private void DisplayReturnToKidTutorial(Color color, Line line) {
        LineManager.Instance.OnLineAdded -= DisplayReturnToKidTutorial;
        PresetStationScheduler.Instance.StartSpawning();
        StationManager.Instance.OnStationAdded += StopStationsSpawning;
        DisplayDialog(Speaker.Terrence, returnToKidTutorial);
        GameStateManager.Instance.OnStateChange += ConnectStationTutorial;
    }

    private void StopStationsSpawning(TrackPiece track) {
        StationManager.Instance.OnStationAdded -= StopStationsSpawning;
        PresetStationScheduler.Instance.Pause();
    }

    private void ConnectStationTutorial(GameState state) {
        if (state == GameState.Kid) {
            GameStateManager.Instance.OnStateChange -= ConnectStationTutorial;
            BankManager.Instance.Spend(-1);
            DisplayDialog(Speaker.Lenny, connectStationTutorial);
            RouteManager.Instance.OnRouteAdded += ReturnToAdultTutorial;
        }
    }

    private void ReturnToAdultTutorial(Route route) {
        RouteManager.Instance.OnRouteAdded -= ReturnToAdultTutorial;
        DisplayDialog(Speaker.Lenny, returnToAdultTutorial);
        GameStateManager.Instance.OnStateChange += SecondLineTutorial;
    }

    private void SecondLineTutorial(GameState state) {
        if (state == GameState.Adult) {
            // GAME STARTS
            PassengerManager.Instance.ResumeSpawning();
            PresetStationScheduler.Instance.Resume();

            GameStateManager.Instance.OnStateChange -= SecondLineTutorial;
            DisplayDialog(Speaker.Terrence, secondLineTutorial);
            LineManager.Instance.OnRouteLineChange += ChangeLineTutorial;
        }
    }

    private void ChangeLineTutorial() {
        LineManager.Instance.OnRouteLineChange -= ChangeLineTutorial;
        DisplayDialog(Speaker.Terrence, changeLineTutorial);
        LineManager.Instance.OnRouteLineChange += Break;
    }

    private void Break() {
        LineManager.Instance.OnRouteLineChange -= Break;
        HideDialog();
        GameStateManager.Instance.OnStateChange += DisplayCoolPointsTutorial;
    }

    private void DisplayCoolPointsTutorial(GameState state) {
        if (state == GameState.Kid) {
            GameStateManager.Instance.OnStateChange -= DisplayCoolPointsTutorial;
            DisplayDialog(Speaker.Lenny, coolPointsTutorial);
            RouteManager.Instance.OnRouteAdded += DisplayMoneyTutorial;
        }
    }

    private void DisplayMoneyTutorial(Route route) {
        RouteManager.Instance.OnRouteAdded -= DisplayMoneyTutorial;
        DisplayDialog(Speaker.Lenny, moneyTutorial);
        GameStateManager.Instance.OnStateChange += DisplayAddTrainsTutorial;
    }

    private void DisplayAddTrainsTutorial(GameState state) {
        if (state == GameState.Adult) {
            GameStateManager.Instance.OnStateChange -= DisplayAddTrainsTutorial;
            DisplayDialog(Speaker.Terrence, addTrainsTutorial);
        }
    }
}