using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTutorial : MonoBehaviour
{
    [SerializeField]
    private Image _offImage;

    private bool _isOff = false;

    private void Awake() {
        _isOff = AudioManager.Instance.IsMusicMuted;
        UpdateOffSprite();
    }

    public void ToggleTut() {
        SettingsManager.Instance.ToggleTutorial();
        _isOff = !_isOff;
        UpdateOffSprite();
    }

    public void UpdateOffSprite() {
        _offImage.enabled = _isOff;
    }
}
