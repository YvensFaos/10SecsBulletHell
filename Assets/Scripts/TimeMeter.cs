using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeMeter : MonoBehaviour
{
   [SerializeField]
   private Image timeMeterImage;
   private TweenerCore<float, float, FloatOptions> _timerTween;

   [SerializeField] 
   private UnityEvent timesUp;
   
   private void Awake()
   {
      timeMeterImage.fillAmount = 0.0f;
   }

   public void StartTimeMeter()
   {
       _timerTween = timeMeterImage.DOFillAmount(1.0f, 10.0f);
       _timerTween.OnComplete(() =>
       {
           GameLogic.GetInstance().NotifyTimesUp();
           timesUp.Invoke();
       });
   }

   public void StopTimeMeter()
   {
       _timerTween.Kill();
   }
}
