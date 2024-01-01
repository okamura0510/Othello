using UnityEngine;
using TMPro;

namespace Othello
{
    /// <summary>
    /// ゲーム情報を表示するアニメーション
    /// </summary>
    public class InfoAnimation : MonoBehaviour
    {
        const string Pass    = "パス";
        const string GameEnd = "ゲーム終了";

        enum Sequence { None, Delay, FallFadeIn, Display, FallFadeOut }

        [SerializeField] GameObject back;
        [SerializeField] TextMeshProUGUI message;
        [SerializeField] float delay           = 0.5f;
        [SerializeField] int fallHeight        = 70;
        [SerializeField] float fallDuration    = 0.1f;
        [SerializeField] float displayDuration = 1;
        Sequence sq;
        float time;
        float startY;

        public bool IsPlaying => (sq != Sequence.None);

        void Start()
        {
            startY = message.transform.localPosition.y;
        }

        void Update()
        {
            if(sq == Sequence.Delay)
            {
                // 少し待つ
                time += Time.deltaTime;
                if(time >= delay)
                {
                    SetAlpha(0);
                    back.SetActive(true);
                    message.gameObject.SetActive(true);

                    sq   = Sequence.FallFadeIn;
                    time = 0;
                }
            }
            else if(sq == Sequence.FallFadeIn)
            {
                // 落下フェイドイン
                time += Time.deltaTime;
                if(time < fallDuration)
                {
                    // 落下中
                    var rate = time / fallDuration;
                    var a    = 1 * rate;
                    var y    = startY + fallHeight - fallHeight * rate;
                    SetAlpha(a);
                    SetY(y);
                }
                else
                {
                    // 落下終了
                    SetAlpha(1);
                    SetY(startY);

                    sq   = Sequence.Display;
                    time = 0;
                }
            }
            else if(sq == Sequence.Display)
            {
                // メッセージ表示
                time += Time.deltaTime;
                if(time >= displayDuration)
                {
                    sq   = Sequence.FallFadeOut;
                    time = 0;
                }
            }
            else if(sq == Sequence.FallFadeOut)
            {
                // 落下フェイドアウト
                time += Time.deltaTime;
                if(time < fallDuration)
                {
                    // 落下中
                    var rate = time / fallDuration;
                    var a    = 1 - 1 * rate;
                    var y    = startY - fallHeight * rate;
                    SetAlpha(a);
                    SetY(y);
                }
                else
                {
                    // 落下終了
                    SetAlpha(0);
                    SetY(fallHeight);
                    back.SetActive(false);
                    message.gameObject.SetActive(false);

                    sq   = Sequence.None;
                    time = 0;
                }
            }
        }

        /// <summary>
        /// パスを再生
        /// </summary>
        public void PlayPass()
        {
            Play(Pass);
        }

        /// <summary>
        /// ゲーム終了を再生
        /// </summary>
        public void PlayGameEnd()
        {
            Play(GameEnd);
        }

        /// <summary>
        /// アニメーションを再生
        /// </summary>
        /// <param name="mes">メッセージ</param>
        public void Play(string mes)
        {
            sq = Sequence.Delay;
            message.text = mes;
        }

        /// <summary>
        /// アニメーションを停止
        /// </summary>
        public void Stop()
        {
            sq   = Sequence.None;
            time = 0;
            SetAlpha(0);
            SetY(fallHeight);
            back.SetActive(false);
            message.gameObject.SetActive(false);
        }

        /// <summary>
        /// メッセージのアルファ値を設定
        /// </summary>
        /// <param name="a">アルファ値</param>
        void SetAlpha(float a)
        {
            var color     = message.color;
            color.a       = a;
            message.color = color;
        }

        /// <summary>
        /// メッセージのY値を設定
        /// </summary>
        /// <param name="y">Y値</param>
        void SetY(float y)
        {
            var pos = message.transform.localPosition;
            pos.y   = y;
            message.transform.localPosition = pos;
        }
    }
}