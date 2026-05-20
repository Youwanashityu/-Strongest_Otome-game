using System.Collections.Generic;
using Kutsuroideke.AdmissionSelection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// DAY進行、キャラクター選択、エンド分岐、終了後ボタンを管理します。
    /// </summary>
    public sealed class ScenarioFlowManager : MonoBehaviour
    {
        private const string SaveKey = "KutsuroidekeScenarioSave";

        [Header("参照")]
        [SerializeField] private ScenarioCsvLoader csvLoader;
        [SerializeField] private NovelManager novelManager;
        [SerializeField] private ScenarioVariableResolver variableResolver;
        [SerializeField] private List<CharacterProfileData> allCharacterProfiles = new();

        [Header("キャラクター選択UI")]
        [SerializeField] private GameObject characterSelectPanel;
        [SerializeField] private TMP_Text daySelectText;
        [SerializeField] private Button[] characterButtons;
        [SerializeField] private TMP_Text[] characterButtonLabels;

        [Header("終了後UI")]
        [SerializeField] private GameObject scenarioEndPanel;
        [SerializeField] private string titleSceneName = "TitleScene";
        [SerializeField] private string defaultEndingKey = "A";

        private readonly List<SelectedCharacterInfo> admittedInfos = new();
        private readonly List<CharacterProfileData> admittedProfiles = new();
        private readonly List<ScenarioBacklogEntry> backlog = new();

        private ScenarioCsvTable currentTable;
        private ScenarioCsvRow currentRow;
        private ScenarioCheckpoint lastChoiceCheckpoint;
        private CharacterProfileData currentCharacter;
        private SelectedCharacterInfo currentSelectedInfo;
        private int currentRowIndex;
        private int currentDay = 1;
        private bool isEnding;
        private string currentEndingKey = "";

        public IReadOnlyList<ScenarioBacklogEntry> Backlog => backlog;

        /// <summary>
        /// NovelManagerの入力イベントを購読し、収容済みキャラからDAY1を開始します。
        /// </summary>
        private void Start()
        {
            if (novelManager != null)
            {
                novelManager.AdvanceRequested += HandleAdvanceRequested;
                novelManager.ChoiceSelected += HandleChoiceSelected;
            }

            CacheAdmittedCharacters();
            SetScenarioEndPanel(false);
            ShowCharacterSelect();
        }

        /// <summary>
        /// ボタンから呼び出し、現在位置をPlayerPrefsへ保存します。
        /// </summary>
        public void SaveCurrentPosition()
        {
            if (currentTable == null || currentRow == null || currentCharacter == null)
            {
                return;
            }

            ScenarioCheckpoint checkpoint = CreateCurrentCheckpoint();
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(checkpoint));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// ボタンから呼び出し、保存済みのCSV名とIDへ戻ります。
        /// </summary>
        public void LoadSavedPosition()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                return;
            }

            ScenarioCheckpoint checkpoint = JsonUtility.FromJson<ScenarioCheckpoint>(PlayerPrefs.GetString(SaveKey));
            RestoreCheckpoint(checkpoint);
        }

        /// <summary>
        /// エンド後ボタンから呼び出し、最後の選択肢前へ戻ります。
        /// </summary>
        public void RetryFromLastChoice()
        {
            RestoreCheckpoint(lastChoiceCheckpoint);
        }

        /// <summary>
        /// エンド後ボタンから呼び出し、シナリオをDAY1の選択からやり直します。
        /// </summary>
        public void RestartScenario()
        {
            currentDay = 1;
            isEnding = false;
            currentEndingKey = "";
            currentTable = null;
            currentRow = null;
            currentCharacter = null;
            currentSelectedInfo = null;
            backlog.Clear();
            SetScenarioEndPanel(false);
            ShowCharacterSelect();
        }

        /// <summary>
        /// エンド後ボタンから呼び出し、タイトルシーンへ遷移します。
        /// </summary>
        public void LoadTitleScene()
        {
            if (!string.IsNullOrWhiteSpace(titleSceneName))
            {
                SceneManager.LoadScene(titleSceneName);
            }
        }

        /// <summary>
        /// インビジブルボタンから呼び出し、ノベルUIの表示状態を切り替えます。
        /// </summary>
        public void SetNovelWindowVisible(bool isVisible)
        {
            if (novelManager != null)
            {
                novelManager.SetNovelWindowVisible(isVisible);
            }
        }

        /// <summary>
        /// 選択シーンで収容した3人をIDからキャラクターデータへ変換します。
        /// </summary>
        private void CacheAdmittedCharacters()
        {
            admittedInfos.Clear();
            admittedProfiles.Clear();

            if (AdmissionGameStateManager.Instance == null)
            {
                Debug.LogWarning("ScenarioFlowManager: AdmissionGameStateManagerが見つかりません。");
                return;
            }

            foreach (SelectedCharacterInfo info in AdmissionGameStateManager.Instance.AdmittedCharacters)
            {
                CharacterProfileData profile = FindProfile(info.CharacterId);
                if (profile == null)
                {
                    continue;
                }

                admittedInfos.Add(info);
                admittedProfiles.Add(profile);
            }
        }

        /// <summary>
        /// 現在の日数に対する会話相手選択UIを表示します。
        /// </summary>
        private void ShowCharacterSelect()
        {
            SetCharacterSelectPanel(true);

            if (daySelectText != null)
            {
                daySelectText.text = $"DAY{currentDay} 誰のところへ行こうか";
            }

            for (int i = 0; i < characterButtons.Length; i++)
            {
                int index = i;
                bool active = i < admittedProfiles.Count;

                characterButtons[i].gameObject.SetActive(active);
                characterButtons[i].onClick.RemoveAllListeners();

                if (active)
                {
                    characterButtons[i].onClick.AddListener(() => SelectDayCharacter(index));
                    SetButtonLabel(i, admittedProfiles[i].DisplayName);
                }
            }
        }

        /// <summary>
        /// DAY選択で選ばれたキャラクターのCSVを読み込みます。
        /// </summary>
        private void SelectDayCharacter(int admittedIndex)
        {
            currentCharacter = admittedProfiles[admittedIndex];
            currentSelectedInfo = admittedInfos[admittedIndex];
            TextAsset csv = currentCharacter.GetDayCsv(currentDay);

            if (csv == null)
            {
                Debug.LogError($"{currentCharacter.DisplayName} のDAY{currentDay} CSVが未設定です。");
                return;
            }

            isEnding = false;
            currentEndingKey = "";
            LoadCsv(csv);
            SetCharacterSelectPanel(false);
            DisplayCurrentRow();
        }

        /// <summary>
        /// 現在行に選択肢がなければ、次の行またはCommand処理へ進みます。
        /// </summary>
        private void HandleAdvanceRequested()
        {
            if (currentRow == null || currentRow.HasChoices())
            {
                return;
            }

            if (HandleCommandIfNeeded())
            {
                return;
            }

            MoveNextRow();
        }

        /// <summary>
        /// 選択肢のジャンプ先IDへ移動します。
        /// </summary>
        private void HandleChoiceSelected(string nextId)
        {
            if (string.IsNullOrWhiteSpace(nextId) || currentTable == null)
            {
                return;
            }

            if (currentTable.TryGetById(nextId, out ScenarioCsvRow row, out int index))
            {
                currentRow = row;
                currentRowIndex = index;
                DisplayCurrentRow();
            }
        }

        /// <summary>
        /// 現在行のCommandが特殊処理なら実行し、処理済みかを返します。
        /// </summary>
        private bool HandleCommandIfNeeded()
        {
            switch (currentRow.Command)
            {
                case ScenarioCommand.EndCsv:
                    FinishDayCsv();
                    return true;
                case ScenarioCommand.EndBranch:
                    StartEndingBranch();
                    return true;
                case ScenarioCommand.ScenarioEnd:
                    FinishScenario();
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 通常の次行へ進み、次行がなければCSV終了扱いにします。
        /// </summary>
        private void MoveNextRow()
        {
            if (currentTable.TryGetNext(currentRowIndex, out ScenarioCsvRow row, out int index))
            {
                currentRow = row;
                currentRowIndex = index;
                DisplayCurrentRow();
                return;
            }

            FinishDayCsv();
        }

        /// <summary>
        /// DAY1/2なら翌日へ、DAY3ならエンド分岐へ進みます。
        /// </summary>
        private void FinishDayCsv()
        {
            if (isEnding)
            {
                FinishScenario();
                return;
            }

            if (currentDay >= 3)
            {
                StartEndingBranch();
                return;
            }

            currentDay++;
            ShowCharacterSelect();
        }

        /// <summary>
        /// 3日目に選んだキャラクターのエンドCSVを読み込みます。
        /// </summary>
        private void StartEndingBranch()
        {
            currentEndingKey = ResolveEndingKey();
            TextAsset endingCsv = currentCharacter.GetEndingCsv(currentEndingKey);

            if (endingCsv == null)
            {
                Debug.LogError($"{currentCharacter.DisplayName} のエンドCSV {currentEndingKey} が未設定です。");
                return;
            }

            isEnding = true;
            LoadCsv(endingCsv);
            DisplayCurrentRow();
        }

        /// <summary>
        /// エンド分岐条件を決定します。好感度などを入れるまでは既定値を返します。
        /// </summary>
        private string ResolveEndingKey()
        {
            return string.IsNullOrWhiteSpace(defaultEndingKey) ? "A" : defaultEndingKey;
        }

        /// <summary>
        /// エンドCSV終了後のボタン群を表示します。
        /// </summary>
        private void FinishScenario()
        {
            SetScenarioEndPanel(true);
        }

        /// <summary>
        /// TextAssetを読み込み、先頭行を現在行にします。
        /// </summary>
        private void LoadCsv(TextAsset csv)
        {
            currentTable = csvLoader.Load(csv);
            currentTable.TryGetByIndex(0, out currentRow);
            currentRowIndex = 0;
        }

        /// <summary>
        /// 現在行を画面へ表示し、バックログとリトライ地点を更新します。
        /// </summary>
        private void DisplayCurrentRow()
        {
            string resolvedText = variableResolver.Resolve(currentRow.Text, currentDay, currentCharacter, currentSelectedInfo);
            novelManager.DisplayRow(currentRow, resolvedText, $"DAY{currentDay}", currentCharacter.Portrait);
            backlog.Add(new ScenarioBacklogEntry(currentRow.Speaker, resolvedText));

            if (currentRow.HasChoices())
            {
                lastChoiceCheckpoint = CreateCurrentCheckpoint();
            }
        }

        /// <summary>
        /// 現在のCSV位置からチェックポイントを作成します。
        /// </summary>
        private ScenarioCheckpoint CreateCurrentCheckpoint()
        {
            return new ScenarioCheckpoint(
                currentTable.CsvKey,
                currentRow.Id,
                currentRowIndex,
                currentDay,
                currentCharacter.CharacterId,
                isEnding,
                currentEndingKey
            );
        }

        /// <summary>
        /// 保存済みチェックポイントをもとにCSVと行位置を復元します。
        /// </summary>
        private void RestoreCheckpoint(ScenarioCheckpoint checkpoint)
        {
            if (checkpoint == null || !checkpoint.IsValid())
            {
                return;
            }

            currentDay = checkpoint.Day;
            currentCharacter = FindProfile(checkpoint.CharacterId);
            currentSelectedInfo = FindSelectedInfo(checkpoint.CharacterId);
            isEnding = checkpoint.IsEnding;
            currentEndingKey = checkpoint.EndingKey;

            TextAsset csv = isEnding
                ? currentCharacter.GetEndingCsv(currentEndingKey)
                : currentCharacter.GetDayCsv(currentDay);

            LoadCsv(csv);

            if (currentTable.TryGetById(checkpoint.RowId, out ScenarioCsvRow row, out int index))
            {
                currentRow = row;
                currentRowIndex = index;
            }

            SetScenarioEndPanel(false);
            SetCharacterSelectPanel(false);
            DisplayCurrentRow();
        }

        /// <summary>
        /// CharacterIdに一致するキャラクターデータを探します。
        /// </summary>
        private CharacterProfileData FindProfile(string characterId)
        {
            for (int i = 0; i < allCharacterProfiles.Count; i++)
            {
                if (allCharacterProfiles[i] != null && allCharacterProfiles[i].CharacterId == characterId)
                {
                    return allCharacterProfiles[i];
                }
            }

            return null;
        }

        /// <summary>
        /// CharacterIdに一致する収容済み選択情報を探します。
        /// </summary>
        private SelectedCharacterInfo FindSelectedInfo(string characterId)
        {
            for (int i = 0; i < admittedInfos.Count; i++)
            {
                if (admittedInfos[i].CharacterId == characterId)
                {
                    return admittedInfos[i];
                }
            }

            return null;
        }

        /// <summary>
        /// キャラクター選択ボタンのラベルを安全に更新します。
        /// </summary>
        private void SetButtonLabel(int index, string label)
        {
            if (index < characterButtonLabels.Length && characterButtonLabels[index] != null)
            {
                characterButtonLabels[index].text = label;
            }
        }

        /// <summary>
        /// キャラクター選択パネルの表示状態を切り替えます。
        /// </summary>
        private void SetCharacterSelectPanel(bool isActive)
        {
            if (characterSelectPanel != null)
            {
                characterSelectPanel.SetActive(isActive);
            }
        }

        /// <summary>
        /// シナリオ終了後パネルの表示状態を切り替えます。
        /// </summary>
        private void SetScenarioEndPanel(bool isActive)
        {
            if (scenarioEndPanel != null)
            {
                scenarioEndPanel.SetActive(isActive);
            }
        }
    }
}
