using System;
using UnityEditor;
using UnityEngine;

namespace GitHub.Unity
{
    class Installer : ScriptableObject
    {
        private const string PackageName = "GitHub extensions";
        private const string QueryTitle = "Embed " + PackageName + "?";
        private const string QueryMessage =
            "This package has no project dependencies and so can either run embedded in your Unity install or remain in your assets folder.\n\nWould you like to embed it?";
        private const string QueryOK = "Embed";
        private const string QueryCancel = "Cancel";
        private const string ErrorTitle = "Installer error";
        private const string ErrorMessage = "An error occured during installation:\n{0}";
        private const string ErrorOK = "OK";

        public static void Initialize()
        {
            if (Utility.IsDevelopmentBuild)
            {
                return;
            }

            // Detect install path
            string selfPath;
            var instance = FindObjectOfType(typeof(Installer)) as Installer;
            if (instance == null)
            {
                instance = CreateInstance<Installer>();
            }
            var script = MonoScript.FromScriptableObject(instance);
            if (script == null)
            {
                selfPath = string.Empty;
            }
            else
            {
                selfPath = AssetDatabase.GetAssetPath(script);
            }
            DestroyImmediate(instance);

            // If we cannot self-locate then forget the whole thing
            if (string.IsNullOrEmpty(selfPath))
            {
                return;
            }

            // Perform move
            if (EditorUtility.DisplayDialog(QueryTitle, QueryMessage, QueryOK, QueryCancel))
            {
                MoveFrom(Application.dataPath + selfPath.Substring("Assets".Length, selfPath.LastIndexOf('/') - "Assets".Length));
            }

            // Self-delete
            AssetDatabase.DeleteAsset(selfPath);
        }

        private static void MoveFrom(string path)
        {
            try
            {
                Debug.LogFormat("Installer move from '{0}'", path);
                // TODO: Create the necessary structure and perform the actual move of files into it from the given install path
            }
            catch (Exception e)
            {
                Failure(e.ToString());
            }
        }

        private static void Failure(string error)
        {
            EditorUtility.DisplayDialog(ErrorTitle, String.Format(ErrorMessage, error), ErrorOK);
        }
    }
}