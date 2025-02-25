using UnityEngine;

namespace JD.Waiting
{
    public class WaitForFramesMutable : CustomYieldInstruction
    {
        public bool Done;
        private int _waitUntil;

        public WaitForFramesMutable()
        {
            
        }

        public WaitForFramesMutable(int frames)
        {
            _waitUntil = Time.frameCount + frames;
        }

        public WaitForFramesMutable Wait(int frames)
        {
            _waitUntil = Time.frameCount + frames;
            return this;
        }

        public override bool keepWaiting
        {
            get
            {
                var waiting = Time.frameCount < _waitUntil;
                if (!waiting) Waiting.Wait.Return(this);
                return waiting;
            }
        }

        public override void Reset()
        {
            Waiting.Wait.Return(this);
        }
    }
}