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
    public string coolPointsTutorial;

    // Start is called before the first frame update
    void Start()
    {
        DisplayIntroduction();
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
        RouteManager.Instance.OnRouteAdded += DisplayScaleUpTutorial;
    }

    void DisplayScaleUpTutorial(Route route) {
        RouteManager.Instance.OnRouteAdded -= DisplayScaleUpTutorial;
        HideDialog();
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
        HideDialog();
        PresetStationScheduler.Instance.StartSpawning();      
        DisplayDialog(Speaker.Terrence, returnToKidTutorial);
        GameStateManager.Instance.OnStateChange += DisplayCoolPointsTutorial;
    }

    void DisplayCoolPointsTutorial(GameState state) {
        if (state == GameState.Kid) {
            GameStateManager.Instance.OnStateChange -= DisplayCoolPointsTutorial;
            DisplayDialog(Speaker.Lenny, coolPointsTutorial);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
