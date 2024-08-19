using TMPro;
using UnityEngine;

public class FloatingTextManager : Singleton<FloatingTextManager>
{
    public GameObject textPrefab;

    public GameObject canvasPrefab;

    public void Show(string text, GameObject parent) {
        Canvas canvas = parent.GetComponentInChildren<Canvas>();
        if (canvas == null) {
            GameObject canvasObject = Instantiate(canvasPrefab);
            canvasObject.transform.SetParent(parent.transform);

            canvas = canvasObject.GetComponent<Canvas>();
        }

        GameObject floatingText = Instantiate(textPrefab, parent.transform.position, Quaternion.identity);
        TMP_Text tmp = floatingText.GetComponent<TMP_Text>();
        tmp.text = text;
        tmp.transform.SetParent(canvas.transform, worldPositionStays: false);
    }
}
