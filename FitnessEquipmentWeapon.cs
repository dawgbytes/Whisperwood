using UnityEngine;

namespace Whisperwood
{
    /// <summary>
    /// Base class for all fitness equipment weapons in Gucci Goblins
    /// Handles weapon mechanics, PS5 DualSense integration, and special abilities
    /// </summary>
    public abstract class FitnessEquipmentWeapon : MonoBehaviour
    {
        [Header("Weapon Stats")]
        public string weaponName;
        public float baseDamage = 50f;
        public float attackSpeed = 1f;
        public float range = 2f;
        public float staminaCost = 10f;
        public FitnessEquipmentType weaponType;
        
        [Header("PS5 DualSense Integration")]
        public float adaptiveTriggerResistance = 0.5f;
        public float hapticIntensity = 1f;
        public bool enableAdaptiveTriggers = true;
        public bool enableHapticFeedback = true;
        
        [Header("Special Abilities")]
        public float specialAbilityCooldown = 10f;
        public float ultimateAbilityCooldown = 30f;
        public bool canUseSpecialAbility = true;
        public bool canUseUltimate = true;
        
        [Header("Visual Effects")]
        public ParticleSystem attackEffect;
        public ParticleSystem specialEffect;
        public ParticleSystem ultimateEffect;
        public AudioClip attackSound;
        public AudioClip specialSound;
        public AudioClip ultimateSound;
        
        // Internal state
        protected float lastAttackTime;
        protected float lastSpecialTime;
        protected float lastUltimateTime;
        protected bool isAttacking = false;
        protected bool isUsingSpecial = false;
        protected bool isUsingUltimate = false;
        
        // Components
        protected AudioSource audioSource;
        protected Animator animator;
        protected PlayerController playerController;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            playerController = GetComponentInParent<PlayerController>();
            
            InitializeWeapon();
        }
        
        /// <summary>
        /// Initialize weapon-specific properties
        /// </summary>
        protected abstract void InitializeWeapon();
        
        /// <summary>
        /// Perform basic attack
        /// </summary>
        public virtual void PerformBasicAttack()
        {
            if (!CanAttack()) return;
            
            lastAttackTime = Time.time;
            isAttacking = true;
            
            // Trigger PS5 DualSense feedback
            TriggerAttackFeedback();
            
            // Play attack animation and effects
            PlayAttackAnimation();
            PlayAttackEffects();
            PlayAttackSound();
            
            // Calculate and apply damage
            float damage = CalculateDamage();
            ApplyDamage(damage);
            
            // Start attack cooldown
            Invoke(nameof(EndAttack), 1f / attackSpeed);
        }
        
        /// <summary>
        /// Perform special ability
        /// </summary>
        public virtual void PerformSpecialAbility()
        {
            if (!CanUseSpecialAbility()) return;
            
            lastSpecialTime = Time.time;
            isUsingSpecial = true;
            canUseSpecialAbility = false;
            
            // Trigger enhanced PS5 feedback
            TriggerSpecialFeedback();
            
            // Play special effects and sounds
            PlaySpecialEffects();
            PlaySpecialSound();
            
            // Execute special ability logic
            ExecuteSpecialAbility();
            
            // Reset cooldown
            Invoke(nameof(ResetSpecialCooldown), specialAbilityCooldown);
            Invoke(nameof(EndSpecial), 2f);
        }
        
        /// <summary>
        /// Perform ultimate ability
        /// </summary>
        public virtual void PerformUltimateAbility()
        {
            if (!CanUseUltimate()) return;
            
            lastUltimateTime = Time.time;
            isUsingUltimate = true;
            canUseUltimate = false;
            
            // Trigger maximum PS5 feedback
            TriggerUltimateFeedback();
            
            // Play ultimate effects and sounds
            PlayUltimateEffects();
            PlayUltimateSound();
            
            // Execute ultimate ability logic
            ExecuteUltimateAbility();
            
            // Reset cooldown
            Invoke(nameof(ResetUltimateCooldown), ultimateAbilityCooldown);
            Invoke(nameof(EndUltimate), 5f);
        }
        
        /// <summary>
        /// Check if weapon can attack
        /// </summary>
        protected virtual bool CanAttack()
        {
            return !isAttacking && Time.time - lastAttackTime >= 1f / attackSpeed;
        }
        
        /// <summary>
        /// Check if special ability can be used
        /// </summary>
        protected virtual bool CanUseSpecialAbility()
        {
            return canUseSpecialAbility && !isUsingSpecial && !isUsingUltimate;
        }
        
        /// <summary>
        /// Check if ultimate can be used
        /// </summary>
        protected virtual bool CanUseUltimate()
        {
            return canUseUltimate && !isUsingSpecial && !isUsingUltimate;
        }
        
        /// <summary>
        /// Calculate damage based on weapon stats and player attributes
        /// </summary>
        protected virtual float CalculateDamage()
        {
            float damage = baseDamage;
            
            // Apply player strength modifier
            if (playerController != null)
            {
                // This would integrate with player stats system
                // damage *= playerController.GetStrengthMultiplier();
            }
            
            return damage;
        }
        
