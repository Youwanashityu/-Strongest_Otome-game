using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kutsuroideke.Scenario
{
    /// <summary>
    /// 1行分のシナリオデータを、名前欄・本文・色・選択肢UIへ反映します。
    /// </summary>
    public sealed class NovelManager : MonoBehaviour
    {
        [Header("基本表示")]
        [SerializeField] private TMP_Text dayText;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Image stillImage;
        [SerializeField] private List<Sprite> stillSprites = new();
        [SerializeField] private Image[] accentImages;

        [Header("選択肢")]
        [SerializeField] private Button choice1Button;
        [SerializeField] private TMP_Text choice1Text;
        [SerializeField] private Button choice2Button;
        [SerializeField] private TMP_Text choice2Text;
        [SerializeField] private Button advanceButton;

        [Header("非表示対象")]
        [SerializeField] private GameObject textWindowRoot;
        [SerializeField] private GameObject frameRoot;

        public event Action AdvanceRequested;
        public event Action<string> ChoiceSelected;

        /// <summary>
        /// ボタン入力をScenarioFlowManagerへ通知するためのイベントを登録します。
        /// </summary>
        private void Awake()
        {
            if (advanceButton != null)
            {
                advanceButton.onClick.AddListener(() => AdvanceRequested?.Invoke());
            }

            if (choice1Button != null)
            {
                choice1Button.onClick.AddListener(() => ChoiceSelected?.Invoke(CurrentChoice1NextId));
            }

            if (choice2Button != null)
            {
                choice2Button.onClick.AddListener(() => ChoiceSelected?.Invoke(CurrentChoice2NextId));
            }
        }

        public string CurrentChoice1NextId { get; private set; }
        public string CurrentChoice2NextId { get; private set; }

        /// <summary>
        /// 現在行の内容をノベルUIへ反映します。
        /// </summary>
        public void DisplayRow(ScenarioCsvRow row, string resolvedText, string dayLabel, Sprite background, Sprite portrait)
        {
            SetText(dayText, dayLabel);
            SetText(speakerText, row.Speaker);
            SetText(bodyText, resolvedText);
            SetBackground(background);
            SetPortrait(portrait);
            SetStill(row.Still);
            ApplyAccentColor(row);
            ApplyChoices(row);
        }

        /// <summary>
        /// テキストウィンドウとフレームの表示状態をまとめて切り替えます。
        /// </summary>
        public void SetNovelWindowVisible(bool isVisible)
        {
            if (textWindowRoot != null)
            {
                textWindowRoot.SetActive(isVisible);
            }

            if (frameRoot != null)
            {
                frameRoot.SetActive(isVisible);
            }
        }

        /// <summary>
        /// TMP_Textが未設定でも落ちないよう安全にテキストを反映します。
        /// </summary>
        private void SetText(TMP_Text target, string value)
        {
            if (target != null)
            {
                target.text = value ?? "";
            }
        }

        /// <summary>
        /// キャラクター固定背景を背景Imageへ反映します。
        /// </summary>
        private void SetBackground(Sprite background)
        {
            if (backgroundImage == null)
            {
                return;
            }

            backgroundImage.sprite = background;
            backgroundImage.enabled = background != null;
        }

        /// <summary>
        /// 立ち絵画像がある場合だけImageへ反映します。
        /// </summary>
        private void SetPortrait(Sprite portrait)
        {
            if (portraitImage == null)
            {
                return;
            }

            portraitImage.sprite = portrait;
            portraitImage.enabled = portrait != null;
        }

        /// <summary>
        /// CSVのスチル列に対応する画像を、立ち絵より上のImageへ表示します。
        /// </summary>
        private void SetStill(string stillKey)
        {
            if (stillImage == null)
            {
                return;
            }

            Sprite stillSprite = FindStillSprite(stillKey);
            stillImage.sprite = stillSprite;
            stillImage.color = stillSprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
            stillImage.enabled = true;
        }

        /// <summary>
        /// スチル列の文字列と同名のSpriteを登録済みリストから探します。
        /// </summary>
        private Sprite FindStillSprite(string stillKey)
        {
            if (string.IsNullOrWhiteSpace(stillKey))
            {
                return null;
            }

            for (int i = 0; i < stillSprites.Count; i++)
            {
                if (stillSprites[i] != null && stillSprites[i].name == stillKey)
                {
                    return stillSprites[i];
                }
            }

            Debug.LogWarning($"NovelManager: スチル '{stillKey}' が登録されていません。");
            return null;
        }

        /// <summary>
        /// CSVのColor列を差し色UIへ反映します。
        /// </summary>
        private void ApplyAccentColor(ScenarioCsvRow row)
        {
            if (!row.TryGetColor(out Color parsedColor))
            {
                return;
            }

            for (int i = 0; i < accentImages.Length; i++)
            {
                if (accentImages[i] != null)
                {
                    accentImages[i].color = parsedColor;
                }
            }
        }

        /// <summary>
        /// CSV行に選択肢がある場合だけ選択肢ボタンを表示します。
        /// </summary>
        private void ApplyChoices(ScenarioCsvRow row)
        {
            CurrentChoice1NextId = row.Choice1NextId;
            CurrentChoice2NextId = row.Choice2NextId;
            SetChoice(choice1Button, choice1Text, row.Choice1);
            SetChoice(choice2Button, choice2Text, row.Choice2);
        }

        /// <summary>
        /// 選択肢ボタンの表示状態とラベルを更新します。
        /// </summary>
        private void SetChoice(Button button, TMP_Text label, string text)
        {
            bool hasChoice = !string.IsNullOrWhiteSpace(text);

            if (button != null)
            {
                button.gameObject.SetActive(hasChoice);
            }

            if (label != null)
            {
                label.text = hasChoice ? text : "";
            }
        }
    }
}
