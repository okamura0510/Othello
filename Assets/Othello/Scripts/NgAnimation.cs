using UnityEngine;
using UnityEngine.UI;

namespace Othello
{
    /// <summary>
    /// 石が置けないことを示すアニメーション
    /// </summary>
    public class NgAnimation : MonoBehaviour
    {
        enum Sequence { None, Display, FadeOut }

        [SerializeField] Image image;
        [SerializeField] float displayDuration = 0.3f;
        [SerializeField] float fadeOutDuration = 0.15f;
        Sequence sq;
        float time;

        public bool IsPlaying => (sq != Sequence.None);

        void Update()
        {
            if(sq == Sequence.Display)
            {
                // NG画像を一定時間表示
                time += Time.deltaTime;
                if(time >= displayDuration)
                {
                    sq   = Sequence.FadeOut;
                    time = 0;
                }
            }
            else if(sq == Sequence.FadeOut)
            {
                // フェイドアウト
                time += Time.deltaTime;
                if(time < fadeOutDuration)
                {
                    // フェイドアウト中
                    var rate = time / fadeOutDuration;
                    var a    = 1 - 1 * rate;
                    SetAlpha(a);
                }
                else
                {
                    // フェイドアウト終了
                    SetAlpha(0);
                    gameObject.SetActive(false);

                    sq   = Sequence.None;
                    time = 0;
                }
            }
        }

        /// <summary>
        /// アニメーションを再生
        /// </summary>
        public void Play()
        {
            sq = Sequence.Display;
            SetAlpha(1);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// アニメーションを停止
        /// </summary>
        public void Stop()
        {
            sq   = Sequence.None;
            time = 0;
            SetAlpha(0);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// イメージのアルファ値を設定
        /// </summary>
        /// <param name="a">アルファ値</param>
        void SetAlpha(float a)
        {
            var color   = image.color;
            color.a     = a;
            image.color = color;
        }
    }
}