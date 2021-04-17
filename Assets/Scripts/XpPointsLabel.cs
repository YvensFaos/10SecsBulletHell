using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class XpPointsLabel : MonoBehaviour
{
   private Text _text;

   [SerializeField]
   private string defaultText;
   
   private void Awake()
   {
      _text = GetComponent<Text>();
      UpdateXpPointsLabel(GameLogic.GetInstance().GetXpPoints());
   }

   public void UpdateXpPointsLabel(int points)
   {
      _text.text = defaultText + " " + points;
   }
}
