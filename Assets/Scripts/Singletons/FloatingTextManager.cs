using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextManager : Singleton<FloatingTextManager>
{
    public GameObject textPrefab;

    public GameObject canvasPrefab;

    public void Show(string text, GameObject parent, float delaySeconds) {
        StartCoroutine(ShowAfterDelay(text, parent, delaySeconds));
    }

    public void Show(string text, GameObject parent) {
        // TODO: Canvas isn't cleaned up after animation
        GameObject canvasObject = Instantiate(canvasPrefab, transform);
        canvasObject.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + 1, 0);

        TMP_Text tmp = canvasObject.GetComponentInChildren<TMP_Text>();
        tmp.text = text;
    }

    private IEnumerator ShowAfterDelay(string text, GameObject parent, float delaySeconds) {
        yield return new WaitForSeconds(delaySeconds);

        Show(text, parent);
    }
}
