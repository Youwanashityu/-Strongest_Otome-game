using System.Collections.Generic;
using UnityEngine;

namespace Kutsuroideke.AdmissionSelection
{
    /// <summary>
    /// 履歴書カードを横並びに配置し、中央の選択対象をADキーや矢印ボタンで切り替えます。
    /// </summary>
    public sealed class ResumeCarouselController : MonoBehaviour
    {
        [Header("カード生成")]
        [SerializeField] private ResumeCardView cardPrefab;
        [SerializeField] private RectTransform cardRoot;
        [SerializeField] private List<CharacterProfileData> profiles = new();

        [Header("配置")]
        [SerializeField] private float cardSpacing = 520f;
        [SerializeField] private float sideScale = 0.72f;
        [SerializeField] private float centerScale = 1f;
        [SerializeField] private float layoutLerpSpeed = 12f;

        private readonly List<ResumeCardView> cards = new();
        private int currentIndex;

        public ResumeCardView CurrentCard => cards.Count == 0 ? null : cards[currentIndex];
        public IReadOnlyList<ResumeCardView> Cards => cards;
        public IReadOnlyList<CharacterProfileData> Profiles => profiles;

        /// <summary>
        /// 登録済みプロフィールから履歴書カードを生成します。
        /// </summary>
        public void BuildCards()
        {
            cards.Clear();

            for (int i = 0; i < profiles.Count; i++)
            {
                ResumeCardView card = Instantiate(cardPrefab, cardRoot);
                card.Bind(profiles[i]);
                cards.Add(card);
            }

            currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, cards.Count - 1));
            SnapLayout();
        }

        /// <summary>
        /// 毎フレーム入力を確認し、現在選択中の履歴書へカード配置を近づけます。
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MovePrevious();
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveNext();
            }

            UpdateLayout(Time.deltaTime);
        }

        /// <summary>
        /// 左矢印ボタンから呼び出し、ひとつ前の履歴書を中央にします。
        /// </summary>
        public void MovePrevious()
        {
            if (cards.Count == 0)
            {
                return;
            }

            currentIndex = (currentIndex - 1 + cards.Count) % cards.Count;
        }

        /// <summary>
        /// 右矢印ボタンから呼び出し、ひとつ後ろの履歴書を中央にします。
        /// </summary>
        public void MoveNext()
        {
            if (cards.Count == 0)
            {
                return;
            }

            currentIndex = (currentIndex + 1) % cards.Count;
        }

        /// <summary>
        /// 生成直後など、補間なしで履歴書カードを目的位置に揃えます。
        /// </summary>
        public void SnapLayout()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                RectTransform rect = cards[i].transform as RectTransform;
                rect.anchoredPosition = GetTargetPosition(i);
                rect.localScale = GetTargetScale(i);
            }
        }

        /// <summary>
        /// 現在の選択位置を基準に、履歴書カードを滑らかに並べ替えます。
        /// </summary>
        private void UpdateLayout(float deltaTime)
        {
            float t = 1f - Mathf.Exp(-layoutLerpSpeed * deltaTime);

            for (int i = 0; i < cards.Count; i++)
            {
                RectTransform rect = cards[i].transform as RectTransform;
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, GetTargetPosition(i), t);
                rect.localScale = Vector3.Lerp(rect.localScale, GetTargetScale(i), t);
                cards[i].transform.SetSiblingIndex(i == currentIndex ? cards.Count - 1 : i);
            }
        }

        /// <summary>
        /// 指定カードが中央からどれだけ離れているかをもとに表示位置を計算します。
        /// </summary>
        private Vector2 GetTargetPosition(int index)
        {
            int offset = index - currentIndex;
            return new Vector2(offset * cardSpacing, 0f);
        }

        /// <summary>
        /// 中央カードを大きく、左右の見切れカードを小さく見せるスケールを返します。
        /// </summary>
        private Vector3 GetTargetScale(int index)
        {
            float scale = index == currentIndex ? centerScale : sideScale;
            return Vector3.one * scale;
        }
    }
}
