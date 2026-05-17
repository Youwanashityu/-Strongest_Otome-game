using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Kutsuroideke.AdmissionSelection
{
    /// <summary>
    /// 攻略対象選択シーンからシナリオシーンへ、収容者3人の選択結果を保持するManagerです。
    /// </summary>
    public sealed class AdmissionGameStateManager : MonoBehaviour
    {
        public const int AdmissionCapacity = 3;

        private static AdmissionGameStateManager instance;

        [SerializeField] private bool persistAcrossScenes = true;

        private readonly List<SelectedCharacterInfo> admittedCharacters = new();

        public static AdmissionGameStateManager Instance => instance;
        public ReadOnlyCollection<SelectedCharacterInfo> AdmittedCharacters => admittedCharacters.AsReadOnly();
        public bool IsFull => admittedCharacters.Count >= AdmissionCapacity;

        /// <summary>
        /// シーン内にManagerを1つだけ残し、必要ならシーン遷移後も破棄されないようにします。
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// 指定キャラクターがすでに収容決定済みかを返します。
        /// </summary>
        public bool Contains(string characterId)
        {
            for (int i = 0; i < admittedCharacters.Count; i++)
            {
                if (admittedCharacters[i].CharacterId == characterId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 指定IDの収容者情報を取得できた場合だけtrueを返します。
        /// </summary>
        public bool TryGetAdmittedCharacter(string characterId, out SelectedCharacterInfo selectedInfo)
        {
            for (int i = 0; i < admittedCharacters.Count; i++)
            {
                if (admittedCharacters[i].CharacterId == characterId)
                {
                    selectedInfo = admittedCharacters[i];
                    return true;
                }
            }

            selectedInfo = null;
            return false;
        }

        /// <summary>
        /// シナリオシーンの選択肢生成で使いやすい、収容者IDの配列を返します。
        /// </summary>
        public string[] GetAdmittedCharacterIds()
        {
            string[] ids = new string[admittedCharacters.Count];

            for (int i = 0; i < admittedCharacters.Count; i++)
            {
                ids[i] = admittedCharacters[i].CharacterId;
            }

            return ids;
        }

        /// <summary>
        /// 収容登録できない理由を文字列で返し、UI側の通知に利用できるようにします。
        /// </summary>
        public bool CanAdmit(SelectedCharacterInfo selectedInfo, out string reason)
        {
            if (selectedInfo == null || !selectedInfo.IsValid())
            {
                reason = "選択情報が不足しています。";
                return false;
            }

            if (IsFull)
            {
                reason = "収容枠が上限に達しています。";
                return false;
            }

            if (Contains(selectedInfo.CharacterId))
            {
                reason = "同じ人物はすでに承認済みです。";
                return false;
            }

            reason = "";
            return true;
        }

        /// <summary>
        /// 収容枠に空きがあり、重複していない場合だけキャラクターを追加します。
        /// </summary>
        public bool TryAdmit(SelectedCharacterInfo selectedInfo)
        {
            if (!CanAdmit(selectedInfo, out _))
            {
                return false;
            }

            admittedCharacters.Add(selectedInfo);
            return true;
        }

        /// <summary>
        /// デバッグややり直し用に、現在の収容者選択をすべて破棄します。
        /// </summary>
        public void ClearAdmissions()
        {
            admittedCharacters.Clear();
        }
    }
}
