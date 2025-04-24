using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class UIStretchTween : MonoBehaviour
{
     
    [Header("Tween Settings")]
    [SerializeField] private Ease firstTweenEase = Ease.OutQuad; 
    [SerializeField] private Ease secondTweenEase = Ease.InOutExpo; 
    [SerializeField] private float firstTweenDelay = 0.5f; 
    [SerializeField] private float secondTweenDelay = 1f;
    [SerializeField] private RectTransform UI_To_Tween;
    [SerializeField] private TextMeshProUGUI tmpText;

    public UnityEvent OnFinish;
    private bool canStartSecondSequence = false;
    private bool secondSequenceComplete = false; 

    private void Start()
    {
        StartCoroutine(StartSequence());
    }

    private void Update()
    {
        if (canStartSecondSequence && Input.anyKeyDown && !secondSequenceComplete)
        {
            StartCoroutine(SecondSequence());
            canStartSecondSequence = false;
        }

        if (secondSequenceComplete)
        {
            //FollowMouseWithDelay();
            OnFinish.Invoke();
        }
    }

    private IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(firstTweenDelay);

        Sequence seq = DOTween.Sequence();

        
        seq.Append(UI_To_Tween.DOScaleY(0.14f, 1f).SetEase(firstTweenEase)).
            OnUpdate(() =>
            {
                if (UI_To_Tween.localScale.y <= 0.14f)
                {
                    DOTween.To(() => tmpText.color, x => tmpText.color = x, new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 1f), 1f);
                }
            });
        
        seq.OnComplete(() => { canStartSecondSequence = true; });
    }

    private IEnumerator SecondSequence()
    {
        yield return new WaitForSeconds(secondTweenDelay);

        Sequence seq = DOTween.Sequence();

        seq.Append(UI_To_Tween.DOScaleY(0f, 1.5f).SetEase(secondTweenEase)
            .OnUpdate(() =>
            {
                if (UI_To_Tween.localScale.y <= 0.05f)
                {
                    seq.Append(DOTween.To(() => tmpText.color, x => tmpText.color = x, new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 0f), 1f));                }
            }));

        seq.OnComplete(() => { secondSequenceComplete = true; });
    }
}