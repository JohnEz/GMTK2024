using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof (TrackPieceController))]
public class OSTrackController : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private SpriteRenderer _tileRenderer;

    void Awake() {
        _tileRenderer.sprite = _sprite;
    }

    public void UpdateTrackColor(Color newColor) {
        _tileRenderer.color = newColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
        GetComponentInParent<OSRouteController>().HandleClick();
    }

    private void OnValidate() {
        _tileRenderer.sprite = _sprite;
    }
}