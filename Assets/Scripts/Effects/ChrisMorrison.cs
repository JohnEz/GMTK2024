using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//Blur manager
public class ChrisMorrison : Singleton<ChrisMorrison> {
    private float _targetBlur;
    private float _velocity = 0f;
    private float _smoothTime = 0.2f;

    private bool _isTransitioning = true;

    private Volume globalVolume;

    private BlurSettings blur;

    private void Awake() {
        globalVolume = GetComponent<Volume>();
        globalVolume.profile.TryGet(out blur);
    }

    private void Update() {
        if (_isTransitioning) {
            blur.Strength.value = Mathf.SmoothDamp(blur.Strength.value, _targetBlur, ref _velocity, _smoothTime);
        }

        if (_isTransitioning && Mathf.Abs(blur.Strength.value - _targetBlur) < 0.05f) {
            blur.Strength.value = _targetBlur;
            _isTransitioning = false;
        }
    }

    public void SetTargetBlur(float blurAmount, float blurTime) {
        _targetBlur = blurAmount;
        _smoothTime = blurTime;
        _isTransitioning = true;
    }

    public void SetInstantBlur(float blurAmount) {
        blur.Strength.value = blurAmount;
        _smoothTime = 0;
        _targetBlur = blurAmount;
    }
}