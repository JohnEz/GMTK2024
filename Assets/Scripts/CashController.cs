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
        BankManager.Instance.OnCashInit += SetDisplayedCash;
        BankManager.Instance.OnCashUpdate += OnCashUpdate;
        SetDisplayedCash(BankManager.Instance.Cash);
    }

    private void SetDisplayedCash(decimal value)
    {
        decimal displayValue = IsAdult ? BankManager.Instance.GetAdultValue(value) : value;
        // Debug.Log($"Displaying cash: {value}, {IsAdult}, {displayValue}, {displayValue.ToString(Format)}");
        tmp.text = displayValue.ToString(Format);
    }

    private void OnCashUpdate(decimal value, decimal diff)
    {
        SetDisplayedCash(value);

        if (ShowDiff) {
            decimal displayDiff = IsAdult ? BankManager.Instance.GetAdultValue(diff) : diff;
            string formattedDif = displayDiff.ToString(Format);
            UIFloatingTextManager.Instance.Show($"{(diff > 0 ? "+" : "")}{formattedDif}", tmp.gameObject, down: true);
        }
    }

    private bool ShowDiff {
        get =>
            (IsAdult && GameStateManager.Instance.State == GameState.Adult)
            || (!IsAdult &&
                (GameStateManager.Instance.State == GameState.Kid || GameStateManager.Instance.State == GameState.KidEditing)
            );
    }
}
