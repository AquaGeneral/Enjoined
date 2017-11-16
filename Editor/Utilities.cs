using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JesseStiller.Enjoined {
    internal static class Utilities {
        internal static float FloorToSignificantDigits(float number, int numberOfDigits) {
            if(number == 0f) return 0f;

            float scale = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(Mathf.Abs(number))) + 1f);
            return scale * (float)Math.Round(number / scale, numberOfDigits);
        }
    }
}