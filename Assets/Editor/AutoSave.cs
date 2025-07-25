using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magikorp
{
    public class AutoSaveConfig : ScriptableObject
    {
        [Tooltip("Enable auto save functionality")]
        public bool Enabled = true;

        [Tooltip("The frequency in minutes auto save will activate"), Min(1)]
        public int Frequency = 2;

        [Tooltip("Log a message every time the scene is auto saved")]
        public bool Logging = true;
    }

    [CustomEditor(typeof(AutoSaveConfig))]
    public class AutoSave : Editor
    {
        private static AutoSaveConfig _config;
        private static CancellationTokenSource _tokenSource;
        private static Task _task;
        private static float _lastSaveTime;

        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            FetchConfig();
            CancelTask();

            _tokenSource = new CancellationTokenSource();
            _task = SaveInterval(_tokenSource.Token);
        }

        private static void FetchConfig()
        {
            if (_config != null) return;

            var path = GetConfigPath();

            if (path == null)
            {
                // Ensure Settings folder exists
                if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                {
                    AssetDatabase.CreateFolder("Assets", "Settings");
                }
                AssetDatabase.CreateAsset(CreateInstance<AutoSaveConfig>(), $"Assets/Settings/{nameof(AutoSaveConfig)}.asset");
                AssetDatabase.SaveAssets();
                Debug.Log("A config file has been created in the Settings folder of your project.<b> You can move this anywhere you'd like.</b>");
                path = GetConfigPath();
            }

            _config = AssetDatabase.LoadAssetAtPath<AutoSaveConfig>(path);
        }

        private static string GetConfigPath()
        {
            var paths = AssetDatabase.FindAssets(nameof(AutoSaveConfig)).Select(AssetDatabase.GUIDToAssetPath).Where(c => c.EndsWith(".asset")).ToList();
            if (paths.Count > 1) Debug.LogWarning("Multiple auto save config assets found. Delete one.");
            return paths.FirstOrDefault();
        }

        private static void CancelTask()
        {
            if (_task == null) return;
            _tokenSource.Cancel();
            try { _task.Wait(); } catch (AggregateException) { /* Task was cancelled, ignore */ }
        }

        private static async Task SaveInterval(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (Time.realtimeSinceStartup - _lastSaveTime < _config.Frequency * 60f)
                {
                    // wait the difference to the next save intervale with a minimum of 30 second
                    float waitTime = Mathf.Max(30f, _config.Frequency * 60f - (Time.realtimeSinceStartup - _lastSaveTime));
                    await Task.Delay((int)(waitTime * 1000), token);
                    continue;
                }

                if (!_config.Enabled || Application.isPlaying || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) continue;
                if (!UnityEditorInternal.InternalEditorUtility.isApplicationActive) continue;

                if (IsDirty())
                {
                    EditorSceneManager.SaveOpenScenes();
                    if (_config.Logging) Debug.Log($"Auto-Saved at {DateTime.Now:h:mm:ss tt}");
                }

                _lastSaveTime = Time.realtimeSinceStartup;
            }
        }

        public static bool IsDirty()
        {
            if (_config == null) return false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).isDirty) return true;
            }

            return false;
        }

        [MenuItem("Window/Auto Save/Find Config")]
        public static void ShowConfig()
        {
            FetchConfig();

            var path = GetConfigPath();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<AutoSaveConfig>(path).GetInstanceID());
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("You can move this asset where ever you'd like.", MessageType.Info);
        }
    }
}