using UnityEngine;
using UnityEngine.AI;

namespace Whisperwood
{
    /// <summary>
    /// Base AI class for all luxury fashion enemies in Gucci Goblins
    /// Handles AI behavior, combat patterns, and PS5-specific interactions
    /// </summary>
    public abstract class EnemyAI : MonoBehaviour
    {
        [Header("Enemy Stats")]
        public string enemyName;
        public float maxHealth = 100f;
        public float currentHealth;
        public float attackDamage = 25f;
        public float attackSpeed = 1f;
        public float moveSpeed = 3f;
        public float detectionRange = 10f;
        public float attackRange = 2f;
        public float aggroRange = 15f;
        
        [Header("AI Behavior")]
        public EnemyState currentState = EnemyState.Patrolling;
        public float patrolRadius = 5f;
        public float patrolWaitTime = 2f;
        public float attackCooldown = 1f;
        
        [Header("Luxury Fashion Properties")]
        public FashionBrand brand;
        public float fashionSense = 1f;
        public float attitudeLevel = 1f;
        public bool isWearingDesigner = true;
        
        [Header("PS5 Integration")]
        public bool enableHapticFeedback = true;
        public float hapticIntensity = 0.5f;
        
        // Components
        protected NavMeshAgent navAgent;
        protected Animator animator;
        protected AudioSource audioSource;
        protected Rigidbody rb;
        protected Collider enemyCollider;
        
        // State management
        protected Transform player;
        protected Vector3 lastKnownPlayerPosition;
        protected float lastAttackTime;
        protected float stateTimer;
        protected bool isDead = false;
        protected bool isStunned = false;
        protected bool isHypnotized = false;
        
        // Patrol system
        protected Vector3 patrolCenter;
        protected Vector3 patrolTarget;
        protected bool hasPatrolTarget = false;
        
        // Debuff system
        protected System.Collections.Generic.Dictionary<string, DebuffEffect> activeDebuffs;
        
        protected virtual void Awake()
        {
            // Get components
            navAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
            enemyCollider = GetComponent<Collider>();
            
            // Initialize
            currentHealth = maxHealth;
            patrolCenter = transform.position;
            activeDebuffs = new System.Collections.Generic.Dictionary<string, DebuffEffect>();
            
            InitializeEnemy();
        }
        
        protected virtual void Start()
        {
            // Find player
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            
            // Setup AI
            SetupAI();
        }
        
        protected virtual void Update()
        {
            if (isDead) return;
            
            // Update debuffs
            UpdateDebuffs();
            
            // Update state machine
            UpdateStateMachine();
            
            // Update animations
            UpdateAnimations();
        }
        
        /// <summary>
        /// Initialize enemy-specific properties
        /// </summary>
        protected abstract void InitializeEnemy();
        
        /// <summary>
        /// Setup AI behavior
        /// </summary>
        protected virtual void SetupAI()
        {
            if (navAgent != null)
            {
                navAgent.speed = moveSpeed;
                navAgent.stoppingDistance = attackRange * 0.8f;
            }
        }
        
        /// <summary>
        /// Update state machine
        /// </summary>
        protected virtual void UpdateStateMachine()
        {
            switch (currentState)
            {
                case EnemyState.Patrolling:
                    UpdatePatrolling();
                    break;
                case EnemyState.Chasing:
                    UpdateChasing();
                    break;
                case EnemyState.Attacking:
                    UpdateAttacking();
                    break;
                case EnemyState.Searching:
                    UpdateSearching();
                    break;
                case EnemyState.Stunned:
                    UpdateStunned();
                    break;
                case EnemyState.Hypnotized:
                    UpdateHypnotized();
                    break;
            }
        }
        
        /// <summary>
        /// Update patrolling state
        /// </summary>
        protected virtual void UpdatePatrolling()
        {
            // Check for player detection
            if (IsPlayerInRange(detectionRange))
            {
                TransitionToState(EnemyState.Chasing);
                return;
            }
            
            // Continue patrolling
            if (!hasPatrolTarget || navAgent.remainingDistance < 0.5f)
            {
                SetNewPatrolTarget();
            }
        }
        
