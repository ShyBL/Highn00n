using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class UIStretchTween : MonoBehaviour
{
    [Header("Object that follows the mouse")]
    [SerializeField] private Transform followObject;
    [SerializeField] private float followDelay = 0.1f;
    [SerializeField] private TextMeshProUGUI tmpText;
     
    [Header("Tween Settings")]
    [SerializeField] private Ease firstTweenEase = Ease.OutQuad; 
    [SerializeField] private Ease secondTweenEase = Ease.InOutExpo; 
    [SerializeField] private float firstTweenDelay = 0.5f; 
    [SerializeField] private float secondTweenDelay = 1f;

    public UnityEvent OnFinish;
    private bool canStartSecondSequence = false;
    private bool secondSequenceComplete = false;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

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

        
        seq.Append(rectTransform.DOScaleY(0.14f, 1f).SetEase(firstTweenEase)).
            OnUpdate(() =>
            {
                if (rectTransform.localScale.y <= 0.14f)
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

        seq.Append(rectTransform.DOScaleY(0f, 1.5f).SetEase(secondTweenEase)
            .OnUpdate(() =>
            {
                if (rectTransform.localScale.y <= 0.05f)
                {
                    seq.Append(DOTween.To(() => tmpText.color, x => tmpText.color = x, new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 0f), 1f));                }
            }));

        seq.OnComplete(() => { secondSequenceComplete = true; });
    }

    private void FollowMouseWithDelay()
    {
        // Get current mouse position
        Vector3 mousePos = Input.mousePosition;

        // Limit offset size to prevent excessive movement
        float offsetMultiplier = 0.2f; // Adjust this value to control how much it lags behind

        Vector3 targetPos = new Vector3(
            Mathf.Lerp(followObject.position.x, mousePos.x, followDelay * offsetMultiplier * Time.deltaTime),
            Mathf.Lerp(followObject.position.y, mousePos.y, followDelay * offsetMultiplier * Time.deltaTime),
            followObject.position.z // Keep Z unchanged
        );

        // Apply calculated position
        followObject.position = targetPos;
    }
}