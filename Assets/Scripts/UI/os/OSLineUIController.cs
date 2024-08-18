using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OSLineUIController : MonoBehaviour {
    private Line _line;

    [SerializeField]
    private Image _lineImage;

    [SerializeField]
    private TMPro.TextMeshPro _trainCountText;

    [SerializeField]
    private TMPro.TextMeshPro _usageText;

    public void SetLine(Color color, Line line) {
        _line = line;

        _lineImage.color = color;
    }
}