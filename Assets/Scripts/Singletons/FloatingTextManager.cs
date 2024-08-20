using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextManager<T> : Singleton<T> where T : MonoBehaviour
{
    public GameObject canvasPrefab;

    public void Show(string text, GameObject parent, float delaySeconds, bool down = false) {
        StartCoroutine(ShowAfterDelay(text, parent, delaySeconds, down));
    }

    public void Show(string text, GameObject parent, bool down = false) {
        // TODO: Canvas isn't cleaned up after animation
        GameObject canvasObject = Instantiate(canvasPrefab, transform);
        canvasObject.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + (down ? -1 : 1), 0);

        TMP_Text tmp = canvasObject.GetComponentInChildren<TMP_Text>();
        tmp.text = text;

        Animator animator = canvasObject.GetComponentInChildren<Animator>();
        animator.Play(down ? "FadeDown" : "FadeUp");
    }

    private IEnumerator ShowAfterDelay(string text, GameObject parent, float delaySeconds, bool down = false) {
        yield return new WaitForSeconds(delaySeconds);

        Show(text, parent, down);
    }
}
