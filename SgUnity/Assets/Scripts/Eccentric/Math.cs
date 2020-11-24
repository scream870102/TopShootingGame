namespace Eccentric {
    using System;

    using UnityEngine;
    static class Math {
        /// <summary>check if a value between the two value or not</summary>
        /// <remarks>will return true if the value equal to a or b</remarks>
        /// <param name="value">the value you want to check</param>
        public static bool Between (float value, float a, float b) {
            if (a > b) {
                if (value <= a && value >= b)return true;
                else return false;
            }
            else {
                if (value >= a && value <= b)return true;
                else return false;
            }
        }

        /// <summary>return the option due to the percentage</summary>
        /// <remarks>will transfer the total of two probability to 100% linearly</remarks>
        /// <param name="option1">first option</param>
        /// <param name="option2">second option</param>
        /// <param name="probability1">the probability of option1</param>
        /// <param name="probability2">the probability of option2</param>
        public static T ChosenDueToProbability<T> (in T option1, in T option2, float probability1, float probability2) {
            if (probability1 + probability2 != 1f) {
                float tmp = 1f / (probability1 + probability2);
                probability1 *= tmp;
                probability2 *= tmp;
            }
            System.Random rand = new System.Random (DateTime.Now.Millisecond);
            float percentage = (float)rand.NextDouble ( );
            if (percentage <= probability1)return option1;
            else return option2;
        }

        /// <summary>return true if option1 being choose</summary>
        /// <param name="probability1">probability of option1</param>
        /// <param name="probability2">probability of option2</param>
        public static bool ChosenDueToProbability (float probability1, float probability2) {
            if (probability1 + probability2 != 1f) {
                float tmp = 1f / (probability1 + probability2);
                probability1 *= tmp;
                probability2 *= tmp;
            }
            System.Random rand = new System.Random (DateTime.Now.Millisecond);
            float percentage = (float)rand.NextDouble ( );
            if (percentage <= probability1)return true;
            else return false;
        }

        /// <summary>return a random integer from 0 to number-1</summary>
        /// <param name="number">how many options</param>
        public static int RandomNum (int number) {
            return UnityEngine.Random.Range (0, number);
        }

        /// <summary>return a random float from 0.0f to number</summary>
        /// <param name="number">the biggest num for this random</param>
        public static float RandomNum (float number) {
            return UnityEngine.Random.Range (0f, number);
        }

        /// <summary>return a boolean randomly</summary>
        public static bool RandomBool ( ) {
            return UnityEngine.Random.Range (0, 2) % 2 == 0;
        }

        /// <summary>inverst the probability if give 0.3 will return 0.7</summary>
        /// <param name="origin">the probability you want to inverse</param>
        public static float InverseProbability (float origin) {
            float tmp = UnityEngine.Mathf.Clamp01 (origin);
            return 1f - tmp;
        }

        /// <summary>return a random vector3</summary>
        public static Vector3 RandomVec3 (float num) {
            return new Vector3 (RandomNum (num), RandomNum (num), RandomNum (num));
        }

        /// <summary>return a random vector3</summary>
        public static Vector3 RandomVec3 (Vector3 num) {
            return new Vector3 (RandomNum (num.x), RandomNum (num.y), RandomNum (num.z));
        }

        /// <summary>return a random vector2</summary>
        public static Vector2 RandomVec2 (float num) {
            return new Vector2 (RandomNum (num), RandomNum (num));
        }

        /// <summary>return a random vector2</summary>
        public static Vector2 RandomVec2 (Vector2 num) {
            return new Vector3 (RandomNum (num.x), RandomNum (num.y));
        }

        /// <summary>return the degree due to direction</summary>
        public static float GetDegree (Vector2 direction) {
            return Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
        }
        public static Vector2 GetDirectionFromRad (float radian) {
            return new Vector2 (Mathf.Cos (radian), Mathf.Sin (radian)).normalized;
        }
        /// <summary>return the vector2 in normalize from degree</summary>
        public static Vector2 GetDirectionFromDeg (float degree) {
            return GetDirectionFromRad (degree * Mathf.Deg2Rad).normalized;
        }
    }
}
