using UnityEngine;
using UnityEngine.UI;

namespace Othello
{
    /// <summary>
    /// 石。黒白画像を切り替えて使う。
    /// </summary>
    public class Disc : MonoBehaviour
    {
        [SerializeField] Sprite[] discSprites;
        [SerializeField] ReverseAnimation reverse;
        [SerializeField] BoundAnimation bound;
        [SerializeField] Image image;
        DiscType discType;
        int reverseIdx;

        public ReverseAnimation Reverse => reverse;
        public BoundAnimation Bound     => bound;
        public DiscType DiscType        => discType;
        public int ReverseIdx { get => reverseIdx; set => reverseIdx = value; }

        /// <summary>
        /// 石を初期化
        /// </summary>
        /// <param name="discType">石タイプ</param>
        public void Init(DiscType discType)
        {
            SetDiscType(discType);
        }

        /// <summary>
        /// 石タイプを設定
        /// </summary>
        /// <param name="discType">石タイプ</param>
        public void SetDiscType(DiscType discType)
        {
            this.discType = discType;
            name          = discType.ToString();
            image.sprite  = discSprites[(int)discType];
        }
    }
}