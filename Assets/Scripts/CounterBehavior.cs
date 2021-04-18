using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CounterBehavior : MonoBehaviour
{
  [SerializeField]
  private Image counterImage;
  [SerializeField]
  private Text counterText;
  [SerializeField]
  private int time = 3;
  
  private void OnEnable()
  {
    AnimateCounter();
  }

  public void AnimateCounter(int anotherTime)
  {
    ResetValues();
    StartCoroutine(CountDown(anotherTime));
    counterImage.DOFillAmount(1.0f, anotherTime)
    .OnComplete(() =>
      {
        counterImage.rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1.0f), 1.0f).OnComplete(() =>
          {
            GameLogic.GetInstance().StartLevel();
            gameObject.SetActive(false);
          });
      });
  }

  private IEnumerator CountDown(int anotherTime)
  {
    do
    {
      counterText.text = anotherTime.ToString();
      yield return new WaitForSeconds(1.0f);
    } while (--anotherTime > 0);
    counterText.text = "0";
  }

  public void AnimateCounter()
  {
    AnimateCounter(time);
  }

  private void OnDisable()
  {
    ResetValues();
    counterImage.rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
  }

  private void ResetValues()
  {
    counterImage.fillAmount = 0.0f;
  }
}
