using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace TUtils{
    public static class Watcher {
        public class Agent : MonoBehaviour{
            public IEnumerator WaitAndDo(float waitTime, Action action, bool useUnscaledTime)
            {
                if (waitTime > 0)
                {
                    if (!useUnscaledTime)
                        yield return new WaitForSeconds(waitTime);
                    else yield return new WaitForSecondsRealtime(waitTime);
                }
                Sandbox.Execute(action);
            }
            public IEnumerator WaitFramesAndDo(int waitFrames, Action action)
            {
                while (waitFrames > 0){
                    waitFrames--;
                    yield return null;
                }
                Sandbox.Execute(action);
            }
            public IEnumerator WaitToEndOfFrameAndDo(Action action){
                yield return new WaitForEndOfFrame();
                Sandbox.Execute(action);
            }
            public delegate bool ConditionTestFunction();
            public IEnumerator WaitForConditionAndDo(ConditionTestFunction condition, Action action, float maxWaitScaledTime, float maxWaitUnscaledTime, int maxWaitFrame, Action onTimeout) {
                if (action == null)
                {
                    Tebug.Warning("Parameter error. WaitForConditionAndDo aborted.");
                    yield break;
                }
                while (true) {
                    if (condition() == true)
                    {
                        Sandbox.Execute(action);
                        yield break;
                    }
                    else
                    {
                        //check if there's still time for waiting
                        if (maxWaitFrame > 0 || maxWaitScaledTime > 0 || maxWaitUnscaledTime > 0)
                        {
                            //now we wait one frame
                            yield return null;
                            //after waiting, update the wait limits
                            maxWaitFrame--;
                            maxWaitScaledTime -= Time.deltaTime;
                            maxWaitUnscaledTime -= Time.unscaledDeltaTime;
                        } 
                        else //can't wait anymore
                        {
                            if (onTimeout != null)
                            {
                                Sandbox.Execute(onTimeout);
                            }
                            yield break;
                        }
                    }
                }
            }
            public IEnumerator RunEveryNFrames(Action action, int frameInterval, bool runNow){
                if (runNow) {
                    if (!Sandbox.Execute(action)) yield break;
                }
                int fc = frameInterval;
                yield return null;
                while (true){
                    --fc;
                    if (fc <= 0) {
                        if (!Sandbox.Execute(action)) yield break;
                        fc = frameInterval;
                    }
                    yield return null;
                }
            }
            public IEnumerator RunEveryNSeconds(Action action, float secondsInterval, bool runNow, bool useUnscaledTime){
                if (runNow) {
                    if (!Sandbox.Execute(action)) yield break;
                }
                float sc = secondsInterval;
                yield return null;
                while (true){
                    if (useUnscaledTime)
                        sc -= Time.unscaledDeltaTime;
                    else 
                        sc -= Time.deltaTime;
                    if (sc <= 0) {
                        if (!Sandbox.Execute(action)) yield break;
                        sc = secondsInterval;
                    }
                    yield return null;
                }
            }
            public IEnumerator RunEveryFrameWhen(ConditionTestFunction condition, Action action, bool runNow) {
                if (action == null || condition == null) yield break;
                if (!runNow) yield return null; //wait for the next frame to start running
                while (true) {
                    if (condition()) {
                        if (!Sandbox.Execute(action)) yield break;
                    }
                    yield return null; 
                }
            }
        }
        static Agent globalInstance;
        static Agent localInstance;
        static Agent GetInstance(bool globally, bool createIfNull = true){
            if (globally){
                if (createIfNull && (globalInstance == null || globalInstance.gameObject == null || globalInstance.gameObject.transform == null)){
                    GameObject g = new GameObject("GlobalWatcher");
                    GameObject.DontDestroyOnLoad(g);
                    globalInstance = g.AddComponent<Agent>();
                }
                return globalInstance;
            } else {
                if (createIfNull && (localInstance == null || localInstance.gameObject == null || localInstance.gameObject.transform == null))
                    localInstance = new GameObject("LocalWatcher").AddComponent<Agent>();
                return localInstance;
            }
        }

        ///<summary>StartCoroutine using Watcher's auto-generated instance. Set globally to true to use Don'tDestroyOnLoad instance.</summary>
        public static Coroutine StartCoroutine(IEnumerator ie, bool globally){
            Agent instance = GetInstance(globally);
            if (instance == null) return null;
            return instance.StartCoroutine(ie);
        }
        ///<summary>StopCoroutine using Watcher's auto-generated instance. Watcher will try both local(not DontDestroyOnLoad) and global(DontDestroyOnLoad) instance.</summary>
        public static void StopCoroutine(Coroutine coroutine){
            GetInstance(true, false)?.StopCoroutine(coroutine);
            GetInstance(false, false)?.StopCoroutine(coroutine);
        }
        ///<summary>Stop all coroutines started by Watcher.</summary>
        public static void StopAllCoroutines(){
            GetInstance(true, false).StopAllCoroutines();
            GetInstance(false, false).StopAllCoroutines();
        }

        //for schedulings
        ///<summary>Schedule an action to happen at some time in the future. The Action WILL NOT RUN when waitTime is NaN or Watcher can't create an instance of itself. Set globally to true to run under DontDestroyOnLoad instance</summary>
        public static Coroutine Schedule(Action action, float waitTime, bool useUnscaledTime, bool globally){
            Agent instance = GetInstance(globally);
            if (instance == null) return null;
            if (float.IsNaN(waitTime)){
                Tebug.Warning("Unexpected behavior: waitTime is NaN. Aborting WaitAndDo.");
                return null;
            }
            return instance.StartCoroutine(instance.WaitAndDo(waitTime, action, useUnscaledTime));
        }
        ///<summary>Schedule an action to happen after some frames in the future. The Action WILL RUN IMMEDIATELY waitFrames is less than 0. The Action WILL NOT RUN if Watcher can't create an instance of itself. Set globally to true to run under DontDestroyOnLoad instance</summary>
        public static Coroutine ScheduleFrames(Action action, int waitFrames, bool globally){
            Agent instance = GetInstance(globally);
            if (instance != null)
                return instance.StartCoroutine(instance.WaitFramesAndDo(waitFrames, action));
            return null;
        }
        ///<summary>Shortcut for ScheduleFramesWithScene(action, 1)</summary>
        public static Coroutine ScheduleNextFrame(Action action, bool globally) {
            return ScheduleFrames(action, 1, globally);
        }
        ///<summary>Schedule an action to run at the end of the frame. The Action WILL NOT RUN if Watcher can't create an instance of itself. Set globally to true to run under DontDestroyOnLoad instance</summary>
        public static Coroutine ScheduleEndOfFrame(Action action, bool globally){
            Agent instance = GetInstance(globally);
            if (instance != null)
                return instance.StartCoroutine(instance.WaitToEndOfFrameAndDo(action));
            return null;
        }
        ///<summary>Schedule an action to run once when the condition is true. The Action WILL NOT RUN if Watcher can't create an instance of itself. Set globally to true to run under DontDestroyOnLoad instance</summary>
        public static Coroutine ScheduleWhen(Agent.ConditionTestFunction condition, Action action, bool globally, float maxWaitScaledTime = float.MaxValue, float maxWaitUnscaledTime = float.MaxValue, int maxWaitFrame = int.MaxValue, Action onTimeout = null) {
            Agent instance = GetInstance(globally);
            if (instance != null) {
                if (condition == null) {
                    Tebug.Warning("Null test condition. The action will not run. Aborting ScheduleWhen");
                    return null;
                } else return instance.StartCoroutine(instance.WaitForConditionAndDo(condition, action, maxWaitScaledTime, maxWaitUnscaledTime, maxWaitFrame, onTimeout));
            }
            return null;
        }

        //setInterval functions
        public static Coroutine RunMeEveryNFrames(Action action, int frameInterval, bool runNow, bool globally){
            Agent instance = GetInstance(globally);
            if (instance == null) return null;
            if (frameInterval <= 0) {
                Tebug.Warning("Parameter error. frameInterval is less than or equal to 0. Aborting RunEveryNFrames.");
                return null;
            }
            return instance.StartCoroutine(instance.RunEveryNFrames(action, frameInterval, runNow));
        }
        public static Coroutine RunMeEveryNSeconds(Action action, float secondsInterval, bool runNow, bool useUnscaledTime, bool globally){
            Agent instance = GetInstance(globally);
            if (instance == null) return null;
            if (float.IsNaN(secondsInterval)){
                Tebug.Warning("Unexpected behavior: secondInterval is NaN. Aborting RunEveryNSeconds.");
                return null;
            }
            if (secondsInterval <= 0) {
                Tebug.Warning("Parameter error: secondInterval is less than or equal to zero. Aborting RunEveryNSeconds.");
                return null;
            }
            return instance.StartCoroutine(instance.RunEveryNSeconds(action, secondsInterval, runNow, useUnscaledTime));
        }
        public static Coroutine RunMeEveryFrameWhen(Agent.ConditionTestFunction condition, Action action, bool runNow, bool globally) {
            Agent instance = GetInstance(globally);
            if (instance != null) {
                if (condition == null) {
                    Tebug.Warning("Null test condition. The action will not run. Aborting RunMeEveryFrameWhen");
                    return null;
                } else return instance.StartCoroutine(instance.RunEveryFrameWhen(condition, action, runNow));
            }
            return null;
        }
    }
}