        /// <summary>
        /// Apply damage to enemies in range
        /// </summary>
        protected virtual void ApplyDamage(float damage)
        {
            // Find enemies in attack range
            Collider[] enemies = Physics.OverlapSphere(transform.position, range);
            
            foreach (Collider enemy in enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(damage, weaponType);
                }
            }
        }
        
        /// <summary>
        /// Trigger PS5 DualSense attack feedback
        /// </summary>
        protected virtual void TriggerAttackFeedback()
        {
            if (!enableHapticFeedback) return;
            
            // Trigger haptic feedback
            TriggerHapticFeedback(hapticIntensity, 0.2f);
            
            // Apply adaptive trigger resistance
            if (enableAdaptiveTriggers)
            {
                ApplyAdaptiveTriggerResistance(adaptiveTriggerResistance);
            }
        }
        
        /// <summary>
        /// Trigger PS5 DualSense special ability feedback
        /// </summary>
        protected virtual void TriggerSpecialFeedback()
        {
            if (!enableHapticFeedback) return;
            
            // Enhanced haptic feedback for special abilities
            TriggerHapticFeedback(hapticIntensity * 1.5f, 0.5f);
            
            // Increased adaptive trigger resistance
            if (enableAdaptiveTriggers)
            {
                ApplyAdaptiveTriggerResistance(adaptiveTriggerResistance * 1.5f);
            }
        }
        
        /// <summary>
        /// Trigger PS5 DualSense ultimate ability feedback
        /// </summary>
        protected virtual void TriggerUltimateFeedback()
        {
            if (!enableHapticFeedback) return;
            
            // Maximum haptic feedback for ultimate abilities
            TriggerHapticFeedback(hapticIntensity * 2f, 1f);
            
            // Maximum adaptive trigger resistance
            if (enableAdaptiveTriggers)
            {
                ApplyAdaptiveTriggerResistance(adaptiveTriggerResistance * 2f);
            }
        }
        
        /// <summary>
        /// Trigger haptic feedback on DualSense controller
        /// </summary>
        protected virtual void TriggerHapticFeedback(float intensity, float duration)
        {
            // This would integrate with PlayStation SDK
            Debug.Log($"Haptic Feedback: {weaponName} - Intensity: {intensity}, Duration: {duration}");
        }
        
        /// <summary>
        /// Apply adaptive trigger resistance
        /// </summary>
        protected virtual void ApplyAdaptiveTriggerResistance(float resistance)
        {
            // This would integrate with PlayStation SDK
            Debug.Log($"Adaptive Trigger Resistance: {weaponName} - Resistance: {resistance}");
        }
        
        /// <summary>
        /// Play attack animation
        /// </summary>
        protected virtual void PlayAttackAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
        
        /// <summary>
        /// Play attack effects
        /// </summary>
        protected virtual void PlayAttackEffects()
        {
            if (attackEffect != null)
            {
                attackEffect.Play();
            }
        }
        
        /// <summary>
        /// Play attack sound
        /// </summary>
        protected virtual void PlayAttackSound()
        {
            if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }
        
        /// <summary>
        /// Play special ability effects
        /// </summary>
        protected virtual void PlaySpecialEffects()
        {
            if (specialEffect != null)
            {
                specialEffect.Play();
            }
        }
        
        /// <summary>
        /// Play special ability sound
        /// </summary>
        protected virtual void PlaySpecialSound()
        {
            if (audioSource != null && specialSound != null)
            {
                audioSource.PlayOneShot(specialSound);
            }
        }
        
        /// <summary>
        /// Play ultimate ability effects
        /// </summary>
        protected virtual void PlayUltimateEffects()
        {
            if (ultimateEffect != null)
            {
                ultimateEffect.Play();
            }
        }
        
        /// <summary>
        /// Play ultimate ability sound
        /// </summary>
        protected virtual void PlayUltimateSound()
        {
            if (audioSource != null && ultimateSound != null)
            {
                audioSource.PlayOneShot(ultimateSound);
            }
        }
        
        /// <summary>
        /// Execute weapon-specific special ability
        /// </summary>
        protected abstract void ExecuteSpecialAbility();
        
        /// <summary>
        /// Execute weapon-specific ultimate ability
        /// </summary>
        protected abstract void ExecuteUltimateAbility();
        
        /// <summary>
        /// End basic attack
        /// </summary>
        protected virtual void EndAttack()
        {
            isAttacking = false;
        }
        
        /// <summary>
        /// End special ability
        /// </summary>
        protected virtual void EndSpecial()
        {
            isUsingSpecial = false;
        }
        
        /// <summary>
        /// End ultimate ability
        /// </summary>
        protected virtual void EndUltimate()
        {
            isUsingUltimate = false;
        }
        
        /// <summary>
        /// Reset special ability cooldown
        /// </summary>
        protected virtual void ResetSpecialCooldown()
        {
            canUseSpecialAbility = true;
        }
        
        /// <summary>
        /// Reset ultimate ability cooldown
        /// </summary>
        protected virtual void ResetUltimateCooldown()
        {
            canUseUltimate = true;
        }
        
        /// <summary>
        /// Get weapon information for UI display
        /// </summary>
        public virtual WeaponInfo GetWeaponInfo()
        {
            return new WeaponInfo
            {
                name = weaponName,
                type = weaponType,
                damage = baseDamage,
                attackSpeed = attackSpeed,
                range = range,
                staminaCost = staminaCost,
                specialCooldown = specialAbilityCooldown,
                ultimateCooldown = ultimateAbilityCooldown,
                canUseSpecial = canUseSpecialAbility,
                canUseUltimate = canUseUltimate
            };
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
    
    /// <summary>
    /// Weapon information structure for UI display
    /// </summary>
    [System.Serializable]
    public struct WeaponInfo
    {
        public string name;
        public FitnessEquipmentType type;
        public float damage;
        public float attackSpeed;
        public float range;
        public float staminaCost;
        public float specialCooldown;
        public float ultimateCooldown;
        public bool canUseSpecial;
        public bool canUseUltimate;
    }
}
