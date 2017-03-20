// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpansionToolSceneObject.cs">
//      Copyright ©Yunomi. All rights reserved.
//  </copyright>
//  <author>Yunomi</author>
//  <email>yunomi@childhooddream.sakura.ne.jp</email>
// --------------------------------------------------------------------------------------------------------------------
namespace MyChildhoodDream
{
    #region

    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    #endregion

    /// <summary>
    /// エディタ拡張
    /// </summary>
    public class ExpansionToolSearchObject : EditorWindow
    {
        // シーンの保管場所のファイルパス
        private const string TargetScene = "Assets/Scenes";

        private bool isCurrentScene = true;

        private bool isFullScene;

        private bool[] isOtherScene = new bool[200];

        private Vector2 leftScrollPos;

        private enum SceneObjectAction
        {
            TextNameDelete
        }

        [MenuItem("ExpansionTool/SceneObject")]
        private static void Open()
        {
            EditorWindow.GetWindow<ExpansionToolSearchObject>("一括処理");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                // 左側のウィンドウ(シーンリストを表示)
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("実行対象");
                    this.isFullScene = GUILayout.Toggle(this.isFullScene, " 全シーン", GUILayout.MinWidth(200));
                    EditorGUILayout.LabelField("---------------------------------------");

                    // スクロール
                    this.leftScrollPos = EditorGUILayout.BeginScrollView(this.leftScrollPos);
                    {
                        this.isCurrentScene = GUILayout.Toggle(this.isCurrentScene, " 現在のシーン", GUILayout.MinWidth(200));

                        string[] lookFolder = { ExpansionToolSearchObject.TargetScene };
                        int sceneNum = 0;

                        foreach (var guid in AssetDatabase.FindAssets("t:Scene", lookFolder)) {
                            var scenePath = AssetDatabase.GUIDToAssetPath(guid);
                            var sceneName = AssetDatabase.LoadMainAssetAtPath(scenePath);

                            if (SceneManager.GetActiveScene().path != scenePath) {
                                this.isOtherScene[sceneNum] = GUILayout.Toggle(this.isOtherScene[sceneNum], " " + sceneName.name);
                            }

                            sceneNum++;
                        }
                    }

                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.EndVertical();

                // 右側のウィンドウ
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(250));
                {
                    EditorGUILayout.LabelField("処理一覧");

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.PrefixLabel("カッコを一括で外す");
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("実行")) {
                                this.GetValue(SceneObjectAction.TextNameDelete, true);
                            }

                            if (GUILayout.Button("調査")) {
                                this.GetValue(SceneObjectAction.TextNameDelete, false);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void GetValue(SceneObjectAction sceneObjectAction, bool isExecution)
        {
            string[] lookFolder = { ExpansionToolSearchObject.TargetScene };
            string currentScenePath = SceneManager.GetActiveScene().path;
            int sceneNum = 0;
            int targetObjectNum = 0;

            // 指定フォルダ以下のシーンファイルを検索
            foreach (var guid in AssetDatabase.FindAssets("t:Scene", lookFolder)) {
                // ProgressBar表示
                EditorUtility.DisplayProgressBar("実行中", "しばらくお待ち下さい", sceneNum / (float)AssetDatabase.FindAssets("t:Scene", lookFolder).Length);

                var objectPathInHierarchy = AssetDatabase.GUIDToAssetPath(guid);

                // チェックなしの場合は処理を行わない
                if ((this.isCurrentScene == false) && (objectPathInHierarchy == currentScenePath) && (this.isFullScene == false)) {
                    sceneNum++;
                    continue;
                }

                if ((this.isOtherScene[sceneNum] == false) && (objectPathInHierarchy != currentScenePath) && (this.isFullScene == false)) {
                    sceneNum++;
                    continue;
                }

                // シーンを開く。編集中なら保存するかしないかを確認するダイアログを表示。
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    EditorSceneManager.OpenScene(objectPathInHierarchy);
                }

                // 開いたシーンのヒエラルキー上のゲームオブジェクトに処理を行う
                foreach (var obj in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
                    // アセットからパスを取得.シーン上に存在するオブジェクトの場合、シーンファイル（.unity）のパスを取得
                    string allAssetsPath = AssetDatabase.GetAssetOrScenePath(obj);

                    // シーン上に存在するオブジェクトかどうか文字列で判定
                    bool isScene = allAssetsPath.Contains(".unity");

                    // シーン上に存在するオブジェクトならば処理
                    if (isScene) {
                        switch (sceneObjectAction) {
                            case SceneObjectAction.TextNameDelete:
                                targetObjectNum += this.DeleteUnnecessaryText(obj, isExecution);
                                break;
                        }
                    }
                }

                sceneNum++;
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }

            // 処理が終わってProgressBarを閉じる
            EditorUtility.ClearProgressBar();

            // 実行時のシーンに戻る
            EditorSceneManager.OpenScene(currentScenePath);

            this.ShowNotification(new GUIContent("対象件数は" + targetObjectNum + "件でした"));
        }

        /// <summary>
        /// テキストを削除する
        /// </summary>
        /// <param name="obj">検索中のオブジェクト</param>
        /// <param name="isExecution">実際に変換処理まで行うのかどうか</param>
        /// <returns>変換した数</returns>
        private int DeleteUnnecessaryText(Object obj, bool isExecution)
        {
            // 置換処理。一致するものがなかった場合は-1を返す
            int matchTextIndexNum = obj.name.IndexOf("(");
            if (matchTextIndexNum >= 0) {
                if (isExecution) {
                    obj.name = obj.name.Substring(0, matchTextIndexNum - 1);
                }

                return 1;
            }

            return 0;
        }
    }
}