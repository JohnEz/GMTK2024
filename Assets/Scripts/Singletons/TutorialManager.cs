using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial {
    public Speaker speaker;
    public string dialog;
}

public class TutorialManager : Singleton<TutorialManager>
{
    private int _tutorialIndex = 0;
    public List<Tutorial> tutorials;
    public int startingIndex = 0;

    public void StepForward() {
        _tutorialIndex += 1;
        Tutorial nextTutorial = tutorials[_tutorialIndex];
        DisplayDialog(nextTutorial);
    }

    private void DisplayDialog(Tutorial tutorial) {
        DialogManager.Instance.DisplayDialog(tutorial.speaker, tutorial.dialog);
    }

    private void DisplayDialog(Speaker speaker, string dialog) {
        DialogManager.Instance.DisplayDialog(speaker, dialog);
    }


    // Start is called before the first frame update
    void Start()
    {
        // _tutorialIndex = startingIndex;
        // Tutorial tutorial = tutorials[_tutorialIndex];
        // DisplayDialog(tutorial);
        DisplayIntroduction();
    }

    void DisplayIntroduction() {
        string dialog = "intro dialog";
        DisplayDialog(Speaker.Lenny, dialog);
        RouteManager.Instance.OnRouteAdded += DisplayScaleTutorial;
        // Wait for first route, then DisplayScaleTutorial
    }

    void DisplayScaleTutorial(Route route) {
        string dialog = "push the scale button (or SPACE)";
        DisplayDialog(Speaker.Lenny, dialog);
        // Wait for arriving in adult mode, then DisplayScaleTutorial
    }

    void DisplayLineTutorial() {
        string dialog = "click on a route to make a line";
        DisplayDialog(Speaker.Terrence, dialog);
        // Wait for making a line
    }

    void DisplayReturnToKidTutorial() {
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
