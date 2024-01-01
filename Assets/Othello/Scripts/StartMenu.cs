using UnityEngine;
using UnityEngine.UI;

namespace Othello
{
    /// <summary>
    /// ゲーム設定を変更するスタートメニュー
    /// </summary>
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] Sprite[] charaSprites;
        [SerializeField] Sprite[] difficultySprites;
        [SerializeField] Sprite[] playFirstSprites;
        [SerializeField] Sprite[] assistSprites;
        [SerializeField] Othello othello;
        [SerializeField] Board board;
        [SerializeField] Image chara;
        [SerializeField] Image difficultyImage;
        [SerializeField] Image playFirstImage;
        [SerializeField] Image assistImage;

        void Start()
        {
            board.Init();
            board.PlaceDiscDirect(3, 3, DiscType.White);
            board.PlaceDiscDirect(4, 3, DiscType.Black);
            board.PlaceDiscDirect(3, 4, DiscType.Black);
            board.PlaceDiscDirect(4, 4, DiscType.White);

            SetDifficulty(othello.Difficulty);
            SetPlayFirst(othello.PlayFirst);
            SetAssist(othello.IsAssist);
        }

        /// <summary>
        /// 難易度の右矢印がクリックされた
        /// </summary>
        public void OnDifficultyRightClick()
        {
            var value = (int)othello.Difficulty;
            value++;
            if(value > difficultySprites.Length - 1)
            {
                value = 0;
            }
            SetDifficulty((Difficulty)value);
        }

        /// <summary>
        /// 難易度の左矢印がクリックされた
        /// </summary>
        public void OnDifficultyLeftClick()
        {
            var value = (int)othello.Difficulty;
            value--;
            if(value < 0)
            {
                value = difficultySprites.Length - 1;
            }
            SetDifficulty((Difficulty)value);
        }

        /// <summary>
        /// 手番の右矢印がクリックされた
        /// </summary>
        public void OnPlayFirstRightClick()
        {
            var value = (int)othello.PlayFirst;
            value++;
            if(value > playFirstSprites.Length - 1)
            {
                value = 0;
            }
            SetPlayFirst((PlayFirst)value);
        }

        /// <summary>
        /// 手番の左矢印がクリックされた
        /// </summary>
        public void OnPlayFirstLeftClick()
        {
            var value = (int)othello.PlayFirst;
            value--;
            if(value < 0)
            {
                value = playFirstSprites.Length - 1;
            }
            SetPlayFirst((PlayFirst)value);
        }

        /// <summary>
        /// アシストの矢印がクリックされた
        /// </summary>
        public void OnAssistClick()
        {
            SetAssist(!othello.IsAssist);
        }

        /// <summary>
        /// 難易度を設定
        /// </summary>
        /// <param name="difficulty">難易度</param>
        void SetDifficulty(Difficulty difficulty)
        {
            othello.Difficulty     = difficulty;
            difficultyImage.sprite = difficultySprites[(int)difficulty];
            chara.sprite           = charaSprites[(int)difficulty];
        }

        /// <summary>
        /// 手番を設定
        /// </summary>
        /// <param name="playFirst">手番</param>
        void SetPlayFirst(PlayFirst playFirst)
        {
            othello.PlayFirst     = playFirst;
            playFirstImage.sprite = playFirstSprites[(int)playFirst];
        }

        /// <summary>
        /// アシストを設定
        /// </summary>
        /// <param name="isAssist">アシスト</param>
        void SetAssist(bool isAssist)
        {
            othello.IsAssist   = isAssist;
            assistImage.sprite = assistSprites[isAssist ? 1 : 0];
            board.UpdateAssist(isAssist, DiscType.Black);
        }
    }
}