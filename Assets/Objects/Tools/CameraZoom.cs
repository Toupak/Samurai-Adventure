using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CameraZoom : MonoBehaviour
{
    public static UnityEvent<float, float> RequestCameraZoomForDuration = new UnityEvent<float, float>();
    
    [SerializeField] private float zoomSpeed;

    private CinemachineVirtualCamera virtualCamera;
    private Coroutine zoomCoroutine = null;

    private float initialZoom;
    private float velocity;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        initialZoom = virtualCamera.m_Lens.OrthographicSize;

        RequestCameraZoomForDuration.AddListener(ZoomForADuration);
    }

    private void ZoomForADuration(float zoom, float duration)
    {
        if (zoomCoroutine != null)
            return;

        zoomCoroutine = StartCoroutine(ZoomForDurationCoroutine(zoom, duration));
    }
    private IEnumerator ZoomForDurationCoroutine(float targetZoom, float duration)
    {
        float timer = duration;

        while (timer >= 0.0f)
        {
            float currentZoom = Tools.NormalizeValueInRange(timer, 0.0f, duration, targetZoom, initialZoom);
            virtualCamera.m_Lens.OrthographicSize = currentZoom;

            yield return null;
            timer -= Time.deltaTime;
        }

        while (timer <= duration)
        {
            float currentZoom = Tools.NormalizeValueInRange(timer, 0.0f, duration, targetZoom, initialZoom);
            virtualCamera.m_Lens.OrthographicSize = currentZoom;

            yield return null;
            timer += Time.deltaTime;
        }

        zoomCoroutine = null;
    }
    
    private IEnumerator ZoomForDurationCoroutineSmooth(float targetZoom, float duration)
    {
        float zoom = initialZoom;

        while (zoom > targetZoom)
        {
            zoom = Mathf.SmoothDamp(zoom, targetZoom, ref velocity, zoomSpeed);
            virtualCamera.m_Lens.OrthographicSize = zoom;
            yield return null;
        }

        while (zoom < initialZoom)
        {
            zoom = Mathf.SmoothDamp(zoom, initialZoom, ref velocity, zoomSpeed);
            virtualCamera.m_Lens.OrthographicSize = zoom;
            yield return null;
        }

        zoomCoroutine = null;
    }
}

