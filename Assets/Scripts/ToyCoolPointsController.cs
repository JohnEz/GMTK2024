using TMPro;
using UnityEngine;

public class ToyCoolPointsController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tmp;

    void Awake() {
        CoolManager.Instance.OnCoolnessUpdate += OnCoolnessUpdate;
    }

    private void OnCoolnessUpdate(int value)
    {
        tmp.text = value.ToString("#,#");
    }
}
