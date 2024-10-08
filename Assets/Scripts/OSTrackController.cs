using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TrackPieceController))]
public class OSTrackController : MonoBehaviour, IPointerDownHandler {

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private SpriteRenderer _tileRenderer;

    private void Awake() {
        _tileRenderer.sprite = _sprite;
    }

    public void UpdateTrackColor(Color newColor) {
        _tileRenderer.color = newColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
        RouteController oSRouteController = GetComponentInParent<RouteController>();
        if (oSRouteController == null) {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left) {
            oSRouteController.HandleClick();
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            oSRouteController.HandleRightClick();
        }
    }
}