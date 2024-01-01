using UnityEngine;

namespace Othello
{
    /// <summary>
    /// 石を反転させるアニメーション
    /// </summary>
    public class ReverseAnimation : MonoBehaviour
    {
        enum Sequence { None, Wait, Rotaion90, Rotaion180 }

        [SerializeField] Disc disc;
        [SerializeField] float delay    = 0.15f;
        [SerializeField] float duration = 0.2f;
        [SerializeField] Vector3 axis   = new Vector3(0, 1, 0);
        Sequence sq;
        float time;
        float waitTime;
        DiscType reversedDiscType;

        public bool IsPlaying => (sq != Sequence.None);

        void Update()
        {
            if(sq == Sequence.Wait)
            {
                // ウェイト
                time += Time.deltaTime;
                if(time >= waitTime)
                {
                    // ウェイト終了
                    sq   = Sequence.Rotaion90;
                    time = 0;
                }
            }
            else if(sq == Sequence.Rotaion90)
            {
                // 90°回転
                time += Time.deltaTime;
                if(time < duration)
                {
                    // 回転中
                    var rate = time / duration;
                    var angle = 90 * rate;
                    disc.transform.rotation = Quaternion.AngleAxis(angle, axis);
                }
                else
                {
                    // 回転終了
                    disc.transform.rotation = Quaternion.AngleAxis(90, axis);
                    disc.SetDiscType(reversedDiscType);

                    sq   = Sequence.Rotaion180;
                    time = 0;
                }
            }
            else if(sq == Sequence.Rotaion180)
            {
                // 180°回転
                time += Time.deltaTime;
                if(time < duration)
                {
                    // 回転中
                    var rate  = time / duration;
                    var angle = 90 + 90 * rate;
                    disc.transform.rotation = Quaternion.AngleAxis(angle, axis);
                }
                else
                {
                    // 回転終了
                    disc.transform.rotation = Quaternion.Euler(0, 0, 0);

                    sq   = Sequence.None;
                    time = 0;
                }
            }
        }

        /// <summary>
        /// アニメーションを再生
        /// </summary>
        /// <param name="discType">反転後の石タイプ</param>
        public void Play(DiscType discType)
        {
            sq       = Sequence.Wait;
            waitTime = delay * disc.ReverseIdx;
            reversedDiscType = discType;
        }

        /// <summary>
        /// アニメーションを停止
        /// </summary>
        public void Stop()
        {
            sq   = Sequence.None;
            time = 0;
            disc.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}