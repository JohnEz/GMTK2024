using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OSLineUIController : MonoBehaviour {
    private Line _line;

    [SerializeField]
    private Image _lineImage;

    [SerializeField]
    private TMP_Text _trainCountText;

    [SerializeField]
    private TMP_Text _usageText;

    public void SetLine(Color color, Line line) {
        if (_line != null) {
            _line.OnTrainCountChange -= HandleTrainCountChange;
        }

        _line = line;

        _lineImage.color = color;
        HandleTrainCountChange(_line.Trains.Count);

        _line.OnTrainCountChange += HandleTrainCountChange;
    }

    private void HandleTrainCountChange(int count) {
        _trainCountText.text = count.ToString();
    }
}