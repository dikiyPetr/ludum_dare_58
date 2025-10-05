using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Cutscenes
{
    /// <summary>
    /// Управление затемнением экрана
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 0.5f;

        private Coroutine fadeCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Создать fade image если не назначен
            if (fadeImage == null)
            {
                CreateFadeImage();
            }

            // Изначально прозрачный
            SetAlpha(0f);
        }

        private void CreateFadeImage()
        {
            var canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                var canvasObj = new GameObject("FadeCanvas");
                canvasObj.transform.SetParent(transform);
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;
                
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                canvasObj.AddComponent<GraphicRaycaster>();
                
                Debug.Log("[ScreenFader] Создан FadeCanvas");
            }

            var imgObj = new GameObject("FadeImage");
            imgObj.transform.SetParent(canvas.transform, false);
            fadeImage = imgObj.AddComponent<Image>();
            fadeImage.color = Color.black;
            fadeImage.raycastTarget = false; // Не блокировать клики

            var rect = fadeImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            Debug.Log("[ScreenFader] FadeImage создан и настроен");
        }

        /// <summary>
        /// Затемнить экран
        /// </summary>
        public void FadeOut(float duration = -1f, Action onComplete = null)
        {
            if (duration < 0) duration = fadeDuration;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            Debug.Log($"[ScreenFader] FadeOut начат, длительность: {duration}s");
            fadeCoroutine = StartCoroutine(FadeCoroutine(1f, duration, onComplete));
        }

        /// <summary>
        /// Осветлить экран
        /// </summary>
        public void FadeIn(float duration = -1f, Action onComplete = null)
        {
            if (duration < 0) duration = fadeDuration;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            Debug.Log($"[ScreenFader] FadeIn начат, длительность: {duration}s");
            fadeCoroutine = StartCoroutine(FadeCoroutine(0f, duration, onComplete));
        }

        /// <summary>
        /// Установить прозрачность напрямую
        /// </summary>
        public void SetAlpha(float alpha)
        {
            if (fadeImage != null)
            {
                var color = fadeImage.color;
                color.a = Mathf.Clamp01(alpha);
                fadeImage.color = color;
            }
        }

        private IEnumerator FadeCoroutine(float targetAlpha, float duration, Action onComplete)
        {
            if (fadeImage == null) yield break;

            float startAlpha = fadeImage.color.a;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));
                yield return null;
            }

            SetAlpha(targetAlpha);
            onComplete?.Invoke();
            fadeCoroutine = null;
        }
    }
}

