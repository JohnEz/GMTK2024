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
    private AudioClip _clickClip;

    [SerializeField]
    private AudioClip _failClip;

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

    public void AddTrainClicked() {
        decimal trainCost = (decimal)2;

        if (BankManager.Instance.Cash > trainCost) {
            BankManager.Instance.Spend(.2f);
            TrainManager.Instance.SpawnTrain("station", _line);

            AudioManager.Instance.PlaySound(_clickClip);
        } else {
            decimal convertedTrainCost = BankManager.Instance.GetAdultValue(trainCost);
            UIFloatingTextManager.Instance.Show($"Trains cost\n£{convertedTrainCost}!", gameObject, true, true);

            AudioManager.Instance.PlaySound(_failClip);
        }
    }
}