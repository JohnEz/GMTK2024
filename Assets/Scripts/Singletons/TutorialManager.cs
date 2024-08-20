using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial {
    public Speaker speaker;
    public string dialog;
}

public class TutorialManager : Singleton<TutorialManager>
{
    private void DisplayDialog(Tutorial tutorial) {
        DialogManager.Instance.DisplayDialog(tutorial.speaker, tutorial.dialog);
    }

    private void DisplayDialog(Speaker speaker, string dialog) {
        DialogManager.Instance.DisplayDialog(speaker, dialog);
    }

    private void HideDialog() {
        DialogManager.Instance.HideDialog();
    }


    // Start is called before the first frame update
    void Start()
    {
        // _tutorialIndex = startingIndex;
        // Tutorial tutorial = tutorials[_tutorialIndex];
        // DisplayDialog(tutorial);
        DisplayIntroduction();
        GameStateManager.Instance.OnStateChange += HandleStateChange;
    }

    void HandleStateChange(GameState state) {
        if (state == GameState.TransitionToWork || state == GameState.TransitionToPlay) {
            HideDialog();
        }
    }

    void DisplayIntroduction() {
        string dialog = "intro dialog";
        DisplayDialog(Speaker.Lenny, dialog);
        RouteManager.Instance.OnRouteAdded += DisplayScaleTutorial;
        // Wait for first route, then DisplayScaleTutorial
    }

    void DisplayScaleTutorial(Route route) {
        RouteManager.Instance.OnRouteAdded -= DisplayScaleTutorial;
        HideDialog();
        string dialog = "push the scale button (or SPACE)";
        DisplayDialog(Speaker.Lenny, dialog);
        // Wait for arriving in adult mode, then DisplayLineTutorial
        GameStateManager.Instance.OnStateChange += DisplayLineTutorial;
    }

    void DisplayLineTutorial(GameState state) {
        if (state == GameState.Adult) {
            GameStateManager.Instance.OnStateChange -= DisplayLineTutorial;
            string dialog = "click on a route to make a line";
            DisplayDialog(Speaker.Terrence, dialog);
            GameStateManager.Instance.OnStateChange += DisplayReturnToKidTutorial;
            // Wait for making a line
        }

    }

    void DisplayReturnToKidTutorial(GameState state) {
        // Trigger new station spawn
        // Show scale button
        string dialog = "click on the scale button to return to kid mode";
        DisplayDialog(Speaker.Terrence, dialog);
        // Wait for return to kid mode, trigger cool mode tutorial
    }

    void DisplayCoolModeTutorial() {
        string dialog = "get more cool points";
        DisplayDialog(Speaker.Terrence, dialog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
