using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace LittleSword.Common
{
    public static class Logger
    {
        [Conditional("DEVELOP_MODE")]
        [Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            //유니티 디버그 호출. DEVELOP_MODE/UNITY_EDITOR 없으면 호출은 빌드 포함 안됨
            Debug.Log(message);
        }

        [Conditional("DEVELOP_MODE")]
        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
            // 심각한 문제를 출력할 때 사용
            Debug.LogError(message);
        }

        [Conditional("DEVELOP_MODE")]
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
            // 주의가 필요한 상태를 출력할 때 사용
            Debug.LogWarning(message);
        }
    }
}
