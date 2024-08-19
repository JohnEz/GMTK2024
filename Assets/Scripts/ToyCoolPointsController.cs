using TMPro;
using UnityEngine;

public class ToyCoolPointsController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tmp;

    void Awake() {
        CoolManager.Instance.OnCoolnessUpdate += OnCoolnessUpdate;
    }

    private void OnCoolnessUpdate(int value, int diff)
    {
        tmp.text = value.ToString("N0");

        // TODO: Not working currently
        FloatingTextManager.Instance.Show($"{(diff > 0 ? "+" : "")}{value:N0}", gameObject);
    }
}
