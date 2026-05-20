using System;
using UnityEngine;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// シナリオCSVの1行分を表すデータです。
    /// </summary>
    [Serializable]
    public sealed class ScenarioCsvRow
    {
        public string Id;
        public string Speaker;
        public string Expression;
        public string ColorCode;
        public string Still;
        public string Text;
        public string Choice1;
        public string Choice1NextId;
        public string Choice2;
        public string Choice2NextId;
        public ScenarioCommand Command;

        /// <summary>
        /// この行に選択肢が1つ以上あるかを返します。
        /// </summary>
        public bool HasChoices()
        {
            return !string.IsNullOrWhiteSpace(Choice1)
                || !string.IsNullOrWhiteSpace(Choice2);
        }

        /// <summary>
        /// カラーコードをUnityのColorへ変換できた場合だけtrueを返します。
        /// </summary>
        public bool TryGetColor(out Color parsedColor)
        {
            if (string.IsNullOrWhiteSpace(ColorCode))
            {
                parsedColor = UnityEngine.Color.white;
                return false;
            }

            return ColorUtility.TryParseHtmlString(ColorCode, out parsedColor);
        }
    }
}
