using UnityEngine;
using UnityEngine.UI;

public class ConfigPanelController : MonoBehaviour
{
    [SerializeField]
    private MixerAudioSlider musicSliderController;
    [SerializeField]
    private MixerAudioSlider sfxSliderController;
    [SerializeField]
    private GameObject debugPanel;
    [SerializeField]
    private Toggle debugToggle;
    
    private void OnEnable()
    {
        musicSliderController.SetSliderValue();
        sfxSliderController.SetSliderValue();
        debugToggle.isOn = debugPanel.activeSelf;
    }
}
