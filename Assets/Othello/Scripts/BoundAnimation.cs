using UnityEngine;

namespace Othello
{
    /// <summary>
    /// オブジェクトをバウンドさせるアニメーション
    /// </summary>
    public class BoundAnimation : MonoBehaviour
    {
        enum Sequence { None, ScaleUp, ScaleDown }

        [SerializeField] float deltaScale = 0.1f;
        [SerializeField] float duration   = 0.1f;
        Sequence sq;
        float time;

        public bool IsPlaying => (sq != Sequence.None);

        void Update()
        {
            if(sq == Sequence.ScaleUp)
            {
                // オブジェクトを大きくする
                time += Time.deltaTime;
                if(time < duration)
                {
                    // 拡大中
                    var rate  = time / duration;
                    var scale = 1 + deltaScale * rate;
                    transform.localScale = new Vector3(scale, scale);
                }
                else
                {
                    // 拡大終了
                    var scale = 1 + deltaScale;
                    transform.localScale = new Vector3(scale, scale);

                    sq   = Sequence.ScaleDown;
                    time = 0;
                }
            }
            else if(sq == Sequence.ScaleDown)
            {
                // オブジェクトを小さくする
                time += Time.deltaTime;
                if(time < duration)
                {
                    // 縮小中
                    var rate  = time / duration;
                    var scale = (1 + deltaScale) - deltaScale * rate;
                    transform.localScale = new Vector3(scale, scale);
                }
                else
                {
                    // 縮小終了
                    transform.localScale = Vector3.one;

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
            sq = Sequence.ScaleUp;
        }

        /// <summary>
        /// アニメーションを停止
        /// </summary>
        public void Stop()
        {
            sq   = Sequence.None;
            time = 0;
            transform.localScale = Vector3.one;
        }
    }
}