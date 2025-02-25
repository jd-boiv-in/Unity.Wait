using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JD.Waiting
{
    public struct WaitHandler
    {
        public Action Action;
        public float Timer;
    }
    
    public static class Wait
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic()
        {
            _frames.Clear();

            Time = 0f;
            RealTime = 0f;
            FakeTime = 0f;
            FakingTime = false;
        }

        public static float Time;
        public static float RealTime;
        public static float FakeTime;
        public static bool FakingTime;
        
        private static readonly Queue<WaitForSecondsMutable> _seconds = new Queue<WaitForSecondsMutable>(10);
        private static readonly Queue<WaitForFramesMutable> _frames = new Queue<WaitForFramesMutable>(10);

        private static readonly List<int> _removeHandlers = new List<int>(10); 
        private static readonly List<WaitHandler> _handlers = new List<WaitHandler>(10);

        public static void Update()
        {
            _removeHandlers.Clear();
            for (var i = 0; i < _handlers.Count; i++)
            {
                var handler = _handlers[i];
                handler.Timer -= UnityEngine.Time.deltaTime;
                if (handler.Timer <= 0)
                {
                    _removeHandlers.Add(i);
                    handler.Action.Invoke();
                    continue;
                }

                _handlers[i] = handler;
            }
            
            foreach (var i in _removeHandlers)
                _handlers.RemoveAt(i);
        }

        public static void Seconds(Action action, float timer)
        {
            _handlers.Add(new WaitHandler()
            {
                Timer = timer,
                Action = action
            });
        }
        
        public static float Start()
        {
            Time = FakeTime;
            return Time;
        }

        // This is when we want to synchronize a coroutine to a specific time
        public static bool For(float duration, out IEnumerator enumerator)
        {
            if (!FakingTime)
            {
                enumerator = Seconds(duration);
                return true;
            }

            // Faking time, so advance the time each time until we hit current real time
            if (Time + duration <= RealTime)
            {
                Time += duration;
                enumerator = null;
                return false;
            }
            
            var advance = RealTime - Time;
            Time = RealTime;
            duration = duration - advance;
            enumerator = Seconds(duration);
            return true;
        }
        
        public static IEnumerator Seconds(int id, float seconds)
        {
            WaitForSecondsMutable coroutine;
            if (_seconds.Count > 0) coroutine = _seconds.Dequeue();
            else coroutine = new WaitForSecondsMutable();

            coroutine.Done = false;
            return coroutine.Wait(seconds);
        }
        
        public static IEnumerator Seconds(float seconds)
        {
            WaitForSecondsMutable coroutine;
            if (_seconds.Count > 0) coroutine = _seconds.Dequeue();
            else coroutine = new WaitForSecondsMutable();

            coroutine.Done = false;
            return coroutine.Wait(seconds);
        }
        
        public static IEnumerator Frames(int frames)
        {
            WaitForFramesMutable coroutine;
            if (_frames.Count > 0) coroutine = _frames.Dequeue();
            else coroutine = new WaitForFramesMutable();

            coroutine.Done = false;
            return coroutine.Wait(frames);
        }

        public static void Return(WaitForSecondsMutable coroutine)
        {
            if (coroutine.Done) return;
            coroutine.Done = true;
            
            _seconds.Enqueue(coroutine);
        }
        
        public static void Return(WaitForFramesMutable coroutine)
        {
            if (coroutine.Done) return;
            coroutine.Done = true;
            
            _frames.Enqueue(coroutine);
        }
    }
}