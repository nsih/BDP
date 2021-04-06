using BS.UI;
using UnityEngine;

namespace BS.Enemy.Boss
{
    [RequireComponent(typeof(HealthBar))]
    public class BaseBoss : MonoBehaviour
    {
        HealthBar _healthBar;

        protected float _maxHealth;
        protected float _health;

        public float Health{
            get{
                return _health;
            }
        }

        public float MaxHealth{
            get{
                return _maxHealth;
            }
        }

        private void Awake(){
            _healthBar = GetComponent<HealthBar>();
        }

        public void SetMaxHealth(float maxHealth){
            _maxHealth = maxHealth;
            _health = maxHealth;
            _healthBar.SetMaxHealth(_maxHealth);
        }

        public void SetHealth(float health){
            _health = health;
            _healthBar.SetHealth(health);
        }
    }    
}