using UnityEngine;

namespace Whisperwood
{
    /// <summary>
    /// Shake-Weight of Infinite Gains - The legendary weapon that shakes enemies to death
    /// Special Ability: Infomercial Hypnosis - Enemies stop fighting to watch the commercial
    /// Ultimate: "As Seen on TV" - Summon a giant Shake-Weight that crushes the battlefield
    /// </summary>
    public class ShakeWeightWeapon : FitnessEquipmentWeapon
    {
        [Header("Shake-Weight Specific")]
        public float shakeIntensity = 1f;
        public float hypnosisDuration = 3f;
        public float hypnosisRadius = 5f;
        public GameObject giantShakeWeightPrefab;
        
        private bool isShaking = false;
        private float shakeDamageAccumulator = 0f;
        
        protected override void InitializeWeapon()
        {
            weaponName = "Shake-Weight of Infinite Gains";
            weaponType = FitnessEquipmentType.ShakeWeight;
            baseDamage = 25f;
            attackSpeed = 2f;
            range = 3f;
            staminaCost = 5f;
            adaptiveTriggerResistance = 0.3f;
            hapticIntensity = 0.8f;
            specialAbilityCooldown = 15f;
            ultimateAbilityCooldown = 45f;
        }
        
        public override void PerformBasicAttack()
        {
            if (!CanAttack()) return;
            
            // Shake-Weight has continuous damage - start shaking
            StartShaking();
            base.PerformBasicAttack();
        }
        
        /// <summary>
        /// Start the continuous shaking motion
        /// </summary>
        private void StartShaking()
        {
            isShaking = true;
            shakeDamageAccumulator = 0f;
            
            // Trigger continuous haptic feedback
            StartCoroutine(ContinuousShaking());
            
            Debug.Log("Shake-Weight Attack: Shake and Bake!");
        }
        
        /// <summary>
        /// Continuous shaking coroutine
        /// </summary>
        private System.Collections.IEnumerator ContinuousShaking()
        {
            while (isShaking && Time.time - lastAttackTime < 3f) // Shake for 3 seconds
            {
                // Apply continuous damage to enemies
                ApplyContinuousShakeDamage();
                
                // Trigger haptic feedback
                TriggerHapticFeedback(hapticIntensity * shakeIntensity, 0.1f);
                
                // Apply adaptive trigger resistance
                if (enableAdaptiveTriggers)
                {
                    ApplyAdaptiveTriggerResistance(adaptiveTriggerResistance * shakeIntensity);
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            
            isShaking = false;
        }
        
        /// <summary>
        /// Apply continuous damage from shaking
        /// </summary>
        private void ApplyContinuousShakeDamage()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, range);
            
            foreach (Collider enemy in enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    float continuousDamage = baseDamage * 0.1f * shakeIntensity;
                    enemyAI.TakeDamage(continuousDamage, weaponType);
                    
                    // Apply "shaken" debuff
                    enemyAI.ApplyDebuff("Shaken", 0.5f, 1f);
                }
            }
        }
        
        protected override void ExecuteSpecialAbility()
        {
            // Infomercial Hypnosis - Stun all enemies in range
            Collider[] enemies = Physics.OverlapSphere(transform.position, hypnosisRadius);
            
            foreach (Collider enemy in enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // Apply hypnosis effect
                    enemyAI.ApplyDebuff("Hypnotized", hypnosisDuration, 0f);
                    
                    // Make enemy stop fighting and watch "commercial"
                    enemyAI.StopCombat(hypnosisDuration);
                }
            }
            
            // Play infomercial sound effect
            PlayInfomercialSound();
            
            Debug.Log("Shake-Weight Special: Infomercial Hypnosis - 'Just 6 minutes a day!'");
        }
        
        protected override void ExecuteUltimateAbility()
        {
            // "As Seen on TV" - Summon giant Shake-Weight
            Vector3 spawnPosition = transform.position + transform.forward * 5f;
            
            if (giantShakeWeightPrefab != null)
            {
                GameObject giantShakeWeight = Instantiate(giantShakeWeightPrefab, spawnPosition, transform.rotation);
                
                // Configure the giant shake weight
                GiantShakeWeightController controller = giantShakeWeight.GetComponent<GiantShakeWeightController>();
                if (controller != null)
                {
                    controller.Initialize(baseDamage * 3f, 10f, 8f);
                }
                
                // Destroy after 10 seconds
                Destroy(giantShakeWeight, 10f);
            }
            
            Debug.Log("Shake-Weight Ultimate: As Seen on TV - GIANT SHAKE-WEIGHT INCOMING!");
        }
        
        /// <summary>
        /// Play infomercial sound effect
        /// </summary>
        private void PlayInfomercialSound()
        {
            // This would play the classic "Just 6 minutes a day!" sound
            Debug.Log("Playing infomercial sound: 'Just 6 minutes a day!'");
        }
        
        /// <summary>
        /// Stop shaking (called when attack ends)
        /// </summary>
        protected override void EndAttack()
        {
            base.EndAttack();
            isShaking = false;
            StopAllCoroutines();
        }
        
        /// <summary>
        /// Get Shake-Weight specific weapon info
        /// </summary>
        public override WeaponInfo GetWeaponInfo()
        {
            WeaponInfo info = base.GetWeaponInfo();
            info.name = "Shake-Weight of Infinite Gains";
            info.damage = baseDamage;
            info.attackSpeed = attackSpeed;
            return info;
        }
    }
}
