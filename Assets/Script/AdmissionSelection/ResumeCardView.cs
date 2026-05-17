using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kutsuroideke.AdmissionSelection
{
    /// <summary>
    /// 1枚の履歴書UIにキャラクター情報を反映し、担当者欄クリックを通知します。
    /// </summary>
    public sealed class ResumeCardView : MonoBehaviour
    {
        [Header("基本表示")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text crimeText;
        [SerializeField] private TMP_Text publicRecordText;
        [SerializeField] private TMP_Text personalityNoteText;
        [SerializeField] private TMP_Text admissionReasonText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Image accentImage;

        [Header("担当者欄")]
        [SerializeField] private Button managerFieldButton;
        [SerializeField] private TMP_Text managerNameText;
        [SerializeField] private GameObject approvedStamp;

        public UnityEvent<ResumeCardView> ManagerFieldClicked = new();

        public CharacterProfileData Profile { get; private set; }
        public bool IsSigned { get; private set; }
        public string SignedManagerName { get; private set; } = "";

        /// <summary>
        /// 担当者欄のクリックイベントをUnity UIから受け取れるようにします。
        /// </summary>
        private void Awake()
        {
            if (managerFieldButton != null)
            {
                managerFieldButton.onClick.AddListener(HandleManagerFieldClicked);
            }
        }

        /// <summary>
        /// キャラクター履歴書の表示内容をまとめて更新します。
        /// </summary>
        public void Bind(CharacterProfileData profile)
        {
            Profile = profile;
            IsSigned = false;
            SignedManagerName = "";

            SetText(nameText, profile.DisplayName);
            SetText(crimeText, profile.CrimeSummary);
            SetText(publicRecordText, profile.PublicRecord);
            SetText(personalityNoteText, profile.PersonalityNote);
            SetText(admissionReasonText, profile.AdmissionReason);
            SetText(managerNameText, "");

            if (portraitImage != null)
            {
                portraitImage.sprite = profile.Portrait;
                portraitImage.enabled = profile.Portrait != null;
            }

            if (accentImage != null)
            {
                accentImage.color = profile.AccentColor;
            }

            if (approvedStamp != null)
            {
                approvedStamp.SetActive(false);
            }
        }

        /// <summary>
        /// 入力されたプレイヤー名を担当者欄に記入し、承認印を表示します。
        /// </summary>
        public void Sign(string managerName)
        {
            SignedManagerName = managerName;
            IsSigned = true;
            SetText(managerNameText, managerName);

            if (approvedStamp != null)
            {
                approvedStamp.SetActive(true);
            }
        }

        /// <summary>
        /// TMP_Textが未設定でも落ちないよう、安全にテキストを反映します。
        /// </summary>
        private void SetText(TMP_Text targetText, string value)
        {
            if (targetText != null)
            {
                targetText.text = value;
            }
        }

        /// <summary>
        /// 担当者欄が押されたことを、選択シーンManagerへ通知します。
        /// </summary>
        private void HandleManagerFieldClicked()
        {
            ManagerFieldClicked.Invoke(this);
        }
    }
}
