// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpansionToolMissingSearch.cs">
//      Copyright ©Yunomi. All rights reserved.
//  </copyright>
//  <author>Yunomi</author>
//  <email>yunomi@childhooddream.sakura.ne.jp</email>
// --------------------------------------------------------------------------------------------------------------------
namespace MyChildhoodDream
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    #endregion

    /// <summary>
    /// エディタ拡張
    /// </summary>
    public class ExpansionToolMissingSearch : EditorWindow
    {
        private static List<AssetParameterData> missingList = new List<AssetParameterData>();

        private Vector2 scrollPos;

        [MenuItem("ExpansionTool/MissingSearch")]
        private static void Open()
        {
            ExpansionToolMissingSearch.missingList.Clear();
            ExpansionToolMissingSearch.ProcessSceneObject();
            EditorWindow.GetWindow<ExpansionToolMissingSearch>("MissingSearch");
        }

        /// <summary>
        /// シーンに実行する内容
        /// </summary>
        private static void ProcessSceneObject()
        {
            // 開いたシーンのヒエラルキー上のゲームオブジェクトに処理を行う
            foreach (var obj in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
                // アセットからパスを取得.シーン上に存在するオブジェクトの場合、シーンファイル（.unity）のパスを取得
                string allAssetsPath = AssetDatabase.GetAssetOrScenePath(obj);

                // シーン上に存在するオブジェクトかどうか文字列で判定
                bool isScene = allAssetsPath.Contains(".unity");

                // シーン上に存在するオブジェクトならば処理
                if (isScene) {
                    // コンポーネント内に一つでもnullがある場合にはmissing判定といえる
                    GameObject gameObject = (GameObject)obj;
                    var gameObjectComponentList = new List<Component>();
                    gameObject.GetComponents(gameObjectComponentList);
                    if (gameObjectComponentList.Any(x => x == null)) {
                        ExpansionToolMissingSearch.missingList.Add(new AssetParameterData { Obj = obj });
                    }
                }
            }
        }

        private void OnGUI()
        {
            // リストを表示
            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
            EditorGUILayout.LabelField("Asset", GUILayout.Width(300));
            foreach (AssetParameterData data in ExpansionToolMissingSearch.missingList) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(data.Obj, data.Obj.GetType(), true, GUILayout.Width(300));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (missingList.Count == 0) {
                this.ShowNotification(new GUIContent("ScriptMissingはありませんでした"));
            }
        }
    }

    /// <summary>
    /// 表示するパラメーター
    /// </summary>
    public class AssetParameterData
    {
        /// <summary> アセット </summary>
        public Object Obj { get; set; }
    }
}