        /// <summary>
        /// Update chasing state
        /// </summary>
        protected virtual void UpdateChasing()
        {
            if (player == null) return;
            
            // Check if player is still in aggro range
            if (!IsPlayerInRange(aggroRange))
            {
                TransitionToState(EnemyState.Searching);
                return;
            }
            
            // Check if in attack range
            if (IsPlayerInRange(attackRange))
            {
                TransitionToState(EnemyState.Attacking);
                return;
            }
            
            // Chase player
            if (navAgent != null)
            {
                navAgent.SetDestination(player.position);
                lastKnownPlayerPosition = player.position;
            }
        }
        
        /// <summary>
        /// Update attacking state
        /// </summary>
        protected virtual void UpdateAttacking()
        {
            if (player == null) return;
            
            // Check if player moved out of attack range
            if (!IsPlayerInRange(attackRange * 1.2f))
            {
                TransitionToState(EnemyState.Chasing);
                return;
            }
            
            // Perform attack
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
        
        /// <summary>
        /// Update searching state
        /// </summary>
        protected virtual void UpdateSearching()
        {
            // Move to last known player position
            if (navAgent != null && navAgent.remainingDistance < 1f)
            {
                // Search for a bit, then return to patrolling
                if (stateTimer > 5f)
                {
                    TransitionToState(EnemyState.Patrolling);
                }
            }
        }
        
        /// <summary>
        /// Update stunned state
        /// </summary>
        protected virtual void UpdateStunned()
        {
            if (!isStunned)
            {
                TransitionToState(EnemyState.Patrolling);
            }
        }
        
        /// <summary>
        /// Update hypnotized state
        /// </summary>
        protected virtual void UpdateHypnotized()
        {
            if (!isHypnotized)
            {
                TransitionToState(EnemyState.Patrolling);
            }
        }
        
        /// <summary>
        /// Set new patrol target
        /// </summary>
        protected virtual void SetNewPatrolTarget()
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += patrolCenter;
            randomDirection.y = patrolCenter.y;
            
            patrolTarget = randomDirection;
            hasPatrolTarget = true;
            
            if (navAgent != null)
            {
                navAgent.SetDestination(patrolTarget);
            }
        }
        
        /// <summary>
        /// Transition to new state
        /// </summary>
        protected virtual void TransitionToState(EnemyState newState)
        {
            currentState = newState;
            stateTimer = 0f;
            
            OnStateChanged(newState);
        }
        
        /// <summary>
        /// Called when state changes
        /// </summary>
        protected virtual void OnStateChanged(EnemyState newState)
        {
            Debug.Log($"{enemyName} transitioned to {newState}");
        }
        
        /// <summary>
        /// Check if player is in range
        /// </summary>
        protected virtual bool IsPlayerInRange(float range)
        {
            if (player == null) return false;
            
            float distance = Vector3.Distance(transform.position, player.position);
            return distance <= range;
        }
        
        /// <summary>
        /// Perform attack on player
        /// </summary>
        protected virtual void PerformAttack()
        {
            if (player == null) return;
            
            // Trigger attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // Play attack sound
            PlayAttackSound();
            
            // Apply damage to player
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // This would integrate with player health system
                // playerController.TakeDamage(attackDamage);
            }
            
            // Trigger haptic feedback
            TriggerHapticFeedback(hapticIntensity, 0.2f);
            
            Debug.Log($"{enemyName} attacked player for {attackDamage} damage!");
        }
        
