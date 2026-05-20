using System;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// バックログに表示する、話者名と本文の履歴です。
    /// </summary>
    [Serializable]
    public sealed class ScenarioBacklogEntry
    {
        public string Speaker;
        public string Text;

        /// <summary>
        /// JsonUtility互換のために空コンストラクタを残します。
        /// </summary>
        public ScenarioBacklogEntry()
        {
        }

        /// <summary>
        /// バックログ1件分を初期化します。
        /// </summary>
        public ScenarioBacklogEntry(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }

        /// <summary>
        /// ポップアップ表示用の整形済みテキストを返します。
        /// </summary>
        public string ToDisplayText()
        {
            if (string.IsNullOrWhiteSpace(Speaker))
            {
                return Text;
            }

            return $"{Speaker}: {Text}";
        }
    }
}
