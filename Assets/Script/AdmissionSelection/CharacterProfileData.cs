using UnityEngine;

namespace Kutsuroideke.AdmissionSelection
{
    /// <summary>
    /// 次のシナリオシーンへ渡す、収容決定済みキャラクターの軽量データです。
    /// </summary>
    [System.Serializable]
    public sealed class SelectedCharacterInfo
    {
        public string CharacterId;
        public string DisplayName;
        public string SignedManagerName;

        /// <summary>
        /// JsonUtilityやインスペクター互換のために空コンストラクタを残します。
        /// </summary>
        public SelectedCharacterInfo()
        {
        }

        /// <summary>
        /// 選択済みキャラクター情報を初期化します。
        /// </summary>
        public SelectedCharacterInfo(string characterId, string displayName, string signedManagerName)
        {
            CharacterId = characterId;
            DisplayName = displayName;
            SignedManagerName = signedManagerName;
        }

        /// <summary>
        /// 次シーンの選択肢に出せるだけの情報が揃っているかを返します。
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(CharacterId)
                && !string.IsNullOrWhiteSpace(DisplayName)
                && !string.IsNullOrWhiteSpace(SignedManagerName);
        }
    }

    /// <summary>
    /// 攻略対象選択とシナリオ進行で使う、キャラクター単位の設定データです。
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterProfileData", menuName = "Kutsuroideke/Character Profile")]
    public sealed class CharacterProfileData : ScriptableObject
    {
        [Header("識別情報")]
        [SerializeField] private string characterId = "";
        [SerializeField] private string displayName = "";

        [Header("画像")]
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Sprite portrait;
        [SerializeField] private Sprite resumeSprite;

        [Header("シナリオCSV")]
        [SerializeField] private TextAsset day1Csv;
        [SerializeField] private TextAsset day2Csv;
        [SerializeField] private TextAsset day3Csv;
        [SerializeField] private TextAsset endingCsvA;
        [SerializeField] private TextAsset endingCsvB;

        [Header("履歴書情報")]
        [TextArea(1, 2)]
        [SerializeField] private string crimeSummary = "";
        [TextArea(2, 5)]
        [SerializeField] private string publicRecord = "";
        [TextArea(2, 5)]
        [SerializeField] private string personalityNote = "";
        [TextArea(2, 5)]
        [SerializeField] private string admissionReason = "";

        [Header("表示補助")]
        [SerializeField] private Color accentColor = Color.white;

        public string CharacterId => characterId;
        public string DisplayName => displayName;
        public Sprite BackgroundSprite => backgroundSprite;
        public Sprite Portrait => portrait;
        public Sprite ResumeSprite => resumeSprite;
        public TextAsset Day1Csv => day1Csv;
        public TextAsset Day2Csv => day2Csv;
        public TextAsset Day3Csv => day3Csv;
        public TextAsset EndingCsvA => endingCsvA;
        public TextAsset EndingCsvB => endingCsvB;
        public string CrimeSummary => crimeSummary;
        public string PublicRecord => publicRecord;
        public string PersonalityNote => personalityNote;
        public string AdmissionReason => admissionReason;
        public Color AccentColor => accentColor;

        /// <summary>
        /// 履歴書データとして最低限必要なIDと表示名が入力済みかを返します。
        /// </summary>
        public bool HasRequiredProfile()
        {
            return !string.IsNullOrWhiteSpace(characterId)
                && !string.IsNullOrWhiteSpace(displayName);
        }

        /// <summary>
        /// 指定された日数に対応する会話CSVを返します。
        /// </summary>
        public TextAsset GetDayCsv(int day)
        {
            switch (day)
            {
                case 1:
                    return day1Csv;
                case 2:
                    return day2Csv;
                case 3:
                    return day3Csv;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 指定されたエンド種別に対応するエンドCSVを返します。
        /// </summary>
        public TextAsset GetEndingCsv(string endingKey)
        {
            switch (endingKey)
            {
                case "A":
                    return endingCsvA;
                case "B":
                    return endingCsvB;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 選択画面の検索やデバッグ表示で使いやすい結合済みテキストを返します。
        /// </summary>
        public string BuildSearchText()
        {
            return string.Join(
                " ",
                characterId,
                displayName,
                crimeSummary,
                publicRecord,
                personalityNote,
                admissionReason
            );
        }

        /// <summary>
        /// インスペクター上で見分けやすい簡易ラベルを返します。
        /// </summary>
        public string BuildDebugLabel()
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return characterId;
            }

            if (string.IsNullOrWhiteSpace(characterId))
            {
                return displayName;
            }

            return $"{displayName} ({characterId})";
        }

        /// <summary>
        /// シーン遷移後に保持する最低限の選択情報へ変換します。
        /// </summary>
        public SelectedCharacterInfo ToSelectedInfo(string signedManagerName)
        {
            return new SelectedCharacterInfo(characterId, displayName, signedManagerName);
        }

        /// <summary>
        /// ScriptableObject編集時にIDと表示名の前後空白を落とします。
        /// </summary>
        private void OnValidate()
        {
            characterId = characterId == null ? "" : characterId.Trim();
            displayName = displayName == null ? "" : displayName.Trim();
        }
    }
}
