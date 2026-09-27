using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codex.PpgModRuntime
{
    public static class ModInvoker
    {
        private const string BadgeObjectName = "FlizanRuntimeFixBadge";

        private static readonly FieldInfo MetadataField = typeof(ModAPI).GetField(
            "metadata",
            BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly FieldInfo MetadataIsValidField = typeof(ModAPI).GetField(
            "metaDataIsValid",
            BindingFlags.NonPublic | BindingFlags.Static);

        public static bool Invoke(string methodName, ModMetaData metaData)
        {
            EnsureBadge();

            if (metaData == null || !metaData.Active || string.IsNullOrWhiteSpace(methodName))
            {
                return false;
            }

            if (!ModLoader.ModScripts.TryGetValue(metaData, out ModScript script) ||
                script?.LoadedAssembly == null)
            {
                return false;
            }

            try
            {
                Type entryPoint = script.LoadedAssembly.GetType(
                    metaData.EntryPoint,
                    throwOnError: false,
                    ignoreCase: false);

                if (entryPoint == null)
                {
                    return RecordFailure(
                        metaData,
                        $"Entry point type '{metaData.EntryPoint}' was not found.");
                }

                MethodInfo method = entryPoint.GetMethod(
                    methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                    binder: null,
                    types: Type.EmptyTypes,
                    modifiers: null);

                if (method == null)
                {
                    return false;
                }

                if (MetadataField == null || MetadataIsValidField == null)
                {
                    return RecordFailure(metaData, "The ModAPI invocation context fields were not found.");
                }

                MetadataField.SetValue(obj: null, value: metaData);
                MetadataIsValidField.SetValue(obj: null, value: true);
                try
                {
                    method.Invoke(obj: null, parameters: null);
                    return true;
                }
                finally
                {
                    MetadataIsValidField.SetValue(obj: null, value: false);
                    MetadataField.SetValue(obj: null, value: null);
                }
            }
            catch (TargetInvocationException exception)
            {
                return RecordFailure(metaData, (exception.InnerException ?? exception).ToString());
            }
            catch (Exception exception)
            {
                return RecordFailure(metaData, exception.ToString());
            }
        }

        public static void EnsureBadge()
        {
            if (GameObject.Find(BadgeObjectName) != null)
            {
                return;
            }

            GameObject badgeObject = new GameObject(BadgeObjectName);
            UnityEngine.Object.DontDestroyOnLoad(badgeObject);
            badgeObject.AddComponent<MainMenuBadge>();
        }

        private static bool RecordFailure(ModMetaData metaData, string error)
        {
            metaData.HasErrors = true;
            metaData.Errors = error;
            Debug.LogError($"Could not invoke mod {metaData.Name}:\n{error}");
            return false;
        }
    }

    internal sealed class MainMenuBadge : MonoBehaviour
    {
        private GUIStyle style;

        private void OnGUI()
        {
            if (!string.Equals(
                SceneManager.GetActiveScene().name,
                "Menu",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperCenter,
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
            }

            GUI.Label(
                new Rect(0f, 8f, Screen.width, 30f),
                "Fix by flizan.com",
                style);
        }
    }
}
