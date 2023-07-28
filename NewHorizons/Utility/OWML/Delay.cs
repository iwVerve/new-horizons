using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NewHorizons.Utility.OWML
{
    public static class Delay
    {
        #region OnSceneUnloaded
        static Delay() => SceneManager.sceneUnloaded += OnSceneUnloaded;

        private static void OnSceneUnloaded(Scene _)
        {
            Main.Instance.StopAllCoroutines();
            _deferred.Clear();
        }
        #endregion

        #region public methods
        public static void StartCoroutine(IEnumerator coroutine) => Main.Instance.StartCoroutine(coroutine);

        public static void RunWhen(Func<bool> predicate, Action action) => StartCoroutine(RunWhenCoroutine(action, predicate));

        public static void FireInNUpdates(Action action, int n) => StartCoroutine(FireInNUpdatesCoroutine(action, n));

        public static void FireOnNextUpdate(Action action) => FireInNUpdates(action, 1);

        public static void RunWhenAndInNUpdates(Action action, Func<bool> predicate, int n) => Delay.StartCoroutine(RunWhenOrInNUpdatesCoroutine(action, predicate, n));
        #endregion

        #region Coroutines
        private static IEnumerator RunWhenCoroutine(Action action, Func<bool> predicate)
        {
            while (!predicate.Invoke())
            {
                yield return new WaitForEndOfFrame();
            }

            action.Invoke();
        }

        private static IEnumerator FireInNUpdatesCoroutine(Action action, int n)
        {
            for (int i = 0; i < n; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            action?.Invoke();
        }

        private static IEnumerator RunWhenOrInNUpdatesCoroutine(Action action, Func<bool> predicate, int n)
        {
            for (int i = 0; i < n; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            while (!predicate.Invoke())
            {
                yield return new WaitForEndOfFrame();
            }

            action.Invoke();
        }
        #endregion

        public static void CallDeferred(int priority, Action action)
        {
            _deferred.Add((priority, action));
            HasDeferred = true;
        }

        public static void CallDeferred(Action action) => CallDeferred(0, action);

        // Lower number = higher priority
        private static readonly List<(int priority, Action action)> _deferred = new();

        public static bool HasDeferred { get; private set; }

        public static void InvokeDeferredActions()
        {
            HasDeferred = false;
            _deferred.OrderBy(x => x.priority);
            foreach (var (priority, action) in _deferred)
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    NHLogger.LogError($"Failed to invoke deferred action {e}");
                }
            }
            _deferred.Clear();
        }
    }
}
