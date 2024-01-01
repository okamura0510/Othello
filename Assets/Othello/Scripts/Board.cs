using UnityEngine;
using System.Collections.Generic;

namespace Othello
{
    /// <summary>
    /// ボード。石やセルの操作を行う。
    /// </summary>
    public class Board : MonoBehaviour
    {
        public const int Column = 8;
        public const int Row    = 8;

        [SerializeField] Othello othello;
        [SerializeField] Transform offset;
        Cell[,] cells = new Cell[Column, Row];
        List<Disc> reverseDiscs = new List<Disc>();
        List<Disc> foundReverseDiscs = new List<Disc>();

        public Cell[,] Cells => cells;

        /// <summary>
        /// ボードを初期化
        /// </summary>
        public void Init()
        {
            for(var y = 0; y < Row; y++)
            {
                for(var x = 0; x < Column; x++)
                {
                    var cell = Instantiate(Resources.Load<Cell>("Prefabs/Cell"), offset);
                    cell.Init(othello, x, y);
                    cells[x, y] = cell;
                }
            }
        }

        /// <summary>
        /// セルに石を置く(直接)
        /// </summary>
        /// <param name="x">セルX位置</param>
        /// <param name="y">セルY位置</param>
        /// <param name="discType">石タイプ</param>
        public void PlaceDiscDirect(int x, int y, DiscType discType)
        {
            var cell = cells[x, y];
            var disc = Instantiate(Resources.Load<Disc>("Prefabs/Disc"), cell.DiscSpace);
            disc.Init(discType);
            cell.Disc = disc;
        }

        /// <summary>
        /// セルに石を置く
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="disc">石</param>
        public void PlaceDisc(Cell cell, Disc disc)
        {
            disc.transform.SetParent(cell.DiscSpace);
            disc.transform.localPosition = Vector3.zero;
            disc.transform.eulerAngles   = Vector3.zero;
            disc.gameObject.SetActive(true);

            cell.Disc = disc;
        }

        /// <summary>
        /// 石が置けるか
        /// </summary>
        /// <param name="discType">石タイプ</param>
        /// <returns>石が置けるなら true</returns>
        public bool CanPlaceDisc(DiscType discType)
        {
            foreach(var cell in cells)
            {
                if(CanPlaceDisc(cell, discType)) return true;
            }
            return false;
        }

        /// <summary>
        /// 石が置けるか
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="discType">石タイプ</param>
        /// <returns>石が置けるなら true</returns>
        public bool CanPlaceDisc(Cell cell, DiscType discType)
        {
            var reverseDiscs = GetReverseDiscs(cell, discType);
            return (reverseDiscs.Count > 0);
        }

        /// <summary>
        /// 反転する石を取得
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="discType">石タイプ</param>
        /// <returns>反転石リスト。反転する石が見つからなかった場合は空。</returns>
        public List<Disc> GetReverseDiscs(Cell cell, DiscType discType)
        {
            reverseDiscs.Clear();

            // すでに石が置かれていたら空を返す
            if(cell.Disc != null) return reverseDiscs;

            // 反転インデックスを初期化
            foreach(var c in cells)
            {
                if(c.Disc == null) continue;
                c.Disc.ReverseIdx = 0;
            }

            // 右の石を検索
            foundReverseDiscs.Clear();
            for(var x = cell.X + 1; x < Column; x++)
            {
                if(SearchReverseDiscs(cells[x, cell.Y], discType)) break;
            }

            // 左の石を検索
            foundReverseDiscs.Clear();
            for(var x = cell.X - 1; x >= 0; x--)
            {
                if(SearchReverseDiscs(cells[x, cell.Y], discType)) break;
            }

            // 下の石を検索
            foundReverseDiscs.Clear();
            for(var y = cell.Y + 1; y < Row; y++)
            {
                if(SearchReverseDiscs(cells[cell.X, y], discType)) break;
            }

            // 上の石を検索
            foundReverseDiscs.Clear();
            for(var y = cell.Y - 1; y >= 0; y--)
            {
                if(SearchReverseDiscs(cells[cell.X, y], discType)) break;
            }

            // 左上の石を検索
            foundReverseDiscs.Clear();
            {
                var x = cell.X - 1;
                var y = cell.Y - 1;
                while(x >= 0 && y >= 0)
                {
                    if(SearchReverseDiscs(cells[x, y], discType)) break;
                    x--;
                    y--;
                }
            }

            // 右上の石を検索
            foundReverseDiscs.Clear();
            {
                var x = cell.X + 1;
                var y = cell.Y - 1;
                while(x < Column && y >= 0)
                {
                    if(SearchReverseDiscs(cells[x, y], discType)) break;
                    x++;
                    y--;
                }
            }

            // 左下の石を検索
            foundReverseDiscs.Clear();
            {
                var x = cell.X - 1;
                var y = cell.Y + 1;
                while(x >= 0 && y < Row)
                {
                    if(SearchReverseDiscs(cells[x, y], discType)) break;
                    x--;
                    y++;
                }
            }

            // 右下の石を検索
            foundReverseDiscs.Clear();
            {
                var x = cell.X + 1;
                var y = cell.Y + 1;
                while(x < Column && y < Row)
                {
                    if(SearchReverseDiscs(cells[x, y], discType)) break;
                    x++;
                    y++;
                }
            }

            return reverseDiscs;
        }
        
        /// <summary>
        /// 反転する石を検索
        /// </summary>
        /// <param name="cell">セル</param>
        /// <param name="discType">石タイプ</param>
        /// <returns>検索終了なら true</returns>
        bool SearchReverseDiscs(Cell cell, DiscType discType)
        {
            if(cell.Disc == null)
            {
                // 石がないので終了
                return true;
            }
            else if(cell.Disc.DiscType != discType)
            {
                // 違う色の石なので反転石を保存(演出用に反転インデックスを設定しとく)
                cell.Disc.ReverseIdx = foundReverseDiscs.Count;
                foundReverseDiscs.Add(cell.Disc);
                return false;
            }
            else
            {
                // 同じ色の石なので終了
                if(foundReverseDiscs.Count > 0)
                {
                    // 見つかった反転石を保存
                    reverseDiscs.AddRange(foundReverseDiscs);
                }
                return true;
            }
        }

        /// <summary>
        /// アシストを更新
        /// </summary>
        /// <param name="isAssist">アシストフラグ</param>
        /// <param name="discType">石タイプ</param>
        public void UpdateAssist(bool isAssist, DiscType discType)
        {
            foreach(var cell in cells)
            {
                if(isAssist)
                {
                    var active = CanPlaceDisc(cell, discType);
                    cell.UpdateAssist(active, discType);
                }
                else
                {
                    cell.UpdateAssist(false, discType);
                }
            }
        }
    }
}