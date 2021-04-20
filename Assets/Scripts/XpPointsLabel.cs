using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(RectTransform))]
public class XpPointsLabel : MonoBehaviour
{
   private Text _text;
   private RectTransform _rectTransform;

   [SerializeField]
   private string defaultText;
   
   private readonly Vector3 _scaleUp = new Vector3(1.5f, 1.4f, 1.0f);
   private readonly Vector3 _scaleNormal = new Vector3(1.0f, 1.0f, 1.0f);
   
   private void Awake()
   {
      _text = GetComponent<Text>();
      _rectTransform = GetComponent<RectTransform>();
      UpdateXpPointsLabel(GameLogic.GetInstance().GetXpPoints());
   }

   public void UpdateXpPointsLabel(int points)
   {
      _text.text = defaultText + " " + points;
      _rectTransform.DOScale(_scaleUp, 0.4f).OnComplete(() => _rectTransform.DOScale(_scaleNormal, 0.2f));
   }
}
