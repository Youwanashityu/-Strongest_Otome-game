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
        [Header("履歴書画像")]
        [SerializeField] private Image resumeImage;

        [Header("担当者欄")]
        [SerializeField] private Button managerFieldButton;
        [SerializeField] private TMP_Text managerNameText;
        [SerializeField] private GameObject approvedStamp;

        public UnityEvent<ResumeCardView> ManagerFieldClicked = new();

        public CharacterProfileData Profile { get; private set; }
        public bool IsSigned { get; private set; }
        public string SignedManagerName { get; private set; } = "";

        /// <summary>
        /// 担当者欄のクリックイベントを登録します。
        /// </summary>
        private void Awake()
        {
            if (managerFieldButton != null)
            {
                managerFieldButton.onClick.AddListener(HandleManagerFieldClicked);
            }
        }

        /// <summary>
        /// キャラクターの履歴書PNGをカードへ反映します。
        /// </summary>
        public void Bind(CharacterProfileData profile)
        {
            Profile = profile;
            IsSigned = false;
            SignedManagerName = "";

            if (resumeImage != null)
            {
                resumeImage.sprite = profile.ResumeSprite;
                resumeImage.enabled = profile.ResumeSprite != null;
            }

            if (managerNameText != null)
            {
                managerNameText.text = "";
            }

            if (approvedStamp != null)
            {
                approvedStamp.SetActive(false);
            }
        }

        /// <summary>
        /// 入力されたプレイヤー名を担当者欄に表示し、承認印を出します。
        /// </summary>
        public void Sign(string managerName)
        {
            SignedManagerName = managerName;
            IsSigned = true;

            if (managerNameText != null)
            {
                managerNameText.text = managerName;
            }

            if (approvedStamp != null)
            {
                approvedStamp.SetActive(true);
            }
        }

        /// <summary>
        /// 担当者欄が押されたことを選択シーンManagerへ通知します。
        /// </summary>
        private void HandleManagerFieldClicked()
        {
            ManagerFieldClicked.Invoke(this);
        }
    }
}