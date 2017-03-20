// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ExpansionToolUI.cs">
//      Copyright ©Yunomi. All rights reserved.
//  </copyright>
//  <author>Yunomi</author>
//  <email>yunomi@childhooddream.sakura.ne.jp</email>
// --------------------------------------------------------------------------------------------------------------------
namespace MyChildhoodDream
{
    #region

    using UnityEditor;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    #endregion

    /// <summary>
    /// 拡張エディタ
    /// </summary>
    public class ExpansionToolUI : EditorWindow
    {
        private const int CanvasSizeX = 750;

        private const int CanvasSizeY = 1334;

        private static GameObject CreateEmptyUIObject(string objectName)
        {
            var createObject = new GameObject(objectName, typeof(RectTransform));
            return createObject;
        }

        /// <summary>
        /// 選択中オブジェクトの子に空のオブジェクトを作成する。未選択の場合は一番親階層
        /// </summary>
        private static GameObject CreateEmptyUIObjectInSelectObject(string objectName)
        {
            GameObject emptyObject = ExpansionToolUI.CreateEmptyUIObject(objectName);

            var selectObject = Selection.GetTransforms(SelectionMode.DeepAssets);

            foreach (Transform t in selectObject) {
                emptyObject.transform.SetParent(t.transform);
                emptyObject.layer = t.gameObject.layer;
            }

            emptyObject.transform.localPosition = Vector3.zero;
            emptyObject.transform.localScale = Vector3.one;

            return emptyObject;
        }

        /// <summary>
        /// 下敷きの作成
        /// </summary>
        [MenuItem("ExpansionTool/UI/SamplePosImage")]
        private static void CreateSamplePosImage()
        {
            GameObject canvasObject = ExpansionToolUI.CreateEmptyUIObject("SamplePosImageCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(ExpansionToolUI.CanvasSizeX, ExpansionToolUI.CanvasSizeY);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject imageObject = ExpansionToolUI.CreateEmptyUIObject("SamplePosImage");
            var childObjectImage = imageObject.AddComponent<Image>();
            imageObject.transform.SetParent(canvasObject.transform, false);
            imageObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            imageObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            imageObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            childObjectImage.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);

            if (GameObject.Find("EventSystem") == null) {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }
        }

        /// <summary>
        /// 画像のみのトグルの作成
        /// </summary>
        [MenuItem("GameObject/ExpansionTool/UI/Toggle", false, 20)]
        private static void CreateToggle()
        {
            GameObject toggleObject = ExpansionToolUI.CreateEmptyUIObjectInSelectObject("Toggle");
            var imageComponent = toggleObject.AddComponent<Image>();
            var toggleComponent = toggleObject.AddComponent<Toggle>();

            toggleComponent.transition = Selectable.Transition.None;
            toggleComponent.graphic = imageComponent;
            toggleComponent.isOn = true;
            Selection.activeGameObject = toggleObject;
        }

        /// <summary>
        /// raycastTargetがoffのイメージの作成
        /// </summary>
        [MenuItem("GameObject/ExpansionTool/UI/Image", false, 20)]
        private static void CreateImage()
        {
            GameObject imageObject = ExpansionToolUI.CreateEmptyUIObjectInSelectObject("Image");
            var imageComponent = imageObject.AddComponent<Image>();
            imageComponent.raycastTarget = false;
            Selection.activeGameObject = imageObject;
        }

        /// <summary>
        /// 縦スクロールオブジェクトの作成
        /// </summary>
        [MenuItem("GameObject/ExpansionTool/UI/VerticalScrollView", false, 20)]
        private static void CreateVerticalScrollView()
        {
            // 初期化
            GameObject scrollView = ExpansionToolUI.CreateEmptyUIObjectInSelectObject("ScrollView");
            var scrollRectComponent = scrollView.AddComponent<ScrollRect>();

            GameObject viewport = ExpansionToolUI.CreateEmptyUIObjectInSelectObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);

            GameObject content = ExpansionToolUI.CreateEmptyUIObjectInSelectObject("Content");
            content.transform.SetParent(viewport.transform, false);

            // 必要内容の追加
            scrollView.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            scrollView.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 0f);
            scrollRectComponent.horizontal = false;

            viewport.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            viewport.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            viewport.AddComponent<RectMask2D>();

            var scrollViewImageComponet = scrollView.AddComponent<Image>();
            scrollViewImageComponet.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            scrollRectComponent.content = content.GetComponent<RectTransform>();
            scrollRectComponent.viewport = viewport.GetComponent<RectTransform>();

            var viewportImageComponet = viewport.AddComponent<Image>();
            viewportImageComponet.color = new Color(1.0f, 1.0f, 1.0f, 0f);

            content.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            content.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            content.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);

            content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            var verticalLayoutGroup = content.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = 50;

            var contentSizeFitter = content.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Selection.activeGameObject = content;
        }
    }
}