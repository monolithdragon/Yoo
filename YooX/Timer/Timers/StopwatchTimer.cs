using UnityEngine;

namespace YooX.Timer {
    /// <summary>
    /// A Timer that counts up from zero to infinity.  
    /// Great for measuring durations.
    /// </summary>
    public class StopwatchTimer : Timer {
        public override bool IsFinished => false;

        public StopwatchTimer() : base(0) { }

        public override void Tick() {
            if (IsRunning) {
                CurrentTime += Time.deltaTime;
            }
        }
    }

}
