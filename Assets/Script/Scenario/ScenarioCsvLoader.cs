using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// TextAssetのCSVをScenarioCsvTableへ変換するだけの読み込み担当です。
    /// </summary>
    public sealed class ScenarioCsvLoader : MonoBehaviour
    {
        private static readonly Dictionary<string, int> HeaderIndex = new();

        /// <summary>
        /// CSVファイルを読み込み、ID検索可能なシナリオテーブルに変換します。
        /// </summary>
        public ScenarioCsvTable Load(TextAsset csvAsset)
        {
            if (csvAsset == null)
            {
                Debug.LogError("ScenarioCsvLoader: CSVが未設定です。");
                return new ScenarioCsvTable("", new List<ScenarioCsvRow>());
            }

            List<List<string>> records = ParseCsv(csvAsset.text);
            if (records.Count <= 1)
            {
                Debug.LogWarning($"ScenarioCsvLoader: {csvAsset.name} にデータ行がありません。");
                return new ScenarioCsvTable(csvAsset.name, new List<ScenarioCsvRow>());
            }

            BuildHeaderIndex(records[0]);
            List<ScenarioCsvRow> rows = new();

            for (int i = 1; i < records.Count; i++)
            {
                ScenarioCsvRow row = BuildRow(records[i]);

                if (!string.IsNullOrWhiteSpace(row.Id))
                {
                    rows.Add(row);
                }
            }

            return new ScenarioCsvTable(csvAsset.name, rows);
        }

        /// <summary>
        /// 1レコードをScenarioCsvRowへ変換します。
        /// </summary>
        private ScenarioCsvRow BuildRow(IReadOnlyList<string> record)
        {
            return new ScenarioCsvRow
            {
                Id = Get(record, "ID"),
                Speaker = Get(record, "Speaker"),
                Expression = Get(record, "Expression"),
                ColorCode = Get(record, "Color"),
                Still = GetEither(record, "スチル", "Still"),
                Text = Get(record, "Text"),
                Choice1 = Get(record, "Choice1"),
                Choice1NextId = Get(record, "Choice1NextID"),
                Choice2 = Get(record, "Choice2"),
                Choice2NextId = Get(record, "Choice2NextID"),
                Command = ParseCommand(Get(record, "Command"))
            };
        }

        /// <summary>
        /// ヘッダー名から列番号を引けるように辞書を作り直します。
        /// </summary>
        private void BuildHeaderIndex(IReadOnlyList<string> headers)
        {
            HeaderIndex.Clear();

            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i].Trim();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    HeaderIndex[header] = i;
                }
            }
        }

        /// <summary>
        /// 指定ヘッダーの値を安全に取得します。
        /// </summary>
        private string Get(IReadOnlyList<string> record, string header)
        {
            if (!HeaderIndex.TryGetValue(header, out int index) || index >= record.Count)
            {
                return "";
            }

            return record[index].Trim();
        }

        /// <summary>
        /// 日本語列名と英語列名のどちらでも同じ値を取得できるようにします。
        /// </summary>
        private string GetEither(IReadOnlyList<string> record, string primaryHeader, string secondaryHeader)
        {
            string primaryValue = Get(record, primaryHeader);
            if (!string.IsNullOrWhiteSpace(primaryValue))
            {
                return primaryValue;
            }

            return Get(record, secondaryHeader);
        }

        /// <summary>
        /// Command列の文字列を列挙型へ変換します。
        /// </summary>
        private ScenarioCommand ParseCommand(string command)
        {
            switch (command)
            {
                case "EndCsv":
                    return ScenarioCommand.EndCsv;
                case "EndBranch":
                    return ScenarioCommand.EndBranch;
                case "ScenarioEnd":
                    return ScenarioCommand.ScenarioEnd;
                default:
                    return ScenarioCommand.None;
            }
        }

        /// <summary>
        /// ダブルクォートとカンマを考慮してCSV文字列をレコードへ分解します。
        /// </summary>
        private List<List<string>> ParseCsv(string csvText)
        {
            List<List<string>> records = new();
            List<string> currentRecord = new();
            StringBuilder currentCell = new();
            bool inQuotes = false;

            for (int i = 0; i < csvText.Length; i++)
            {
                char c = csvText[i];

                if (c == '"' && i + 1 < csvText.Length && csvText[i + 1] == '"')
                {
                    currentCell.Append('"');
                    i++;
                }
                else if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    currentRecord.Add(currentCell.ToString());
                    currentCell.Clear();
                }
                else if ((c == '\n' || c == '\r') && !inQuotes)
                {
                    AddRecord(records, currentRecord, currentCell);
                    if (c == '\r' && i + 1 < csvText.Length && csvText[i + 1] == '\n')
                    {
                        i++;
                    }
                }
                else
                {
                    currentCell.Append(c);
                }
            }

            AddRecord(records, currentRecord, currentCell);
            return records;
        }

        /// <summary>
        /// 読み取り中のセルとレコードを確定し、空行でなければ一覧へ追加します。
        /// </summary>
        private void AddRecord(List<List<string>> records, List<string> currentRecord, StringBuilder currentCell)
        {
            currentRecord.Add(currentCell.ToString());
            currentCell.Clear();

            if (currentRecord.Count > 1 || !string.IsNullOrWhiteSpace(currentRecord[0]))
            {
                records.Add(new List<string>(currentRecord));
            }

            currentRecord.Clear();
        }
    }
}
