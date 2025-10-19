using UnityEngine;

namespace Whisperwood
{
    /// <summary>
    /// Thigh Master of Crushing Victory - Feel the burn!
    /// Special Ability: Thigh Master Combo - Chain multiple enemies together
    /// Ultimate: "Thighs of Steel" - Become immune to damage while crushing everything in your path
    /// </summary>
    public class ThighMasterWeapon : FitnessEquipmentWeapon
    {
        [Header("Thigh Master Specific")]
        public float squeezeIntensity = 1f;
        public float burnDamagePerSecond = 10f;
        public float burnDuration = 5f;
        public float comboRadius = 3f;
        public float comboDamageMultiplier = 1.5f;
        
        private bool isSqueezing = false;
        private float squeezeTime = 0f;
        private System.Collections.Generic.List<EnemyAI> chainedEnemies;
        
        protected override void InitializeWeapon()
        {
            weaponName = "Thigh Master of Crushing Victory";
            weaponType = FitnessEquipmentType.ThighMaster;
            baseDamage = 35f;
            attackSpeed = 1f;
            range = 2.5f;
            staminaCost = 20f;
            adaptiveTriggerResistance = 0.8f;
            hapticIntensity = 1.2f;
            specialAbilityCooldown = 18f;
            ultimateAbilityCooldown = 50f;
            
            chainedEnemies = new System.Collections.Generic.List<EnemyAI>();
        }
        
        public override void PerformBasicAttack()
        {
            if (!CanAttack()) return;
            
            // Start squeezing for continuous damage
            StartSqueezing();
            base.PerformBasicAttack();
        }
        
        /// <summary>
        /// Start the squeezing motion
        /// </summary>
        private void StartSqueezing()
        {
            isSqueezing = true;
            squeezeTime = 0f;
            
            // Trigger continuous haptic feedback
            StartCoroutine(ContinuousSqueezing());
            
            Debug.Log("Thigh Master Attack: Feel the Burn!");
        }
        
        /// <summary>
        /// Continuous squeezing coroutine
        /// </summary>
        private System.Collections.IEnumerator ContinuousSqueezing()
        {
            while (isSqueezing && squeezeTime < 3f) // Squeeze for 3 seconds
            {
                // Apply continuous damage to enemies
                ApplyContinuousSqueezeDamage();
                
                // Trigger haptic feedback
                TriggerHapticFeedback(hapticIntensity * squeezeIntensity, 0.15f);
                
                // Apply adaptive trigger resistance
                if (enableAdaptiveTriggers)
                {
                    ApplyAdaptiveTriggerResistance(adaptiveTriggerResistance * squeezeIntensity);
                }
                
                squeezeTime += Time.deltaTime;
                yield return new WaitForSeconds(0.1f);
            }
            
            isSqueezing = false;
        }
        
        /// <summary>
        /// Apply continuous damage from squeezing
        /// </summary>
        private void ApplyContinuousSqueezeDamage()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, range);
            
