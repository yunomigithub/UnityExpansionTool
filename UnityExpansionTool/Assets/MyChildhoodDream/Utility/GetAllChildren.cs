namespace MyChildhoodDream.Utility
{
    #region

    using System.Collections.Generic;
    using UnityEngine;

    #endregion

    /// <summary>
    /// 自身と子要素のオブジェクトをすべて取得する
    /// かずおの開発ブログさんのこちらの拡張メソッドを利用しています。
    /// http://kazuooooo.hatenablog.com/entry/2015/08/07/010938
    /// </summary>
    public static class GetAllChildren
    {
        /// <summary>
        /// 自身と子要素をすべて取得し、Listで返す
        /// </summary>
        /// <param name="obj">親のオブジェクト</param>
        public static List<GameObject> GetAll(this GameObject obj)
        {
            var allChildren = new List<GameObject>();
            allChildren.Add(obj);
            GetAllChildren.GetChildren(obj, ref allChildren);
            return allChildren;
        }

        /// <summary>
        /// 子要素を取得してリストに追加
        /// </summary>
        public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
        {
            Transform children = obj.GetComponentInChildren<Transform>();

            // 子要素がいなければ終了
            if (children.childCount == 0) {
                return;
            }

            foreach (Transform ob in children) {
                allChildren.Add(ob.gameObject);
                GetAllChildren.GetChildren(ob.gameObject, ref allChildren);
            }
        }
    }
}