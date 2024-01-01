using UnityEngine;

namespace Othello
{
    /// <summary>
    /// セル。石を置いたり、アシストやNG画像を表示したりする。
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [SerializeField] Disc assist;
        [SerializeField] Transform discSpace;
        [SerializeField] NgAnimation ng;
        Othello othello;
        int x;
        int y;
        Disc disc;

        public Transform DiscSpace => discSpace;
        public NgAnimation Ng      => ng;
        public int X               => x;
        public int Y               => y;
        public Disc Disc { get => disc; set => disc = value; }
        public bool IsEdge
        {
            get
            {
                if(x == 0)                return true;
                if(x == Board.Column - 1) return true;
                if(y == 0)                return true;
                if(y == Board.Row - 1)    return true;
                return false;
            }
        }
        public bool IsCorner
        {
            get
            {
                if(x == 0                && y == 0)             return true;
                if(x == Board.Column - 1 && y == 0)             return true;
                if(x == 0                && y == Board.Row - 1) return true;
                if(x == Board.Column - 1 && y == Board.Row - 1) return true;
                return false;
            }
        }

        /// <summary>
        /// セルを初期化
        /// </summary>
        /// <param name="othello">オセロ</param>
        /// <param name="x">セルX位置</param>
        /// <param name="y">セルY位置</param>
        public void Init(Othello othello, int x, int y)
        {
            this.othello = othello;
            this.x       = x;
            this.y       = y;

            name = $"{x}_{y}";
        }

        /// <summary>
        /// セルがクリックされた
        /// </summary>
        public void OnClick()
        {
            othello.OnCellClick(this);
        }

        /// <summary>
        /// アシストを更新
        /// </summary>
        /// <param name="active">アシストの表示状態。アシストONかつ石が置けるなら true</param>
        /// <param name="discType">石タイプ</param>
        public void UpdateAssist(bool active, DiscType discType)
        {
            if(assist.DiscType != discType) assist.SetDiscType(discType);
            assist.gameObject.SetActive(active);
        }
    }
}