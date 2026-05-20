using System.Text;
using TMPro;
using UnityEngine;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// シナリオ中のセーブ/ロード、バックログ、インビジブル表示を操作するUI担当です。
    /// </summary>
    public sealed class ScenarioOptionManager : MonoBehaviour
    {
        [Header("参照")]
        [SerializeField] private ScenarioFlowManager flowManager;

        [Header("ポップアップ")]
        [SerializeField] private GameObject saveLoadPopup;
        [SerializeField] private GameObject backlogPopup;
        [SerializeField] private TMP_Text backlogText;

        private bool isNovelWindowVisible = true;

        /// <summary>
        /// 初期状態では各ポップアップを閉じておきます。
        /// </summary>
        private void Start()
        {
            SetSaveLoadPopup(false);
            SetBacklogPopup(false);
        }

        /// <summary>
        /// セーブボタンから呼び出し、セーブ/ロード選択ポップアップを開閉します。
        /// </summary>
        public void ToggleSaveLoadPopup()
        {
            if (saveLoadPopup != null)
            {
                saveLoadPopup.SetActive(!saveLoadPopup.activeSelf);
            }
        }

        /// <summary>
        /// セーブ/ロードポップアップ内のセーブボタンから呼び出します。
        /// </summary>
        public void SaveCurrentPosition()
        {
            if (flowManager != null)
            {
                flowManager.SaveCurrentPosition();
            }

            SetSaveLoadPopup(false);
        }

        /// <summary>
        /// セーブ/ロードポップアップ内のロードボタンから呼び出します。
        /// </summary>
        public void LoadSavedPosition()
        {
            if (flowManager != null)
            {
                flowManager.LoadSavedPosition();
            }

            SetSaveLoadPopup(false);
        }

        /// <summary>
        /// バックログボタンから呼び出し、表示済み本文の履歴を開閉します。
        /// </summary>
        public void ToggleBacklogPopup()
        {
            if (backlogPopup == null)
            {
                return;
            }

            bool nextActive = !backlogPopup.activeSelf;
            backlogPopup.SetActive(nextActive);

            if (nextActive)
            {
                RefreshBacklogText();
            }
        }

        /// <summary>
        /// インビジブルボタンから呼び出し、テキストウィンドウとフレームを表示/非表示にします。
        /// </summary>
        public void ToggleInvisible()
        {
            isNovelWindowVisible = !isNovelWindowVisible;

            if (flowManager != null)
            {
                flowManager.SetNovelWindowVisible(isNovelWindowVisible);
            }
        }

        /// <summary>
        /// 任意の閉じるボタンから呼び出し、全ポップアップを閉じます。
        /// </summary>
        public void CloseAllPopups()
        {
            SetSaveLoadPopup(false);
            SetBacklogPopup(false);
        }

        /// <summary>
        /// FlowManagerが保持するバックログをTextMeshProへ整形して流し込みます。
        /// </summary>
        private void RefreshBacklogText()
        {
            if (flowManager == null || backlogText == null)
            {
                return;
            }

            StringBuilder builder = new();

            foreach (ScenarioBacklogEntry entry in flowManager.Backlog)
            {
                builder.AppendLine(entry.ToDisplayText());
                builder.AppendLine();
            }

            backlogText.text = builder.ToString();
        }

        /// <summary>
        /// セーブ/ロードポップアップの表示状態を切り替えます。
        /// </summary>
        private void SetSaveLoadPopup(bool isActive)
        {
            if (saveLoadPopup != null)
            {
                saveLoadPopup.SetActive(isActive);
            }
        }

        /// <summary>
        /// バックログポップアップの表示状態を切り替えます。
        /// </summary>
        private void SetBacklogPopup(bool isActive)
        {
            if (backlogPopup != null)
            {
                backlogPopup.SetActive(isActive);
            }
        }
    }
}
