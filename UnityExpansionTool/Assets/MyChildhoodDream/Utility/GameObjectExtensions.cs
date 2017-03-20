namespace MyChildhoodDream.Utility
{
    #region

    using UnityEngine;

    #endregion

    /// <summary>
    /// GameObject 型の拡張メソッドを管理するクラス
    /// コガネブログさんのこちらの拡張メソッドを利用しています。
    /// http://baba-s.hatenablog.com/entry/2014/03/24/100614
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 指定されたコンポーネントがアタッチされているかどうかを返します
        /// </summary>
        public static bool HasComponent<T>(this GameObject self) where T : Component
        {
            return self.GetComponent<T>() != null;
        }
    }
}