// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpansionToolSceneWindowControll.cs">
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

    #endregion

    /// <summary>
    /// エディタ拡張
    /// </summary>
    [InitializeOnLoad]
    public class ExpansionToolSceneWindowControll : EditorWindow
    {
        static ExpansionToolSceneWindowControll()
        {
            // ウィンドウの初期位置
            Rect windowRect = new Rect(10, 30, 120, 30);

            var quarterViewPos = Quaternion.LookRotation(new Vector3(-1, -1, 1));
            GameObject cameraObject;
            string textField = string.Empty;

            // シーン上にGUIを配置する
            SceneView.onSceneGUIDelegate += sceneView => {
                // 各種ボタンの処理
                windowRect = GUILayout.Window(1,
                    windowRect,
                    id => {
                        // シーンのカメラがクォータービューになる
                        if (GUILayout.Button("QuarterView")) {
                            sceneView.rotation = quarterViewPos;
                        }

                        // MainCameraタグの付いたカメラをシーンビューと同期させる
                        if (GUILayout.Button("CemeraSync")) {
                            cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
                            cameraObject.transform.position = sceneView.camera.transform.position;
                            cameraObject.transform.rotation = sceneView.camera.transform.rotation;
                        }

                        if (GUILayout.Button("SceneMove")) {
                            // シーンを開く。編集中なら保存するかしないかを確認するダイアログを表示。
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                                EditorSceneManager.OpenScene("Assets/Scenes/" + textField + ".unity");
                            }
                        }

                        textField = GUILayout.TextField(textField);

                        GUI.DragWindow();
                    },
                    "ExpansionTool");
            };
        }
    }
}