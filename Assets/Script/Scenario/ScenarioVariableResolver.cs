using Kutsuroideke.AdmissionSelection;
using UnityEngine;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// CSV本文に含まれる {managerName} などの変数を現在の値へ置換します。
    /// </summary>
    public sealed class ScenarioVariableResolver : MonoBehaviour
    {
        [SerializeField] private string fallbackManagerName = "管理人";

        /// <summary>
        /// 現在のシナリオ状態を使って本文中の変数を置換します。
        /// </summary>
        public string Resolve(
            string sourceText,
            int day,
            CharacterProfileData currentCharacter,
            SelectedCharacterInfo selectedInfo
        )
        {
            string result = sourceText ?? "";
            string managerName = GetManagerName(selectedInfo);
            string characterName = currentCharacter == null ? "" : currentCharacter.DisplayName;

            result = result.Replace("{managerName}", managerName);
            result = result.Replace("{day}", $"DAY{day}");
            result = result.Replace("{characterName}", characterName);
            return result;
        }

        /// <summary>
        /// 選択シーンで入力された担当者名を取得し、空なら予備名を返します。
        /// </summary>
        private string GetManagerName(SelectedCharacterInfo selectedInfo)
        {
            if (selectedInfo != null && !string.IsNullOrWhiteSpace(selectedInfo.SignedManagerName))
            {
                return selectedInfo.SignedManagerName;
            }

            return fallbackManagerName;
        }
    }
}
