using UnityEngine;

namespace JD.Waiting
{
    // Better solution: https://forum.unity.com/threads/when-we-need-new-instance-of-intrinsic-yieldinstruction.1014100/#post-6575215
    // Saved the DLL file in notes folder outside of Unity since it is a binary and we cannot rebuild the source...
    public class WaitForSecondsMutable : CustomYieldInstruction
    {
        public bool Done;
        private float _waitUntil;

        public WaitForSecondsMutable()
        {
            
        }

        public WaitForSecondsMutable(float seconds)
        {
            _waitUntil = Time.time + seconds;
        }

        public WaitForSecondsMutable Wait(float seconds)
        {
            _waitUntil = Time.time + seconds;
            return this;
        }

        public override bool keepWaiting
        {
            get
            {
                var waiting = Time.time < _waitUntil;
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