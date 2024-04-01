using NewHorizons.Utility.OWML;
using System;
using System.Collections;
using UnityEngine;

namespace NewHorizons.Handlers
{
    public static class FadeHandler
    {
        public static void FadeOut(float length) => Delay.StartCoroutine(FadeOutCoroutine(length));

        public static void FadeIn(float length) => Delay.StartCoroutine(FadeInCoroutine(length));

        private static IEnumerator FadeOutCoroutine(float length)
        {
            LoadManager.s_instance._fadeCanvas.enabled = true;
            float startTime = Time.unscaledTime;
            float endTime = Time.unscaledTime + length;

            while (Time.unscaledTime < endTime)
            {
                LoadManager.s_instance._fadeImage.color = Color.Lerp(Color.clear, Color.black, (Time.unscaledTime - startTime) / length);
                yield return new WaitForEndOfFrame();
            }

            LoadManager.s_instance._fadeImage.color = Color.black;
            yield return new WaitForEndOfFrame();
        }

        private static IEnumerator FadeInCoroutine(float length)
        {
            float startTime = Time.unscaledTime;
            float endTime = Time.unscaledTime + length;

            while (Time.unscaledTime < endTime)
            {
                LoadManager.s_instance._fadeImage.color = Color.Lerp(Color.black, Color.clear, (Time.unscaledTime - startTime) / length);
                yield return new WaitForEndOfFrame();
            }

            LoadManager.s_instance._fadeCanvas.enabled = false;
            LoadManager.s_instance._fadeImage.color = Color.clear;

            yield return new WaitForEndOfFrame();
        }

        public static void FadeThen(float length, Action action) => Delay.StartCoroutine(FadeThenCoroutine(length, action));

        private static IEnumerator FadeThenCoroutine(float length, Action action)
        {
            yield return FadeOutCoroutine(length);

            action?.Invoke();
        }
    }
}
