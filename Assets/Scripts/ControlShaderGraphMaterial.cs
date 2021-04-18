using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ControlShaderGraphMaterial : MonoBehaviour
{
    private Material _material;
    private TweenerCore<float, float, FloatOptions> _tween;

    private void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        _material = spriteRenderer.material;
    }

    public void SetMaterialValue(string uniform, float value)
    {
        _material.SetFloat(uniform, value);
    }

    public void AnimateMaterialValue(string uniform, float animaeteTo, float time)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        _tween = DOTween.To(() => _material.GetFloat(uniform), value => _material.SetFloat(uniform, value), animaeteTo, time);
    }
}
