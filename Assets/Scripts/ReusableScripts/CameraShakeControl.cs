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
        ShakeCameraFor(time, amplitude, frequency, pivotOffset);
    }
    
    public void ShakeCameraFor(float time, float pamplitude, float pfrequency, Vector3 ppivotOffset)
    {
        if (_noiseTimer != null)
        {
            StopCoroutine(_noiseTimer);    
        }
        
        _noiseTimer = ShakeCameraCoroutine(time, pamplitude, pfrequency, ppivotOffset); 
        StartCoroutine(_noiseTimer);
    }

    IEnumerator ShakeCameraCoroutine(float time, float pamplitude, float pfrequency, Vector3 ppivotOffset)
    {
        _virtualCameraNoise.m_AmplitudeGain = pamplitude;
        _virtualCameraNoise.m_FrequencyGain = pfrequency;
        _virtualCameraNoise.m_PivotOffset = ppivotOffset;
        yield return new WaitForSeconds(time);
        _virtualCameraNoise.m_AmplitudeGain = 0.0f;
        _virtualCameraNoise.m_FrequencyGain = 0.0f;
        _virtualCameraNoise.m_PivotOffset = Vector3.zero;
    }
}
