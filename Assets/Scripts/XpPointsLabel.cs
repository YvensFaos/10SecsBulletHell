using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(RectTransform))]
public class XpPointsLabel : MonoBehaviour
{
   private Text _text;
   private RectTransform _rectTransform;

   private readonly Vector3 _scaleUp = new Vector3(1.5f, 1.4f, 1.0f);
   private readonly Vector3 _scaleNormal = new Vector3(1.0f, 1.0f, 1.0f);
   private StringBuilder _stringBuilder;
   private void Awake()
   {
      _text = GetComponent<Text>();
      _rectTransform = GetComponent<RectTransform>();
      UpdateValues();
   }

   public void UpdateXpPointsLabel()
   {
      UpdateValues();
      _rectTransform.DOScale(_scaleUp, 0.4f).OnComplete(() => _rectTransform.DOScale(_scaleNormal, 0.2f));
   }

   public void UpdateValues()
   {
      if (_stringBuilder == null)
      {
         _stringBuilder = new StringBuilder();
      }
      _stringBuilder.Clear();
      _stringBuilder.AppendFormat("XP Points: {0}  (+{1}/10 secs)", GameLogic.GetInstance().GetXp(), GameLogic.GetInstance().GetXpPerRound());
      _text.text = _stringBuilder.ToString();
   }
}
