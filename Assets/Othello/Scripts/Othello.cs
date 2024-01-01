using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Othello
{
    public enum DiscType   { Black,  White }
    public enum Difficulty { Normal, Hard }
    public enum PlayFirst  { Player, Enemy, Random }
    public enum Turn       { Player, Enemy }
    public enum ResultType { Win, Lose, Draw }

    /// <summary>
    /// オセロシーン。ゲームの基本情報を持ってたり、ゲームのイベントはここからスタートする。
    /// </summary>
    public class Othello : MonoBehaviour
    {
        enum Sequence
        {
            None, DiscPlacement, DiscReversing, Pass, PassPlaying, GameEnd, GameEndPlaying, ResultPlaying, ResultEnd
        }

        [SerializeField] Player player;
        [SerializeField] Enemy enemy;
        [SerializeField] Board board;
        [SerializeField] Button restart;
        [SerializeField] InfoAnimation info;
        [SerializeField] Result result;
        [SerializeField] StartMenu startMenu;
        [SerializeField] Difficulty difficulty;
        [SerializeField] PlayFirst playFirst;
        [SerializeField] bool isAssist;
        Turn turn;
        Sequence sq;
        float time;
        float waitTime;
        Disc selectedDisc;
        Cell selectedCell;
        List<Disc> selectedReverseDiscs;
        int passCount;

        public Difficulty Difficulty { get => difficulty; set => difficulty = value; }
        public PlayFirst PlayFirst   { get => playFirst;  set => playFirst  = value; }
        public bool IsAssist         { get => isAssist;   set => isAssist   = value; }

        void Update()
        {
            if(sq == Sequence.DiscPlacement)
            {
                // 石を置く
                time += Time.deltaTime;
                if(time >= waitTime)
                {
                    time = 0;

                    board.PlaceDisc(selectedCell, selectedDisc);
                    foreach(var disc in selectedReverseDiscs)
                    {
                        disc.Reverse.Play(selectedDisc.DiscType);
                    }
                    sq = Sequence.DiscReversing;
                }
            }
            else if(sq == Sequence.DiscReversing)
            {
                // 石が反転中
                var isPlaying = false;
                foreach(var disc in selectedReverseDiscs)
                {
                    if(disc.Reverse.IsPlaying)
                    {
                        isPlaying = true;
                        break;
                    }
                }

                if(!isPlaying)
                {
                    // 反転終了
                    sq = Sequence.None;
                    restart.interactable = true;
                    ChangeTurn();
                }
            }
            else if(sq == Sequence.Pass)
            {
                // パス
                info.PlayPass();
                sq = Sequence.PassPlaying;
            }
            else if(sq == Sequence.PassPlaying)
            {
                // パス中
                if(!info.IsPlaying)
                {
                    // パス終了
                    sq = Sequence.None;
                    restart.interactable = true;
                    ChangeTurn();
                }
            }
            else if(sq == Sequence.GameEnd)
            {
                // ゲーム終了
                info.PlayGameEnd();
                sq = Sequence.GameEndPlaying;
            }
            else if(sq == Sequence.GameEndPlaying)
            {
                // ゲーム終了中
                if(!info.IsPlaying)
                {
                    // リザルトへ
                    result.Play();
                    sq = Sequence.ResultPlaying;
                }
            }
            else if(sq == Sequence.ResultPlaying)
            {
                // リザルト中
                if(!result.IsPlaying)
                {
                    sq = Sequence.ResultEnd;
                }
            }
            else if(sq == Sequence.ResultEnd)
            {
                // 画面タップではじめから
                if(Input.GetMouseButtonUp(0))
                {
                    OnRestartClick();
                }
            }
        }

        /// <summary>
        /// スタートメニューのスタートボタンがクリックされた
        /// </summary>
        public void OnGameStartClick()
        {
            Turn firstTurn;
            var r = Random.Range(0, 2);
            if(playFirst == PlayFirst.Player || (playFirst == PlayFirst.Random && r == 0))
            {
                firstTurn = Turn.Player;
                player.Init(DiscType.Black, true);
                enemy.Init( DiscType.White, false, difficulty);
            }
            else
            {
                firstTurn = Turn.Enemy;
                player.Init(DiscType.White, false);
                enemy.Init( DiscType.Black, true, difficulty);
            }

            startMenu.gameObject.SetActive(false);
            ChangeTurn(firstTurn);
        }

        /// <summary>
        /// はじめからがクリックされた
        /// </summary>
        public void OnRestartClick()
        {
            SceneManager.LoadScene("Othello");
        }

        /// <summary>
        /// セルがクリックされた
        /// </summary>
        /// <param name="cell">セル</param>
        public void OnCellClick(Cell cell)
        {
            if(turn != Turn.Player) return;
            if(sq != Sequence.None) return;

            var reverseDiscs = board.GetReverseDiscs(cell, player.DiscType);
            ExecuteDiscPlacement(cell, reverseDiscs);
        }

        /// <summary>
        /// 石を置く
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="reverseDiscs">反転石リスト</param>
        void ExecuteDiscPlacement(Cell cell, List<Disc> reverseDiscs)
        {
            if(reverseDiscs.Count > 0)
            {
                // 石が置ける
                sq                   = Sequence.DiscPlacement;
                waitTime             = (turn == Turn.Enemy) ? Enemy.DiscPlacementWaitTime : 0;
                selectedCell         = cell;
                selectedReverseDiscs = reverseDiscs;
                restart.interactable = false;
            }
            else
            {
                // 石が置けない
                cell.Ng.Play();
            }
        }

        /// <summary>
        /// ターン変更
        /// </summary>
        void ChangeTurn()
        {
            var nextTurn = (turn == Turn.Player) ? Turn.Enemy : Turn.Player;
            ChangeTurn(nextTurn);
        }

        /// <summary>
        /// ターン変更
        /// </summary>
        /// <param name="nextTurn">次のターン</param>
        void ChangeTurn(Turn nextTurn)
        {
            turn = nextTurn;
            if((player.Discs.Count == 0 && enemy.Discs.Count == 0) || passCount >= 2)
            {
                // ゲーム終了
                board.UpdateAssist(false, DiscType.Black);
                restart.interactable = false;
                sq = Sequence.GameEnd;
            }
            else if(turn == Turn.Player)
            {
                // プレイヤーターン
                board.UpdateAssist(isAssist, player.DiscType);
                player.Bound.Play();

                if(board.CanPlaceDisc(player.DiscType))
                {
                    // 石が置ける
                    passCount = 0;
                    if(enemy.Discs.Count > player.Discs.Count)
                    {
                        // 相手の石を使う(パスで石の数がズレたりする)
                        selectedDisc = enemy.GetNextDisc();
                        selectedDisc.SetDiscType(player.DiscType);
                    }
                    else
                    {
                        // 自分の石を使う
                        selectedDisc = player.GetNextDisc();
                    }
                }
                else
                {
                    // 石を置くところがないのでパス
                    passCount++;
                    restart.interactable = false;
                    sq = Sequence.Pass;
                }
            }
            else
            {
                // 敵ターン
                board.UpdateAssist(isAssist, enemy.DiscType);
                enemy.Bound.Play();

                if(board.CanPlaceDisc(enemy.DiscType))
                {
                    // 石が置ける
                    passCount = 0;
                    if(player.Discs.Count > enemy.Discs.Count)
                    {
                        // 相手の石を使う(パスで石の数がズレたりする)
                        selectedDisc = player.GetNextDisc();
                        selectedDisc.SetDiscType(enemy.DiscType);
                    }
                    else
                    {
                        // 自分の石を使う
                        selectedDisc = enemy.GetNextDisc();
                    }

                    enemy.TryGetReverseDiscs(out var selectedCell, out var selectedReverseDiscs);
                    ExecuteDiscPlacement(selectedCell, selectedReverseDiscs);
                }
                else
                {
                    // 石を置くところがないのでパス
                    passCount++;
                    restart.interactable = false;
                    sq = Sequence.Pass;
                }
            }
        }
    }
}