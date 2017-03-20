// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpansionToolAttachObject.cs">
//      Copyright ©Yunomi. All rights reserved.
//  </copyright>
//  <author>Yunomi</author>
//  <email>yunomi@childhooddream.sakura.ne.jp</email>
// --------------------------------------------------------------------------------------------------------------------
namespace MyChildhoodDream
{
    #region

    using System.Collections.Generic;
    using MyChildhoodDream.Utility;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    #endregion

    /// <summary>
    /// エディタ拡張
    /// </summary>
    public class ExpansionToolAttachObject : EditorWindow
    {
        // 改行コード(\r\n)
        private string textArea = "ここに該当の内容が入ります";

        private Object obj;

        private GameObject attachObject;

        [MenuItem("ExpansionTool/AttachObject")]
        private static void Open()
        {
            EditorWindow.GetWindow<ExpansionToolAttachObject>("アタッチしたオブジェクトに処理");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            this.obj = EditorGUILayout.ObjectField(this.obj, typeof(Object), true);
            EditorGUILayout.EndHorizontal();
            this.attachObject = (GameObject)this.obj;

            if (GUILayout.Button("Anchorの確認をする")) {
                var outputText = string.Empty;

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                if (this.attachObject.HasComponent<CanvasScaler>() == false) {
                    this.ShowNotification(new GUIContent("Canvasオブジェクトをセットして下さい"));
                    return;
                }

                if (this.attachObject.transform.childCount > 0) {
                    for (var i = 0; i < this.attachObject.transform.childCount; i++) {
                        if (!this.attachObject.transform.GetChild(i).gameObject.HasComponent<RectTransform>()) {
                            continue;
                        }

                        if ((this.attachObject.transform.GetChild(i).GetComponent<RectTransform>().anchorMax == new Vector2(0.5f, 0.5f))
                            && (this.attachObject.transform.GetChild(i).GetComponent<RectTransform>().anchorMin == new Vector2(0.5f, 0.5f))) {
                            outputText += this.attachObject.transform.GetChild(i).gameObject.name + "\r\n";
                        }
                    }
                }
                else {
                    this.ShowNotification(new GUIContent("子を持つオブジェクトをセットしてください"));
                }

                if (outputText == string.Empty) {
                    this.ShowNotification(new GUIContent("Anchorの付け忘れはありません"));
                }

                this.textArea = outputText;
            }

            if (GUILayout.Button("子階層の名前に付くカッコを一括で外す")) {
                int count = 0;

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                List<GameObject> objList = this.attachObject.GetAll();
                foreach (GameObject obj in objList) {
                    // 一致するものがなかった場合は-1。emptyの場合は0を返す
                    int matchTextIndexNum = obj.name.IndexOf("(");
                    if (matchTextIndexNum >= 0) {
                        obj.name = obj.name.Substring(0, matchTextIndexNum - 1);
                        count++;
                    }
                }

                this.ShowNotification(new GUIContent(count + "件変換しました"));
            }

            if (GUILayout.Button("子階層のImage/TextのRaycastTargetを一括でOFFにする")) {
                var count = 0;
                var outputText = "変更を加えたオブジェクトのリスト" + "\r\n";

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                List<GameObject> objList = this.attachObject.GetAll();
                foreach (GameObject obj in objList) {
                    if (obj.HasComponent<Image>() && obj.GetComponent<Image>().raycastTarget) {
                        obj.GetComponent<Image>().raycastTarget = false;
                        outputText += obj.name + "\r\n";
                        count++;
                    }
                    else if (obj.HasComponent<Text>() && obj.GetComponent<Text>().raycastTarget) {
                        obj.GetComponent<Text>().raycastTarget = false;
                        outputText += obj.name + "\r\n";
                        count++;
                    }
                }

                this.ShowNotification(new GUIContent(count + "件のRaycastTargetをOFFにしました"));

                this.textArea = outputText;
            }

            if (GUILayout.Button("オブジェクト名リストを作成する")) {
                string outputText = string.Empty;

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                List<GameObject> objList = this.attachObject.GetAll();
                foreach (GameObject obj in objList) {
                    outputText += obj.name + "\r\n";
                }

                this.textArea = outputText;
            }

            if (GUILayout.Button("オブジェクト名の変数リストを作成する(全部入り)")) {
                string outputText = string.Empty;

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                List<GameObject> objList = this.attachObject.GetAll();
                foreach (GameObject obj in objList) {
                    outputText += this.ConvertSerializeFieldText(obj);
                }

                this.textArea = outputText;
            }

            if (GUILayout.Button("リセット文を作成する(全部入り)")) {
                string outputText = "private void Reset ()\r\n{\r\n\r\n";

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                List<GameObject> objList = this.attachObject.GetAll();
                foreach (GameObject obj in objList) {
                    outputText += this.ConvertResetText(obj);
                }

                this.textArea = outputText + "}";
            }

            if (GUILayout.Button("オブジェクト名の変数リストを作成する(オブジェクトのみ)")) {
                string outputText = string.Empty;

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                List<GameObject> objList = this.attachObject.GetAll();
                foreach (GameObject obj in objList) {
                    outputText += this.ObjectOnlyConvertSerializeFieldText(obj);
                }

                this.textArea = outputText;
            }

            if (GUILayout.Button("リセット文を作成する(オブジェクトのみ)")) {
                string outputText = "private void Reset ()\r\n{\r\n\r\n";

                if (this.attachObject == null) {
                    this.ShowNotification(new GUIContent("オブジェクトをセットして下さい"));
                    return;
                }

                List<GameObject> objList = this.attachObject.GetAll();
                foreach (GameObject obj in objList) {
                    outputText += this.ObjectOnlyConvertResetText(obj);
                }

                this.textArea = outputText + "}";
            }

            if (GUILayout.Button("リセット")) {
                this.textArea = "ここに該当の内容が入ります";
            }

            EditorGUILayout.TextArea(this.textArea);
        }