            foreach (Collider enemy in enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    float squeezeDamage = baseDamage * 0.15f * squeezeIntensity;
                    enemyAI.TakeDamage(squeezeDamage, weaponType);
                    
                    // Apply "burn" debuff
                    enemyAI.ApplyDebuff("Burned", burnDuration, burnDamagePerSecond);
                    
                    // Apply "crushed" debuff
                    enemyAI.ApplyDebuff("Crushed", 2f, 0.5f);
                }
            }
        }
        
        protected override void ExecuteSpecialAbility()
        {
            // Thigh Master Combo - Chain multiple enemies together
            StartCoroutine(ThighMasterCombo());
            
            Debug.Log("Thigh Master Special: Thigh Master Combo - 'Feel the burn together!'");
        }
        
        /// <summary>
        /// Thigh Master Combo - Chain enemies together
        /// </summary>
        private System.Collections.IEnumerator ThighMasterCombo()
        {
            // Find enemies in combo radius
            Collider[] enemies = Physics.OverlapSphere(transform.position, comboRadius);
            chainedEnemies.Clear();
            
            foreach (Collider enemy in enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    chainedEnemies.Add(enemyAI);
                    
                    // Apply chain effect
                    ApplyChainEffect(enemyAI);
                }
            }
            
            // Chain damage - each enemy takes damage based on total chained enemies
            float comboDamage = baseDamage * comboDamageMultiplier * chainedEnemies.Count;
            
            foreach (EnemyAI enemy in chainedEnemies)
            {
                enemy.TakeDamage(comboDamage, weaponType);
                yield return new WaitForSeconds(0.1f);
            }
            
            // Clear chain after 5 seconds
            yield return new WaitForSeconds(5f);
            ClearChainEffects();
        }
        
        /// <summary>
        /// Apply chain effect to enemy
        /// </summary>
        private void ApplyChainEffect(EnemyAI enemy)
        {
            // Create visual chain effect
            GameObject chainEffect = new GameObject("ChainEffect");
            chainEffect.transform.SetParent(enemy.transform);
            chainEffect.transform.localPosition = Vector3.zero;
            
            // Add chain visual
            LineRenderer chainLine = chainEffect.AddComponent<LineRenderer>();
            chainLine.material = new Material(Shader.Find("Sprites/Default"));
            chainLine.color = Color.red;
            chainLine.startWidth = 0.1f;
            chainLine.endWidth = 0.1f;
            chainLine.positionCount = 2;
            
            // Set chain positions
            chainLine.SetPosition(0, transform.position);
            chainLine.SetPosition(1, enemy.transform.position);
            
            // Store reference for cleanup
            enemy.GetComponent<ThighMasterChainEffect>()?.Cleanup();
            ThighMasterChainEffect chainComponent = enemy.gameObject.AddComponent<ThighMasterChainEffect>();
            chainComponent.Initialize(chainEffect, 5f);
        }
        
        /// <summary>
        /// Clear all chain effects
        /// </summary>
        private void ClearChainEffects()
        {
            foreach (EnemyAI enemy in chainedEnemies)
            {
                ThighMasterChainEffect chainEffect = enemy.GetComponent<ThighMasterChainEffect>();
                if (chainEffect != null)
                {
                    chainEffect.Cleanup();
                }
            }
            
            chainedEnemies.Clear();
        }
        
        protected override void ExecuteUltimateAbility()
        {
            // "Thighs of Steel" - Become immune to damage while crushing everything
            StartCoroutine(ThighsOfSteel());
            
            Debug.Log("Thigh Master Ultimate: Thighs of Steel - UNSTOPPABLE CRUSHING FORCE!");
        }
        
        /// <summary>
        /// Thighs of Steel - Ultimate ability
        /// </summary>
        private System.Collections.IEnumerator ThighsOfSteel()
        {
            float ultimateDuration = 8f;
            float crushInterval = 0.2f;
            
            // Make player immune to damage
            PlayerController player = GetComponentInParent<PlayerController>();
            if (player != null)
            {
                // This would integrate with player invincibility system
                // player.SetInvincible(true);
            }
            
            // Continuous crushing damage
            while (ultimateDuration > 0)
            {
                // Find all enemies in large radius
                Collider[] enemies = Physics.OverlapSphere(transform.position, range * 2f);
                
                foreach (Collider enemy in enemies)
                {
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        float crushDamage = baseDamage * 2f;
                        enemyAI.TakeDamage(crushDamage, weaponType);
                        
                        // Apply "crushed" debuff
                        enemyAI.ApplyDebuff("Crushed", 3f, 1f);
                    }
                }
                
                // Maximum haptic feedback
                TriggerHapticFeedback(hapticIntensity * 3f, crushInterval);
                
                ultimateDuration -= crushInterval;
                yield return new WaitForSeconds(crushInterval);
            }
            
            // Remove invincibility
            if (player != null)
            {
                // player.SetInvincible(false);
            }
        }
        
        protected override void EndAttack()
        {
            base.EndAttack();
            isSqueezing = false;
            StopAllCoroutines();
        }
        
        /// <summary>
        /// Get Thigh Master specific weapon info
        /// </summary>
        public override WeaponInfo GetWeaponInfo()
        {
            WeaponInfo info = base.GetWeaponInfo();
            info.name = "Thigh Master of Crushing Victory";
            info.damage = baseDamage;
            info.attackSpeed = attackSpeed;
            return info;
        }
        
        private void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Draw combo radius
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, comboRadius);
        }
    }
    
    /// <summary>
    /// Component for Thigh Master chain effects
    /// </summary>
    public class ThighMasterChainEffect : MonoBehaviour
    {
        private GameObject chainEffect;
        private float duration;
        
        public void Initialize(GameObject effect, float effectDuration)
        {
            chainEffect = effect;
            duration = effectDuration;
            
            // Destroy after duration
            Destroy(gameObject, duration);
        }
        
        public void Cleanup()
        {
            if (chainEffect != null)
            {
                Destroy(chainEffect);
            }
            
            Destroy(this);
        }
        
        private void OnDestroy()
        {
            Cleanup();
        }
    }
}
