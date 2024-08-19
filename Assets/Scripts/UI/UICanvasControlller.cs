using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICanvasControlller : MonoBehaviour {

    public void ShowUI() {
        GetComponentsInChildren<HideableUIComponent>().ToList().ForEach(hideableComponent => {
            hideableComponent.Show();
        });
    }

    public void HideUI() {
        GetComponentsInChildren<HideableUIComponent>().ToList().ForEach(hideableComponent => {
            hideableComponent.Hide();
        });
    }
}