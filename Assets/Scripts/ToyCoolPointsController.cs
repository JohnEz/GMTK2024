using TMPro;
using UnityEngine;

public class ToyCoolPointsController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tmp;

    void Awake() {
        CoolManager.Instance.OnCoolnessInit += SetDisplayedCoolness;
        CoolManager.Instance.OnCoolnessUpdate += OnCoolnessUpdate;
        SetDisplayedCoolness(CoolManager.Instance.Coolness);
    }

    private void SetDisplayedCoolness(int value)
    {
        tmp.text = value.ToString("N0");
    }

    private void OnCoolnessUpdate(int value, int diff)
    {
        SetDisplayedCoolness(value);

        UIFloatingTextManager.Instance.Show($"{(diff > 0 ? "+" : "")}{value:N0}", tmp.gameObject, down: true);
    }
}
