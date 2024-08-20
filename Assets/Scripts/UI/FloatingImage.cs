using DG.Tweening;
using UnityEngine;

public class FloatingImage : MonoBehaviour {

    public void Start() {
        transform.DOLocalMoveY(30, 5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }
}