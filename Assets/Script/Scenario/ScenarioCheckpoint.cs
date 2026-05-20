using System;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// セーブ、ロード、リトライで戻るためのCSV位置情報です。
    /// </summary>
    [Serializable]
    public sealed class ScenarioCheckpoint
    {
        public string CsvKey;
        public string RowId;
        public int RowIndex;
        public int Day;
        public string CharacterId;
        public bool IsEnding;
        public string EndingKey;

        /// <summary>
        /// JsonUtility互換のために空コンストラクタを残します。
        /// </summary>
        public ScenarioCheckpoint()
        {
        }

        /// <summary>
        /// 現在位置からチェックポイントを作成します。
        /// </summary>
        public ScenarioCheckpoint(
            string csvKey,
            string rowId,
            int rowIndex,
            int day,
            string characterId,
            bool isEnding,
            string endingKey
        )
        {
            CsvKey = csvKey;
            RowId = rowId;
            RowIndex = rowIndex;
            Day = day;
            CharacterId = characterId;
            IsEnding = isEnding;
            EndingKey = endingKey;
        }

        /// <summary>
        /// ロード可能なだけの位置情報が入っているかを返します。
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(CsvKey)
                && !string.IsNullOrWhiteSpace(RowId)
                && !string.IsNullOrWhiteSpace(CharacterId);
        }
    }
}
