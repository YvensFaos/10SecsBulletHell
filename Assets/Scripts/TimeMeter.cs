using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeMeter : MonoBehaviour
{
    [SerializeField] private Image timeMeterImage;
    private TweenerCore<float, float, FloatOptions> _timerTween;

    [SerializeField] private Text hpText;
    [SerializeField] private Text shieldText;
    [SerializeField] private Text levelText;

    private PlayerBehavior _player;
    private ShieldBehavior _shieldBehavior;
    private LevelManager _levelManager;

    [SerializeField] private UnityEvent timesUp;

    private void Awake()
    {
        timeMeterImage.fillAmount = 0.0f;
    }

    private void Start()
    {
        _player = GameLogic.GetInstance().Player;
        _shieldBehavior = _player.GetShield();
        _levelManager = GameLogic.GetInstance().Level;
    }

    public void StartTimeMeter()
    {
        timeMeterImage.fillAmount = 0.0f;
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
        hpText.text = "H: 0";
        shieldText.text = "S: 0";
        levelText.text = "L: 0";
    }

    public void Update()
    {
        hpText.text = "H: " + _player.GetCurrentHealth();
        shieldText.text = "S: " + _shieldBehavior.GetCurrentStrength();
        levelText.text = "L: " + _levelManager.GetCurrentLevel();
    }
}