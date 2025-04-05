#if UNITY_EDITOR
using UnityEngine;
using System.Reflection;
using Firebase.RemoteConfig;
using System.Collections.Generic;

namespace RimuruDev
{
    public static class FirebaseRemoteConfigHelper
    {
        public static void PrintRemoteConfigCache()
        {
            var type = typeof(FirebaseRemoteConfig);
            var field = type.GetField("remoteConfigByInstanceKey", BindingFlags.Static | BindingFlags.NonPublic);

            if (field != null)
            {
                if (field.GetValue(null) is IDictionary<string, FirebaseRemoteConfig> dictionary)
                {
                    Debug.Log($"<color=yellow>RemoteConfig cache count: {dictionary.Count}</color>");

                    foreach (var kvp in dictionary)
                        Debug.Log($"<color=yellow>Key: {kvp.Key}, Instance: {kvp.Value}</color>");
                }
                else
                    Debug.LogWarning("<color=yellow>Dictionary remoteConfigByInstanceKey == null.</color>");
            }
            else
                Debug.LogWarning("<color=red>Field remoteConfigByInstanceKey not found.</color>");
        }

        public static void ClearRemoteConfigCache()
        {
            // NOTE:
            // Словарик забетонирован, по этому пришлось действовать немного по плохому.
            // *
            // Так выглядит словарик remoteConfigByInstanceKey у FirebaseRemoteConfig:
            //
            //  public sealed class FirebaseRemoteConfig
            //  {
            //      private static readonly Dictionary<string, FirebaseRemoteConfig> remoteConfigByInstanceKey = new Dictionary<string, FirebaseRemoteConfig>();
            //  }
            var type = typeof(FirebaseRemoteConfig);
            var field = type.GetField("remoteConfigByInstanceKey", BindingFlags.Static | BindingFlags.NonPublic);

            if (field != null)
            {
                var dictionary = field.GetValue(null) as IDictionary<string, FirebaseRemoteConfig>;
                dictionary?.Clear();

                Debug.Log("<color=yellow>FirebaseRemoteConfig cache cleared.</color>");
            }
            else
                Debug.LogWarning("<color=red>Field remoteConfigByInstanceKey not found.</color>");
        }
    }
}
#endif