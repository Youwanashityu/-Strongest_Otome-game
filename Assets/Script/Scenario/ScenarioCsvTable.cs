using System.Collections.Generic;
using UnityEngine;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// 読み込み済みCSV行をID検索と順番再生の両方で扱うためのテーブルです。
    /// </summary>
    public sealed class ScenarioCsvTable
    {
        private readonly List<ScenarioCsvRow> rows = new();
        private readonly Dictionary<string, int> idToIndex = new();

        public string CsvKey { get; private set; }
        public int Count => rows.Count;

        /// <summary>
        /// CSV名と行一覧から検索用テーブルを構築します。
        /// </summary>
        public ScenarioCsvTable(string csvKey, IReadOnlyList<ScenarioCsvRow> sourceRows)
        {
            CsvKey = csvKey;

            for (int i = 0; i < sourceRows.Count; i++)
            {
                rows.Add(sourceRows[i]);

                if (!string.IsNullOrWhiteSpace(sourceRows[i].Id))
                {
                    idToIndex[sourceRows[i].Id] = i;
                }
            }
        }

        /// <summary>
        /// 指定インデックスの行を取得できた場合だけtrueを返します。
        /// </summary>
        public bool TryGetByIndex(int index, out ScenarioCsvRow row)
        {
            if (index < 0 || index >= rows.Count)
            {
                row = null;
                return false;
            }

            row = rows[index];
            return true;
        }

        /// <summary>
        /// 指定IDの行とインデックスを取得できた場合だけtrueを返します。
        /// </summary>
        public bool TryGetById(string id, out ScenarioCsvRow row, out int index)
        {
            if (string.IsNullOrWhiteSpace(id) || !idToIndex.TryGetValue(id, out index))
            {
                row = null;
                index = -1;
                return false;
            }

            row = rows[index];
            return true;
        }

        /// <summary>
        /// 現在インデックスの次の行を取得できるかを返します。
        /// </summary>
        public bool TryGetNext(int currentIndex, out ScenarioCsvRow row, out int nextIndex)
        {
            nextIndex = currentIndex + 1;
            return TryGetByIndex(nextIndex, out row);
        }
    }
}
