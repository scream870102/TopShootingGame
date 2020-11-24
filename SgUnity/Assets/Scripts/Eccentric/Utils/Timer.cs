namespace Eccentric.Utils {
    using UnityEngine;
    /// <summary>a countdown timer easy to use not affected by TimeScale</summary>
    /// <remarks>call method Reset to reset timer and call property IsFinshed to check if countdown finished</remarks>
    [System.Serializable]
    class UnscaledTimer {
        float timeSection;
        float timer;
#if UNITY_EDITOR
        [ReadOnly, SerializeField] float remain = 0f;
        [ReadOnly, SerializeField] bool bFinished = false;
#endif

        /// <summary>remaining time until the countdown end</summary>
        public float Remain {
            get {
                float offset = timer - UnityEngine.Time.unscaledTime;
                if (offset >= 0f)
                    return offset;
                return 0f;
            }
        }

        /// <summary>return the cd range from 0 to 1 0 means timer finished </summary>
        public float Remain01 => Remain / timeSection;

        /// <summary>if this countdown finished or not</summary>
        public bool IsFinished {
            get {
#if UNITY_EDITOR
                bFinished = timer <= UnityEngine.Time.unscaledTime;
                remain = this.Remain;
#endif
                return timer <= UnityEngine.Time.unscaledTime;
            }
        }

        public UnscaledTimer (float timeSection = 0f, bool CanUseFirst = true) {
            this.timeSection = timeSection;
            if (!CanUseFirst)
                Reset ( );
            else
                this.timer = 0f;
        }

        /// <summary>Reset countdown timer with default setting</summary>
        public void Reset ( ) => timer = UnityEngine.Time.unscaledTime + timeSection;

        /// <summary>reset countdown timer with new timeSection</summary>
        public void Reset (float timeSection) {
            this.timeSection = timeSection;
            timer = UnityEngine.Time.unscaledTime + timeSection;
        }
    }

    /// <summary>a countdown timer easy to use affected by TimeScale</summary>
    /// <remarks>call method Reset to reset timer and call property IsFinshed to check if countdown finished</remarks>
    [System.Serializable]
    public class ScaledTimer {
        float timeSection;
        float timer;
#if UNITY_EDITOR
        [ReadOnly, SerializeField] float remain = 0f;
        [ReadOnly, SerializeField] bool bFinished = false;
#endif
        /// <summary>remaining time until the countdown end</summary>
        public float Remain {
            get {
                float offset = timer - UnityEngine.Time.time;
                if (offset >= 0f)
                    return offset;
                return 0f;
            }
        }

        /// <summary>return the cd range from 0 to 1 0 means timer finished </summary>
        public float Remain01 => Remain / timeSection;

        /// <summary>if this countdown finished or not</summary>
        public bool IsFinished {
            get {
#if UNITY_EDITOR
                bFinished = timer <= UnityEngine.Time.unscaledTime;
                remain = this.Remain;
#endif
                return timer <= UnityEngine.Time.time;
            }
        }

        public ScaledTimer (float timeSection = 0f, bool CanUseFirst = true) {
            this.timeSection = timeSection;
            if (!CanUseFirst)
                Reset ( );
            else
                this.timer = 0f;
        }

        /// <summary>Reset countdown timer with default setting</summary>
        public void Reset ( ) => timer = UnityEngine.Time.time + timeSection;

        /// <summary>reset countdown timer with new timeSection</summary>
        public void Reset (float timeSection) {
            this.timeSection = timeSection;
            timer = UnityEngine.Time.time + timeSection;
        }
    }
}