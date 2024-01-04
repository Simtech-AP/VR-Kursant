using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PointArrowHandler : MonoBehaviour
{
    [SerializeField]
    private Transform objectToAnimate;

    [SerializeField]
    private float dislocation = default;

    [SerializeField]
    private float animationTime = default;

    private Sequence animation = default;

    public void InitializeDelayed(int delay)
    {
        StartCoroutine(delayInit(delay));
    }

    private IEnumerator delayInit(int delay)
    {
        yield return new WaitForSeconds(delay);
        Initialize();
    }

    public void Initialize()
    {
        objectToAnimate.gameObject.SetActive(true);


        animation = DOTween.Sequence();

        var forwardMove = objectToAnimate.DOLocalMoveY(dislocation, animationTime).SetEase(Ease.InOutSine);
        var returnMove = objectToAnimate.DOLocalMoveY(0, animationTime).SetEase(Ease.InOutSine);
        animation.Append(forwardMove);
        animation.Append(returnMove);

        animation.SetLoops(-1);

        animation.Play();
    }

    public void CleanUp()
    {
        animation.Kill();
        gameObject.SetActive(false);
    }
}
