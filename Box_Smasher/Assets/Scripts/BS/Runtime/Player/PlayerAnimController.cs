using System.Collections;
using UnityEngine;

namespace BS.Player
{
    public class PlayerAnimController : MonoBehaviour
    {
        // 임시로 public으로 지정
        // Init 함수로 초기화 하는 게 좋을 지도 모름

        /// Animators
        protected PlayerController _player;
        protected Animator _bodyAnimator;
        protected Animator _faceAnimator;

        /// Sprite renders
        protected SpriteRenderer _bodySprite;
        protected SpriteRenderer _faceSprite;

        bool isBlink = false;

        enum BodyState{
            Idle,
            Error
        }

        enum FaceState{
            Idle
        }

        /// <summary>
        /// 플레이어 Animation Controller 초기화
        /// </summary>
        /// <param name="player"> Player Controller 객체</param>
        /// <param name="bodyAnimator"> body animator</param>
        /// <param name="faceAnimator"> face animator</param>
        public void Init(PlayerController player, Animator bodyAnimator, Animator faceAnimator){
            _player = player;
            _bodyAnimator = bodyAnimator;
            _bodySprite = _bodyAnimator.GetComponent<SpriteRenderer>();
            _faceAnimator = faceAnimator;
            _faceSprite = _faceAnimator.GetComponent<SpriteRenderer>();
        }


        /// <summary>
        /// 플레이어의 sprite alpha를 깜빡거리게 표현합니다.
        /// 삼각함수의 형태로 alpha가 변합니다.
        /// (freq에 0을 할당하지 마세요)
        /// </summary>
        /// <param name="t"> blink 지속 시간</param>
        /// <param name="freq"> alpha가 1이 되는 blink 주기</param>
        public void SpriteAlphaBlink(float t, float freq){
            StartCoroutine(SpriteAlphaBlinkLoop(t, freq));
        }

        protected IEnumerator SpriteAlphaBlinkLoop(float t, float freq){
            float duration = t;
            isBlink = true;
            while(duration > 0){
                float x = (t - duration);
                float alpha = Mathf.Abs(Mathf.Cos((Mathf.PI / freq) * x));
                SetSpriteAlpha(alpha);

                duration -= Time.deltaTime;
                yield return null;
            }
        
            isBlink = false;
            SetSpriteAlpha(1f);
            yield return null;
        }

        
        /// <summary>
        /// 플레이어의 sprite alpha를 set합니다.
        /// </summary>
        /// <param name="alpha"> set할 alpha값 </param>
        protected void SetSpriteAlpha(float alpha){
            _bodySprite.color = new Color(  _bodySprite.color.r,
                                            _bodySprite.color.g,
                                            _bodySprite.color.b,
                                            alpha);
            _faceSprite.color = new Color(  _faceSprite.color.r,
                                            _faceSprite.color.g,
                                            _faceSprite.color.b,
                                            alpha);
        }

        public void Render(){
            if(_player.Invincible && !isBlink){
                SpriteAlphaBlink(0.9f, 0.03f);
                _bodyAnimator.SetTrigger("OnHit");
                _faceAnimator.SetTrigger("OnHit");
            }

            if(_player.DEAD == 1){
                SetSpriteAlpha(0.35f);
            }

        }
    }
}
