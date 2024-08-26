using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdedWarning : MonoBehaviour {
    private CanvasGroup _canvasGroup;

    private bool _isShown = false;

    private Vector3 _basePosition;
    private Vector3 _baseScale;

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _throbAnimation;

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _isShown = false;
        _basePosition = transform.localPosition;
        _baseScale = transform.localScale;
    }

    private void Update() {
        bool isFailing = OSMapManager.Instance.Stations.Exists(station => station.IsOvercrowded);

        if (isFailing && !_isShown) {
            Show();
        } else if (!isFailing && _isShown) {
            Hide();
        }
    }

    private void Show() {
        _isShown = true;
        _canvasGroup.alpha = 1;

        transform.DOShakePosition(
            duration: 1f,
            strength: new Vector3(20f, 20f, 0),
            vibrato: 10,
            randomness: 90
        ).OnComplete(() => transform.localPosition = _basePosition);

        transform.DOScale(new Vector3(3, 3, 3), .5f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => {
                transform.localScale = _baseScale;

                _throbAnimation = transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1f)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }

    private void Hide() {
        _isShown = false;
        _canvasGroup.DOFade(0, .5f);

        if (_throbAnimation != null) {
            _throbAnimation.Kill();
            _throbAnimation = null;
        }
    }
}