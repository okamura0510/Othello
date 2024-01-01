using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Othello
{
    /// <summary>
    /// リザルト。石集計や結果表示を行う。
    /// </summary>
    public class Result : MonoBehaviour
    {
        enum Sequence { None, Delay, DiscBound, DiscBounding, End }

        [SerializeField] Player player;
        [SerializeField] Enemy enemy;
        [SerializeField] Board board;
        [SerializeField] Disc playerDisc;
        [SerializeField] TextMeshProUGUI playerDiscCount;
        [SerializeField] Disc enemyDisc;
        [SerializeField] TextMeshProUGUI enemyDiscCount;
        [SerializeField] GameObject back;
        [SerializeField] GameObject[] results;
        [SerializeField] float startDelay = 0.5f;
        [SerializeField] float lastDelay  = 1;
        Sequence sq;
        float time;
        List<Disc> discs = new List<Disc>();
        int discIdx;
        int nowPlayerDiscCount;
        int nowEnemyDiscCount;
        ResultType resultType;

        public bool IsPlaying => (sq != Sequence.None);

        void Update()
        {
            if(sq == Sequence.Delay)
            {
                // 少し待つ
                time += Time.deltaTime;
                if(time >= startDelay)
                {
                    sq = Sequence.DiscBound;
                }
            }
            else if(sq == Sequence.DiscBound)
            {
                // 石をバウンド
                discs[discIdx].Bound.Play();
                sq = Sequence.DiscBounding;
            }
            else if(sq == Sequence.DiscBounding)
            {
                // 石がバウンド中
                if(!discs[discIdx].Bound.IsPlaying)
                {
                    // バウンドが終了したのでその石は非表示
                    discs[discIdx].gameObject.SetActive(false);

                    // 石をカウント
                    if(discs[discIdx].DiscType == player.DiscType)
                    {
                        // プレイヤー
                        nowPlayerDiscCount++;
                        playerDiscCount.text = nowPlayerDiscCount.ToString();
                        playerDisc.Bound.Stop();
                        playerDisc.Bound.Play();
                    }
                    else
                    {
                        // 敵
                        nowEnemyDiscCount++;
                        enemyDiscCount.text = nowEnemyDiscCount.ToString();
                        enemyDisc.Bound.Stop();
                        enemyDisc.Bound.Play();
                    }

                    // 次の石へ
                    discIdx++;
                    if(discIdx < discs.Count)
                    {
                        // 次の石をバウンド
                        sq = Sequence.DiscBound;
                    }
                    else
                    {
                        // 石集計終了。結果確定。
                        if(nowPlayerDiscCount > nowEnemyDiscCount)
                        {
                            resultType = ResultType.Win;
                        }
                        else if(nowPlayerDiscCount < nowEnemyDiscCount)
                        {
                            resultType = ResultType.Lose;
                        }
                        else
                        {
                            resultType = ResultType.Draw;
                        }

                        // 結果表示へ
                        sq   = Sequence.End;
                        time = 0;
                    }
                }
            }
            else if(sq == Sequence.End)
            {
                // 結果表示
                time += Time.deltaTime;
                if(time >= lastDelay)
                {
                    back.SetActive(true);
                    results[(int)resultType].SetActive(true);

                    sq   = Sequence.None;
                    time = 0;
                }
            }
        }

        /// <summary>
        /// リザルトを再生
        /// </summary>
        public void Play()
        {
            // プレイヤーの石置き場を消し、石集計の画像を表示
            player.DiscSpace.gameObject.SetActive(false);
            playerDisc.SetDiscType(player.DiscType);
            playerDisc.gameObject.SetActive(true);

            // 敵の石置き場を消し、石集計の画像を表示
            enemy.DiscSpace.gameObject.SetActive(false);
            enemyDisc.SetDiscType(enemy.DiscType);
            enemyDisc.gameObject.SetActive(true);

            // 全ての石を保存
            for(var y = 0; y < Board.Row; y++)
            {
                for(var x = 0; x < Board.Column; x++)
                {
                    var cell = board.Cells[x, y];
                    if(cell.Disc != null)
                    {
                        discs.Add(cell.Disc);
                    }
                }
            }

            sq = Sequence.Delay;
        }
    }
}