        /// <summary>
        /// オブジェクト名をSerializeField付きの変数名にする
        /// </summary>
        /// <param name="currentObject">アタッチしているオブジェクト</param>
        /// <returns>変換後のテキストを挿入済みの画面に表示するテキスト</returns>
        private string ConvertSerializeFieldText(GameObject currentObject)
        {
            string outputText = "[SerializeField] private GameObject " + this.ConvertFirstTextLowerCase(currentObject.name) + "Object;\r\n\r\n";
            if (currentObject.HasComponent<Text>()) {
                outputText += "[SerializeField] private Text " + this.ConvertFirstTextLowerCase(currentObject.name) + ";\r\n\r\n";
            }

            if (currentObject.HasComponent<Button>()) {
                outputText += "[SerializeField] private Button " + this.ConvertFirstTextLowerCase(currentObject.name) + ";\r\n\r\n";
            }

            if (currentObject.HasComponent<Image>()) {
                outputText += "[SerializeField] private Image " + this.ConvertFirstTextLowerCase(currentObject.name) + ";\r\n\r\n";
            }

            return outputText;
        }

        /// <summary>
        /// リセットメソッドを作成する
        /// </summary>
        /// <param name="currentObject">アタッチしているオブジェクト</param>
        /// <returns>変換後のテキストを挿入済みの画面に表示するテキスト</returns>
        private string ConvertResetText(GameObject currentObject)
        {
            string outputText = "    this." + this.ConvertFirstTextLowerCase(currentObject.name) + "Object = GameObject.Find(\"" + currentObject.name + "\").gameObject;\r\n\r\n";
            if (currentObject.HasComponent<Text>()) {
                outputText += "    this." + this.ConvertFirstTextLowerCase(currentObject.name) + " = GameObject.Find(\"" + currentObject.name + "\").GetComponent<Text>();\r\n\r\n";
            }

            if (currentObject.HasComponent<Button>()) {
                outputText += "    this." + this.ConvertFirstTextLowerCase(currentObject.name) + " = GameObject.Find(\"" + currentObject.name + "\").GetComponent<Button>();\r\n\r\n";
            }

            if (currentObject.HasComponent<Image>()) {
                outputText += "    this." + this.ConvertFirstTextLowerCase(currentObject.name) + " = GameObject.Find(\"" + currentObject.name + "\").GetComponent<Image>();\r\n\r\n";
            }

            return outputText;
        }

        /// <summary>
        /// リセットメソッドを作成する
        /// </summary>
        /// <param name="currentObject">アタッチしているオブジェクト</param>
        /// <returns>変換後のテキストを挿入済みの画面に表示するテキスト</returns>
        private string ObjectOnlyConvertResetText(GameObject currentObject)
        {
            string outputText = "    this." + this.ConvertFirstTextLowerCase(currentObject.name) + "Object = GameObject.Find(\"" + currentObject.name + "\").gameObject;\r\n\r\n";
            return outputText;
        }

        /// <summary>
        /// オブジェクト名をSerializeField付きの変数名にする（オブジェクトのみ）
        /// </summary>
        /// <param name="currentObject">アタッチしているオブジェクト</param>
        /// <returns>変換後のテキストを挿入済みの画面に表示するテキスト</returns>
        private string ObjectOnlyConvertSerializeFieldText(GameObject currentObject)
        {
            string outputText = "[SerializeField] private GameObject " + this.ConvertFirstTextLowerCase(currentObject.name) + "Object;\r\n\r\n";
            return outputText;
        }

        /// <summary>
        /// 先頭の文字を小文字に変換する
        /// </summary>
        /// <param name="convertText">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        private string ConvertFirstTextLowerCase(string convertText)
        {
            var returnText = string.Empty;
            for (var i = 0; i < convertText.Length; i++) {
                if (i == 0) {
                    returnText += convertText[i].ToString().ToLower();
                }
                else {
                    returnText += convertText[i];
                }
            }

            return returnText;
        }
    }
}