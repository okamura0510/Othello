using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Othello
{
    /// <summary>
    /// プレイヤー。オーダー表示や手持ちの石の管理など。
    /// </summary>
    public class Player : MonoBehaviour
    {
        public const int DiscLayoutSpacing  = 18;
        public const int DiscLayoutSpacing5 = 15;

        [SerializeField] Sprite[] orderSprites;
        [SerializeField] BoundAnimation bound;
        [SerializeField] Image order;
        [SerializeField] Transform discSpace;
        DiscType discType;
        List<Disc> discs = new List<Disc>();

        public BoundAnimation Bound => bound;
        public Transform DiscSpace  => discSpace;
        public DiscType DiscType    => discType;
        public List<Disc> Discs     => discs;

        /// <summary>
        /// プレイヤーを初期化
        /// </summary>
        /// <param name="discType">石タイプ</param>
        /// <param name="isFirstTurn">先攻ターンか</param>
        public void Init(DiscType discType, bool isFirstTurn)
        {
            this.discType = discType;

            order.sprite = orderSprites[isFirstTurn ? 0 : 1];

            // 石を並べる(左にちょっとずつズラす)
            var x = 0f;
            for(var i = 0; i < 30; i++)
            {
                var disc = Instantiate(Resources.Load<Disc>("Prefabs/Disc"), discSpace);
                disc.Init(discType);

                var pos = disc.transform.localPosition;
                pos.x   = x;
                disc.transform.localPosition = pos;

                // 左にズラしていく
                x -= DiscLayoutSpacing;
                if(((i + 1) % 5) == 0)
                {
                    // 5個きざみでさらにズラす(5個セットが分かりやすいように)
                    x -= DiscLayoutSpacing5;
                }

                disc.transform.eulerAngles = new Vector3(0, 0, 90);
                discs.Add(disc);
            }
        }

        /// <summary>
        /// 次の持ち石を取得
        /// </summary>
        /// <returns>石</returns>
        public Disc GetNextDisc()
        {
            var disc = discs[discs.Count - 1];
            discs.RemoveAt(discs.Count - 1);
            disc.gameObject.SetActive(false);
            return disc;
        }
    }
}