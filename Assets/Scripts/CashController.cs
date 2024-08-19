using TMPro;
using UnityEngine;

public class CashController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tmp;

    void Awake() {
        BankManager.Instance.OnCashUpdate += OnCashUpdate;
    }

    private void OnCashUpdate(decimal value, decimal diff)
    {

        tmp.text = value.ToString("N2");

        // TODO: Not working currently
        FloatingTextManager.Instance.Show($"{(diff > 0 ? "+" : "")}{diff:N2}", gameObject);
    }
}
