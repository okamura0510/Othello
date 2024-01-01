using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Othello
{
    /// <summary>
    /// 敵AI。難易度ごとにAIロジックを変える。
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] Board board;
        [SerializeField] Enemy enemy;
        List<Cell> foundCells = new List<Cell>();

        /// <summary>
        /// ノーマルAIを計算
        /// </summary>
        /// <param name="selectedCell">決定したセル</param>
        /// <param name="selectedReverseDiscs">決定した反転石リスト</param>
        /// <returns>反転石が見つかったら true</returns>
        public bool CalculateNormalAI(out Cell selectedCell, out List<Disc> selectedReverseDiscs)
        {
            // 石が置けるセルを全て保存
            foundCells.Clear();
            foreach(var cell in board.Cells)
            {
                if(board.CanPlaceDisc(cell, enemy.DiscType))
                {
                    foundCells.Add(cell);
                }
            }

            // 見つけたセルリストからランダムにセルを決定
            selectedCell         = foundCells[Random.Range(0, foundCells.Count)];
            selectedReverseDiscs = board.GetReverseDiscs(selectedCell, enemy.DiscType);
            return (selectedReverseDiscs.Count > 0);
        }

        /// <summary>
        /// ハードAIを計算
        /// </summary>
        /// <param name="selectedCell">決定したセル</param>
        /// <param name="selectedReverseDiscs">決定した反転石リスト</param>
        /// <returns>反転石が見つかったら true</returns>
        public bool CalculateHardAI(out Cell selectedCell, out List<Disc> selectedReverseDiscs)
        {
            foundCells.Clear();
            var maxReverseCount = 0;
            var hasEdge         = false;
            var hasCorner       = false;
            foreach(var cell in board.Cells)
            {
                var reverseDiscs = board.GetReverseDiscs(cell, enemy.DiscType);
                var reverseCount = reverseDiscs.Count;
                if(reverseCount == 0) continue;

                if(cell.IsCorner || hasCorner)
                {
                    // 4隅で一番多く反転出来るセル
                    if(cell.IsCorner)
                    {
                        if(!hasCorner || reverseCount > maxReverseCount)
                        {
                            hasCorner       = true;
                            maxReverseCount = reverseCount;
                            foundCells.Clear();
                            foundCells.Add(cell);
                        }
                        else if(reverseCount == maxReverseCount)
                        {
                            foundCells.Add(cell);
                        }
                    }
                }
                else if(cell.IsEdge || hasEdge)
                {
                    // 端で一番多く反転出来るセル
                    if(cell.IsEdge)
                    {
                        if(!hasEdge || reverseCount > maxReverseCount)
                        {
                            hasEdge         = true;
                            maxReverseCount = reverseCount;
                            foundCells.Clear();
                            foundCells.Add(cell);
                        }
                        else if(reverseCount == maxReverseCount)
                        {
                            foundCells.Add(cell);
                        }
                    }
                }
                else
                {
                    // 一番多く反転出来るセル
                    if(reverseCount > maxReverseCount)
                    {
                        maxReverseCount = reverseCount;
                        foundCells.Clear();
                        foundCells.Add(cell);
                    }
                    else if(reverseCount == maxReverseCount)
                    {
                        foundCells.Add(cell);
                    }
                }
            }

            // 見つけたセルリストからランダムにセルを決定
            selectedCell         = foundCells[Random.Range(0, foundCells.Count)];
            selectedReverseDiscs = board.GetReverseDiscs(selectedCell, enemy.DiscType);
            return (selectedReverseDiscs.Count > 0);
        }
    }
}