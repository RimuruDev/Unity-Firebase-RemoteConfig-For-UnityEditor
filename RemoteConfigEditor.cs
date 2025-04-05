#if UNITY_EDITOR
using System;
using Firebase;
using UnityEditor;
using UnityEngine;
using Firebase.RemoteConfig;
using System.Threading.Tasks;

namespace RimuruDev
{
    public sealed class RemoteConfigEditor : EditorWindow
    {
        private string log;
        private bool firebaseInitialized;

        [MenuItem("RimuruDev Tools/Firebase RemoteConfig Editor")]
        private static void ShowWindow() =>
            GetWindow<RemoteConfigEditor>("RemoteConfig Editor");

        private async void OnEnable()
        {
            try
            {
                var result = await FirebaseApp.CheckAndFixDependenciesAsync();

                if (result == DependencyStatus.Available)
                {
                    firebaseInitialized = true;
                    log += "\nFirebase initialized in Editor";
                }
                else
                {
                    log += $"\nCould not resolve Firebase dependencies: {result}";
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"TODO: handle exception");
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Remote Config Editor", EditorStyles.boldLabel);
            GUILayout.Label(log, EditorStyles.wordWrappedLabel);

            EditorGUI.BeginDisabledGroup(!firebaseInitialized);

            if (GUILayout.Button("Print RemoteConfig Cache"))
                FirebaseRemoteConfigHelper.PrintRemoteConfigCache();

            if (GUILayout.Button("Restart Firebase and Fetch Remote Config"))
                _ = RestartFirebaseAndFetchAsync();

            EditorGUI.EndDisabledGroup();
        }

        private async Task RestartFirebaseAndFetchAsync()
        {
            try
            {
                FirebaseRemoteConfigHelper.PrintRemoteConfigCache();
                FirebaseRemoteConfigHelper.ClearRemoteConfigCache();
                FirebaseAppHelper.DisposeAllFirebaseApps();

                // NOTE: Ну я на всякий пожарный подожду, мало ли не успеет прогреться :D
                await Task.Delay(500);

                var result = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (result != DependencyStatus.Available)
                {
                    log += $"\nCould not reinitialize Firebase: {result}";
                    Repaint();
                    return;
                }

                firebaseInitialized = true;
                log += "\nFirebase reinitialized in Editor";

                // NOTE: Прогрев перед подтягиванием свежачка с сервачка :3
                var config = FirebaseRemoteConfig.DefaultInstance;
                var settings = config.ConfigSettings;

                settings.MinimumFetchIntervalInMilliseconds = 0;
                settings.FetchTimeoutInMilliseconds = 30000;

                await config.SetConfigSettingsAsync(settings);

                await config.EnsureInitializedAsync();

                // NOTE: Получаем свежачок, и кайфуем.
                await config.FetchAsync(TimeSpan.Zero);
                await config.ActivateAsync();

                // NOTE: Мега лень париться с StringBuilder, один-фиг редактор.
                log += $"\nFetch succeeded! Time: {config.Info.FetchTime}";
                log += $"\nAll values count: {config.AllValues.Count}";

                foreach (var pair in config.AllValues)
                    log += $"\n[ Key: {pair.Key} | Value: {pair.Value.StringValue} ]";
            }
            catch (Exception e)
            {
                log += $"\nRestart/Fetch failed: {e}";
            }

            Repaint();
        }
    }
}
#endif