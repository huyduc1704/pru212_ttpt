using UnityEngine;

namespace Oink.UIAnimation.Static
{
    public static class Settings
    {
        public static float MinimumKeyFrameValue = 0.01f;

#if UNITY_EDITOR
        public static float GUIPadding = 2f;
        public static float GUILabelWidth = 0.4f;
        public static float GUINameWidth = 0.6f;
        public static float GUIFeatureWidth = 0.3f;
        public static float GUIValueWidth = 0.6f;
        public static float GUIEasingWidth = 0.5f;
#endif

        public enum OKEasing
        {
            // None,
            Linear,
            EaseInOutSine,
            EaseInQuart,
            EaseOutQuart,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInElastic,
            EaseOutElastic,
            Spring,
        }

        public static float GetAnimatedValue(float minValue, float maxValue, float progress, OKEasing easing)
        {
            // Clamp progress between 0 and 1
            progress = Mathf.Clamp01(progress);

            switch (easing)
            {
                case OKEasing.Linear: return Mathf.Lerp(minValue, maxValue, progress);
                case OKEasing.EaseInOutSine: return Mathf.Lerp(minValue, maxValue, -(Mathf.Cos(Mathf.PI * progress) - 1) / 2);
                case OKEasing.EaseInQuart: return Mathf.Lerp(minValue, maxValue, Mathf.Pow(progress, 4));
                case OKEasing.EaseOutQuart: return Mathf.Lerp(minValue, maxValue, 1 - Mathf.Pow(1 - progress, 4));
                case OKEasing.EaseInCubic: return Mathf.Lerp(minValue, maxValue, Mathf.Pow(progress, 3));
                case OKEasing.EaseOutCubic: return Mathf.Lerp(minValue, maxValue, 1 - Mathf.Pow(1 - progress, 3));
                case OKEasing.EaseInOutCubic: return Mathf.Lerp(minValue, maxValue, progress < 0.5f ? 4 * Mathf.Pow(progress, 3) : 1 - Mathf.Pow(-2 * progress + 2, 3) / 2);
                case OKEasing.EaseInElastic:
                    {
                        float c4 = 2 * Mathf.PI / 3;
                        return Mathf.Lerp(minValue, maxValue, progress == 0 ? 0 : progress == 1 ? 1 : -Mathf.Pow(2, 10 * progress - 10) * Mathf.Sin((progress * 10 - 10.75f) * c4));
                    }
                case OKEasing.EaseOutElastic:
                    {
                        float c4 = 2 * Mathf.PI / 3;
                        return Mathf.Lerp(minValue, maxValue, progress == 0 ? 0 : progress == 1 ? 1 : Mathf.Pow(2, -10 * progress) * Mathf.Sin((progress * 10 - 0.75f) * c4) + 1);
                    }
                case OKEasing.Spring: return Mathf.Lerp(minValue, maxValue, Mathf.Sin(progress * Mathf.PI * (0.2f + 2.5f * Mathf.Pow(progress, 3))) * Mathf.Pow(1 - progress, 2.2f) + progress);
                default: return Mathf.Lerp(minValue, maxValue, progress);
            }
        }
    }
}