        /// <summary>
        /// Take damage from player attack
        /// </summary>
        public virtual void TakeDamage(float damage, FitnessEquipmentType weaponType)
        {
            if (isDead) return;
            
            // Apply weapon-specific damage modifiers
            float modifiedDamage = ApplyWeaponDamageModifier(damage, weaponType);
            
            currentHealth -= modifiedDamage;
            
            // Trigger damage effects
            PlayDamageEffect();
            TriggerHapticFeedback(hapticIntensity * 1.5f, 0.3f);
            
            Debug.Log($"{enemyName} took {modifiedDamage} damage from {weaponType}! Health: {currentHealth}/{maxHealth}");
            
            // Check for death
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // Enter combat state if not already
                if (currentState == EnemyState.Patrolling)
                {
                    TransitionToState(EnemyState.Chasing);
                }
            }
        }
        
        /// <summary>
        /// Apply weapon-specific damage modifiers
        /// </summary>
        protected virtual float ApplyWeaponDamageModifier(float damage, FitnessEquipmentType weaponType)
        {
            float modifier = 1f;
            
            // Fashion enemies have different vulnerabilities
            switch (weaponType)
            {
                case FitnessEquipmentType.ShakeWeight:
                    // Shake-Weight is effective against all fashion enemies
                    modifier = 1.2f;
                    break;
                case FitnessEquipmentType.BowFlex:
                    // Bow-Flex is especially effective against Versace Valkyries
                    if (brand == FashionBrand.Versace)
                        modifier = 1.5f;
                    break;
                case FitnessEquipmentType.ThighMaster:
                    // Thigh Master is effective against Chanel Chimeras
                    if (brand == FashionBrand.Chanel)
                        modifier = 1.5f;
                    break;
                case FitnessEquipmentType.ShakesSpear:
                    // Shakes-Spear is effective against all but especially Gucci Pixies
                    modifier = 1.1f;
                    if (brand == FashionBrand.Gucci)
                        modifier = 1.3f;
                    break;
            }
            
            return damage * modifier;
        }
        
        /// <summary>
        /// Apply debuff effect
        /// </summary>
        public virtual void ApplyDebuff(string debuffName, float duration, float intensity)
        {
            if (activeDebuffs.ContainsKey(debuffName))
            {
                activeDebuffs[debuffName].duration = duration;
                activeDebuffs[debuffName].intensity = intensity;
            }
            else
            {
                activeDebuffs[debuffName] = new DebuffEffect
                {
                    name = debuffName,
                    duration = duration,
                    intensity = intensity,
                    startTime = Time.time
                };
            }
            
            // Apply debuff-specific effects
            ApplyDebuffEffect(debuffName, intensity);
            
            Debug.Log($"{enemyName} received debuff: {debuffName} for {duration} seconds");
        }
        
        /// <summary>
        /// Apply specific debuff effect
        /// </summary>
        protected virtual void ApplyDebuffEffect(string debuffName, float intensity)
        {
            switch (debuffName)
            {
                case "Shaken":
                    // Reduce movement speed
                    if (navAgent != null)
                        navAgent.speed = moveSpeed * (1f - intensity * 0.5f);
                    break;
                case "Hypnotized":
                    // Stop combat and stare at "commercial"
                    StopCombat(intensity);
                    break;
                case "Burned":
                    // Take damage over time
                    StartCoroutine(ApplyBurnDamage(intensity));
                    break;
            }
        }
        
        /// <summary>
        /// Stop combat for specified duration
        /// </summary>
        public virtual void StopCombat(float duration)
        {
            isHypnotized = true;
            TransitionToState(EnemyState.Hypnotized);
            
            // Resume combat after duration
            Invoke(nameof(ResumeCombat), duration);
        }
        
        /// <summary>
        /// Resume combat
        /// </summary>
        protected virtual void ResumeCombat()
        {
            isHypnotized = false;
            TransitionToState(EnemyState.Patrolling);
        }
        
        /// <summary>
        /// Update debuffs
        /// </summary>
        protected virtual void UpdateDebuffs()
        {
            var debuffsToRemove = new System.Collections.Generic.List<string>();
            
            foreach (var debuff in activeDebuffs)
            {
                debuff.Value.duration -= Time.deltaTime;
                
                if (debuff.Value.duration <= 0)
                {
                    debuffsToRemove.Add(debuff.Key);
                    RemoveDebuffEffect(debuff.Key);
                }
            }
            
            foreach (string debuffName in debuffsToRemove)
            {
                activeDebuffs.Remove(debuffName);
            }
        }
        
        /// <summary>
        /// Remove debuff effect
        /// </summary>
        protected virtual void RemoveDebuffEffect(string debuffName)
        {
            switch (debuffName)
            {
                case "Shaken":
                    // Restore movement speed
                    if (navAgent != null)
                        navAgent.speed = moveSpeed;
                    break;
            }
        }
        
        /// <summary>
        /// Apply burn damage over time
        /// </summary>
        private System.Collections.IEnumerator ApplyBurnDamage(float intensity)
        {
            float burnDuration = 5f;
            float burnInterval = 0.5f;
            
            while (burnDuration > 0 && currentHealth > 0)
            {
                float burnDamage = intensity * 5f;
                currentHealth -= burnDamage;
                
                Debug.Log($"{enemyName} burned for {burnDamage} damage!");
                
                burnDuration -= burnInterval;
                yield return new WaitForSeconds(burnInterval);
            }
        }
        
        /// <summary>
        /// Die
        /// </summary>
        protected virtual void Die()
        {
            isDead = true;
            currentState = EnemyState.Dead;
            
            // Play death animation
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            // Play death sound
            PlayDeathSound();
            
            // Trigger death haptic feedback
            TriggerHapticFeedback(hapticIntensity * 2f, 0.5f);
            
            // Drop loot
            DropLoot();
            
            // Destroy after delay
            Destroy(gameObject, 3f);
            
            Debug.Log($"{enemyName} has been defeated!");
        }
        
        /// <summary>
        /// Drop loot when defeated
        /// </summary>
        protected virtual void DropLoot()
        {
            // This would integrate with loot system
            Debug.Log($"{enemyName} dropped luxury fashion loot!");
        }
        
        /// <summary>
        /// Play attack sound
        /// </summary>
        protected abstract void PlayAttackSound();
        
        /// <summary>
        /// Play damage effect
        /// </summary>
        protected abstract void PlayDamageEffect();
        
        /// <summary>
        /// Play death sound
        /// </summary>
        protected abstract void PlayDeathSound();
        
        /// <summary>
        /// Update animations
        /// </summary>
        protected virtual void UpdateAnimations()
        {
            if (animator == null) return;
            
            // Update movement animations
            if (navAgent != null)
            {
                animator.SetFloat("Speed", navAgent.velocity.magnitude);
                animator.SetBool("IsMoving", navAgent.velocity.magnitude > 0.1f);
            }
            
            // Update state animations
            animator.SetBool("IsAttacking", currentState == EnemyState.Attacking);
            animator.SetBool("IsChasing", currentState == EnemyState.Chasing);
            animator.SetBool("IsStunned", isStunned);
            animator.SetBool("IsHypnotized", isHypnotized);
        }
        
        /// <summary>
        /// Trigger haptic feedback
        /// </summary>
        protected virtual void TriggerHapticFeedback(float intensity, float duration)
        {
            if (!enableHapticFeedback) return;
            
            // This would integrate with PlayStation SDK
            Debug.Log($"Enemy Haptic Feedback: {enemyName} - Intensity: {intensity}, Duration: {duration}");
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Draw aggro range
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, aggroRange);
            
            // Draw patrol radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolCenter, patrolRadius);
        }
    }
    
    /// <summary>
    /// Enemy states
    /// </summary>
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
        Searching,
        Stunned,
        Hypnotized,
        Dead
    }
    
    /// <summary>
    /// Fashion brands for enemies
    /// </summary>
    public enum FashionBrand
    {
        Gucci,
        Prada,
        Versace,
        Chanel,
        Generic
    }
    
    /// <summary>
    /// Debuff effect structure
    /// </summary>
    [System.Serializable]
    public struct DebuffEffect
    {
        public string name;
        public float duration;
        public float intensity;
        public float startTime;
    }
}
