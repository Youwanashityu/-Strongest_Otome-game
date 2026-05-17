using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kutsuroideke.AdmissionSelection
{
    /// <summary>
    /// 攻略対象選択シーンの根幹を担当し、署名確定と次シーン遷移を制御するManagerです。
    /// </summary>
    public sealed class AdmissionSelectionManager : MonoBehaviour
    {
        [Header("参照")]
        [SerializeField] private ResumeCarouselController carouselController;
        [SerializeField] private ResumeZoomPanController zoomPanController;
        [SerializeField] private AdmissionGameStateManager stateManager;

        [Header("署名UI")]
        [SerializeField] private GameObject signPanel;
        [SerializeField] private TMP_InputField managerNameInput;
        [SerializeField] private TMP_Text capacityText;
        [SerializeField] private TMP_Text noticeText;

        [Header("シーン遷移")]
        [SerializeField] private string scenarioSceneName = "ScenarioScene";

        private ResumeCardView pendingSignCard;

        /// <summary>
        /// 必要なManager参照を補完し、履歴書カードを生成します。
        /// </summary>
        private void Start()
        {
            if (stateManager == null)
            {
                stateManager = AdmissionGameStateManager.Instance;
            }

            if (carouselController != null)
            {
                carouselController.BuildCards();
                RegisterCardEvents();
                SyncZoomTarget();
            }

            SetSignPanelActive(false);
            RefreshCapacityText();
        }

        /// <summary>
        /// 中央カードの切り替えに合わせてズーム対象を更新します。
        /// </summary>
        private void Update()
        {
            SyncZoomTarget();
        }

        /// <summary>
        /// 署名パネルの決定ボタンから呼び、担当者名を記入して収容者として登録します。
        /// </summary>
        public void ConfirmSignature()
        {
            if (pendingSignCard == null || pendingSignCard.Profile == null)
            {
                ShowNotice("署名対象の履歴書がありません。");
                return;
            }

            string managerName = managerNameInput == null ? "" : managerNameInput.text.Trim();
            if (string.IsNullOrWhiteSpace(managerName))
            {
                ShowNotice("担当者名を入力してください。");
                return;
            }

            SelectedCharacterInfo selectedInfo = pendingSignCard.Profile.ToSelectedInfo(managerName);
            if (!stateManager.TryAdmit(selectedInfo))
            {
                ShowNotice("この履歴書は承認できません。収容枠または重複を確認してください。");
                return;
            }

            pendingSignCard.Sign(managerName);
            SetSignPanelActive(false);
            RefreshCapacityText();
            ShowNotice($"{pendingSignCard.Profile.DisplayName}を収容者として承認しました。");

            if (stateManager.IsFull)
            {
                LoadScenarioScene();
            }
        }

        /// <summary>
        /// 署名パネルのキャンセルボタンから呼び、現在の署名操作を取り消します。
        /// </summary>
        public void CancelSignature()
        {
            pendingSignCard = null;
            SetSignPanelActive(false);
            ShowNotice("");
        }

        /// <summary>
        /// 矢印ボタン用に、前の履歴書へ移動します。
        /// </summary>
        public void MovePrevious()
        {
            carouselController.MovePrevious();
            SyncZoomTarget();
        }

        /// <summary>
        /// 矢印ボタン用に、次の履歴書へ移動します。
        /// </summary>
        public void MoveNext()
        {
            carouselController.MoveNext();
            SyncZoomTarget();
        }

        /// <summary>
        /// 生成された全履歴書カードの担当者欄クリックを購読します。
        /// </summary>
        private void RegisterCardEvents()
        {
            foreach (ResumeCardView card in carouselController.Cards)
            {
                card.ManagerFieldClicked.AddListener(OpenSignPanel);
            }
        }

        /// <summary>
        /// 担当者欄がクリックされた履歴書を署名対象として保持し、入力欄を開きます。
        /// </summary>
        private void OpenSignPanel(ResumeCardView card)
        {
            if (card == null || card.Profile == null)
            {
                return;
            }

            if (stateManager.Contains(card.Profile.CharacterId))
            {
                ShowNotice("この人物はすでに承認済みです。");
                return;
            }

            pendingSignCard = card;
            SetSignPanelActive(true);

            if (managerNameInput != null)
            {
                managerNameInput.text = "";
                managerNameInput.ActivateInputField();
            }
        }

        /// <summary>
        /// 現在中央にある履歴書を拡大・移動対象として設定します。
        /// </summary>
        private void SyncZoomTarget()
        {
            if (carouselController == null || zoomPanController == null || carouselController.CurrentCard == null)
            {
                return;
            }

            RectTransform target = carouselController.CurrentCard.transform as RectTransform;
            zoomPanController.SetZoomTarget(target);
        }

        /// <summary>
        /// 収容人数表示を現在の承認数に合わせて更新します。
        /// </summary>
        private void RefreshCapacityText()
        {
            if (capacityText != null && stateManager != null)
            {
                capacityText.text = $"収容人数\n{stateManager.AdmittedCharacters.Count}/{AdmissionGameStateManager.AdmissionCapacity}人";
            }
        }

        /// <summary>
        /// 署名入力パネルの表示状態を切り替えます。
        /// </summary>
        private void SetSignPanelActive(bool isActive)
        {
            if (signPanel != null)
            {
                signPanel.SetActive(isActive);
            }
        }

        /// <summary>
        /// 画面上の補足メッセージを更新します。
        /// </summary>
        private void ShowNotice(string message)
        {
            if (noticeText != null)
            {
                noticeText.text = message;
            }
        }

        /// <summary>
        /// 収容者3人の選択結果を保持したまま、次のシナリオシーンへ遷移します。
        /// </summary>
        private void LoadScenarioScene()
        {
            if (!string.IsNullOrWhiteSpace(scenarioSceneName))
            {
                SceneManager.LoadScene(scenarioSceneName);
            }
        }
    }
}
