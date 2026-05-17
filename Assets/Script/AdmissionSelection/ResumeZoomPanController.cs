using UnityEngine;

namespace Kutsuroideke.AdmissionSelection
{
    /// <summary>
    /// マウスホイールによる履歴書拡大と、拡大中のみWASDで閲覧位置を移動する制御です。
    /// </summary>
    public sealed class ResumeZoomPanController : MonoBehaviour
    {
        [SerializeField] private RectTransform zoomTarget;
        [SerializeField] private float minZoom = 1f;
        [SerializeField] private float maxZoom = 2.2f;
        [SerializeField] private float zoomStep = 0.16f;
        [SerializeField] private float panSpeed = 540f;
        [SerializeField] private Vector2 panLimit = new(420f, 300f);

        private float currentZoom = 1f;
        private Vector2 panOffset;
        private RectTransform currentTarget;

        public bool IsZoomed => currentZoom > minZoom + 0.01f;

        /// <summary>
        /// ホイール入力とWASD入力を読み取り、対象履歴書の拡大率と表示位置を更新します。
        /// </summary>
        private void Update()
        {
            if (zoomTarget == null)
            {
                return;
            }

            UpdateZoom();
            UpdatePan(Time.deltaTime);
            ApplyTransform();
        }

        /// <summary>
        /// 外部から現在中央にある履歴書を渡し、ズーム対象を切り替えます。
        /// </summary>
        public void SetZoomTarget(RectTransform target)
        {
            if (currentTarget == target)
            {
                return;
            }

            currentTarget = target;
            zoomTarget = target;
            ResetZoom();
        }

        /// <summary>
        /// 拡大率と移動量を初期値に戻します。
        /// </summary>
        public void ResetZoom()
        {
            currentZoom = minZoom;
            panOffset = Vector2.zero;
            ApplyTransform();
        }

        /// <summary>
        /// マウスホイール量から拡大率を更新し、等倍に戻ったら移動量も戻します。
        /// </summary>
        private void UpdateZoom()
        {
            float wheel = Input.mouseScrollDelta.y;
            if (Mathf.Approximately(wheel, 0f))
            {
                return;
            }

            currentZoom = Mathf.Clamp(currentZoom + wheel * zoomStep, minZoom, maxZoom);

            if (!IsZoomed)
            {
                panOffset = Vector2.zero;
            }
        }

        /// <summary>
        /// 拡大中だけWASD入力をカメラ移動のように扱い、履歴書の閲覧位置を動かします。
        /// </summary>
        private void UpdatePan(float deltaTime)
        {
            if (!IsZoomed)
            {
                return;
            }

            Vector2 input = Vector2.zero;
            input.x = GetAxis(KeyCode.D, KeyCode.A);
            input.y = GetAxis(KeyCode.W, KeyCode.S);

            panOffset += input.normalized * panSpeed * deltaTime;
            panOffset.x = Mathf.Clamp(panOffset.x, -panLimit.x, panLimit.x);
            panOffset.y = Mathf.Clamp(panOffset.y, -panLimit.y, panLimit.y);
        }

        /// <summary>
        /// 2つのキー状態から、正方向・負方向・無入力の値を返します。
        /// </summary>
        private float GetAxis(KeyCode positiveKey, KeyCode negativeKey)
        {
            float value = 0f;

            if (Input.GetKey(positiveKey))
            {
                value += 1f;
            }

            if (Input.GetKey(negativeKey))
            {
                value -= 1f;
            }

            return value;
        }

        /// <summary>
        /// 計算済みの拡大率と移動量をRectTransformに反映します。
        /// </summary>
        private void ApplyTransform()
        {
            if (zoomTarget == null)
            {
                return;
            }

            zoomTarget.localScale = Vector3.one * currentZoom;
            zoomTarget.anchoredPosition = panOffset;
        }
    }
}
