using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Othello
{
    /// <summary>
    /// 敵。プレイヤーと構造は同じだけど敵はAIで動く。
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        public const float DiscPlacementWaitTime = 0.5f;

        [SerializeField] Sprite[] charaSprites;
        [SerializeField] Sprite[] orderSprites;
        [SerializeField] EnemyAI ai;
        [SerializeField] Image chara;
        [SerializeField] BoundAnimation bound;
        [SerializeField] Image order;
        [SerializeField] Transform discSpace;
        DiscType discType;
        Difficulty difficulty;
        List<Disc> discs = new List<Disc>();

        public BoundAnimation Bound => bound;
        public Transform DiscSpace  => discSpace;
        public DiscType DiscType    => discType;
        public List<Disc> Discs     => discs;

        /// <summary>
        /// 敵を初期化
        /// </summary>
        /// <param name="discType">石タイプ</param>
        /// <param name="isFirstTurn">先攻ターンか</param>
        /// <param name="difficulty">難易度</param>
        public void Init(DiscType discType, bool isFirstTurn, Difficulty difficulty)
        {
            this.discType   = discType;
            this.difficulty = difficulty;

            chara.sprite = charaSprites[(int)difficulty];
            order.sprite = orderSprites[isFirstTurn ? 0 : 1];

            // 石を並べる(右にちょっとずつズラす)
            var x = 0f;
            for(var i = 0; i < 30; i++)
            {
                var disc = Instantiate(Resources.Load<Disc>("Prefabs/Disc"), discSpace);
                disc.Init(discType);

                var pos = disc.transform.localPosition;
                pos.x   = x;
                disc.transform.localPosition = pos;

                // 右にズラしていく
                x += Player.DiscLayoutSpacing;
                if(((i + 1) % 5) == 0)
                {
                    // 5個きざみでさらにズラす(5個セットが分かりやすいように)
                    x += Player.DiscLayoutSpacing5;
                }

                disc.transform.eulerAngles = new Vector3(0, 0, -90);
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

        /// <summary>
        /// 反転石リストを取得
        /// </summary>
        /// <param name="selectedCell">セル</param>
        /// <param name="selectedReverseDiscs">反転石リスト</param>
        /// <returns>反転石が見つかったら true</returns>
        public bool TryGetReverseDiscs(out Cell selectedCell, out List<Disc> selectedReverseDiscs)
        {
            if(difficulty == Difficulty.Normal)
            {
                return ai.CalculateNormalAI(out selectedCell, out selectedReverseDiscs);
            }
            else
            {
                return ai.CalculateHardAI(out selectedCell, out selectedReverseDiscs);
            }
        }
    }
}