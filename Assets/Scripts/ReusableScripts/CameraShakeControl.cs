using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShakeControl : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin _virtualCameraNoise;

    [SerializeField]
    private Vector3 pivotOffset;
    [SerializeField]
    private float amplitude;
    [SerializeField]
    private float frequency;

    private IEnumerator _noiseTimer;
    
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _virtualCameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    public void ShakeCameraFor(float time)
    {
        if (_noiseTimer != null)
        {
            StopCoroutine(_noiseTimer);    
        }
        
        _noiseTimer = ShakeCameraCoroutine(time); 
        StartCoroutine(_noiseTimer);
    }

    IEnumerator ShakeCameraCoroutine(float duration)
    {
        _virtualCameraNoise.m_AmplitudeGain = amplitude;
        _virtualCameraNoise.m_FrequencyGain = frequency;
        _virtualCameraNoise.m_PivotOffset = pivotOffset;
        yield return new WaitForSeconds(duration);
        _virtualCameraNoise.m_AmplitudeGain = 0.0f;
        _virtualCameraNoise.m_FrequencyGain = 0.0f;
        _virtualCameraNoise.m_PivotOffset = Vector3.zero;
    }
}
