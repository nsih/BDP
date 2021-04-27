using System.Collections;
using BS.Manager.Cameras;
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
        int prevLeftRightDirection = 1;

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

        /// <summary>
        /// 4 frame 애니메이션으로
        /// 0.75 부터 마지막 프레임이기 때문에
        /// player의 power가 애니메이션의 진행도가 됨
        /// </summary>
        protected void ChargingAnim(){

            if(_player._isCharging){
                float progress = (Mathf.Abs(_player._currentPower) / _player._maxPower) * 0.75f;

                if(progress > 0.75f){
                    progress = 1f;
                }
        
                _faceAnimator.SetFloat("Charging", progress);
            }else{
                _faceAnimator.SetFloat("Charging", 0);
            }
        }

        /// <summary>
        /// Player 공격 성공시 애니메이션 재생
        /// </summary>
        public void AttackSuccess(){
            _faceAnimator.SetTrigger("AttackSuccess");
        }

        /// <summary>
        /// Player가 공격을 받아 무적 상태에 진입
        /// </summary>
        public void OnHit(){
            if(_player._isInvincible && !isBlink){
                SpriteAlphaBlink(0.9f, 0.03f);

                _bodyAnimator.SetTrigger("OnHit");
                if(!_player._isCharging){
                    _faceAnimator.SetTrigger("OnHit");
                }
            }
        }

        /// <summary>
        /// Player 사망시 애니메이션 재생
        /// </summary>
        public void Dead(){
            if(_player._isDead){
                SetSpriteAlpha(0.35f);
            }
        }
        
        /// <summary>
        /// 이동 방향에 따라 Player sprite를 좌우로 뒤집습니다.
        /// Player의 z rotation이 180이라면 상하로 뒤집습니다.
        /// </summary>
        protected void FlipSprite(){
            float rotationZ = Mathf.Abs(_player.transform.rotation.eulerAngles.z);


            int directionUpDown = 1;
            // Player가 Charging 중...
            if(_player._isCharging){
                if((90 < rotationZ) && (rotationZ < 270)){
                    directionUpDown = -1;
                }
            }
            else if((170 < rotationZ) && (rotationZ < 190)){
                directionUpDown = -1;
            }


            int directionLeftRight = prevLeftRightDirection;
            // Player가 Charging 중...
            if(_player._isCharging){
                Vector2 pos = this.transform.position;
				Vector2 mouseOnWorld = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

                directionLeftRight = (mouseOnWorld - pos).x <= 0 ? -1 : 1;
                prevLeftRightDirection = directionLeftRight;
            }
            else if(Mathf.Abs(_player._moveDirection) > 0){
                directionLeftRight = (int)(_player._moveDirection / Mathf.Abs(_player._moveDirection));
                prevLeftRightDirection = directionLeftRight;
            }

            Vector3 scale = new Vector3(1 * directionLeftRight * directionUpDown, 1 * directionUpDown, 1);

            _faceSprite.transform.localScale = scale;
            _bodySprite.transform.localScale = scale;
        }

        public void Render(){            
            ChargingAnim();
            
            // Player가 Charging 중이 아니면 Falling Animation
            _faceAnimator.SetBool("Falling", _player.IsFalling() && !_player._isCharging && !_player._attackSuccess);
            
            FlipSprite();
        }

        /// <summary>
        /// 디버깅 용
        /// 아직 제대로 만들지 않음
        /// </summary>
        private void OnDrawGizmos()
        {
            if(_bodySprite != null){
                float distance = 1f;
                // Left
                Gizmos.color = Color.yellow;
                Vector3 dst = (_bodySprite.transform.position - new Vector3(distance * _bodySprite.transform.localScale.x, 0, 0));
                Gizmos.DrawLine(_bodySprite.transform.position, dst);
                // Up
                Gizmos.color = Color.blue;
                dst = (_bodySprite.transform.position - new Vector3(0, distance * _bodySprite.transform.localEulerAngles.y, 0));
                Gizmos.DrawLine(_bodySprite.transform.position, dst);
            }
        }
    }
}
