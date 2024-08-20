using TMPro;
using UnityEngine;

public class CashController : MonoBehaviour
{
    [SerializeField]
    private string Format;

    [SerializeField]
    private bool IsAdult;

    [SerializeField]
    private TMP_Text tmp;

    void Awake() {
        BankManager.Instance.OnCashUpdate += OnCashUpdate;
        OnCashUpdate(BankManager.Instance.Cash, 0);
    }

    private void OnCashUpdate(decimal value, decimal diff)
    {
        decimal displayValue = IsAdult ? BankManager.Instance.GetAdultValue(value) : value;
        Debug.Log($"Displaying cash: {value}, {IsAdult}, {displayValue}, {displayValue.ToString(Format)}");
        tmp.text = displayValue.ToString(Format);

        if (diff > 0) {
            decimal displayDiff = IsAdult ? BankManager.Instance.GetAdultValue(diff) : diff;
            string formattedDif = displayDiff.ToString(Format);
            UIFloatingTextManager.Instance.Show($"{(diff > 0 ? "+" : "")}{formattedDif}", tmp.gameObject);
        }
    }
}
