using UnityEngine;

namespace Whisperwood
{
    /// <summary>
    /// Gucci Pixie AI - Tiny terrors in $3,000 jackets who shoot designer bullets
    /// Fast, agile enemies with high fashion sense and attitude
    /// </summary>
    public class GucciPixieAI : EnemyAI
    {
        [Header("Gucci Pixie Specific")]
        public float flightHeight = 3f;
        public float designerBulletDamage = 15f;
        public float designerBulletSpeed = 20f;
        public GameObject designerBulletPrefab;
        public Transform bulletSpawnPoint;
        
        [Header("Fashion Abilities")]
        public float styleBoostRadius = 5f;
        public float styleBoostMultiplier = 1.5f;
        public bool hasStyleBoost = false;
        
        private bool isFlying = false;
        private Vector3 flightTarget;
        
        protected override void InitializeEnemy()
        {
            enemyName = "Gucci Pixie";
            maxHealth = 60f;
            attackDamage = 20f;
            attackSpeed = 2f;
            moveSpeed = 4f;
            detectionRange = 12f;
            attackRange = 8f;
            aggroRange = 15f;
            brand = FashionBrand.Gucci;
            fashionSense = 1.5f;
            attitudeLevel = 2f;
            isWearingDesigner = true;
            
            // Gucci Pixies are fast and agile
            patrolRadius = 8f;
            patrolWaitTime = 1f;
            attackCooldown = 0.5f;
        }
        
        protected override void SetupAI()
        {
            base.SetupAI();
            
            // Enable flying behavior
            isFlying = true;
            flightTarget = transform.position + Vector3.up * flightHeight;
            
            // Setup bullet spawn point if not assigned
            if (bulletSpawnPoint == null)
            {
                GameObject spawnPoint = new GameObject("BulletSpawnPoint");
                spawnPoint.transform.SetParent(transform);
                spawnPoint.transform.localPosition = Vector3.forward * 1f + Vector3.up * 0.5f;
                bulletSpawnPoint = spawnPoint.transform;
            }
        }
        
        protected override void Update()
        {
            base.Update();
            
            // Update flying behavior
            UpdateFlyingBehavior();
            
            // Update style boost
            UpdateStyleBoost();
        }
        
        /// <summary>
        /// Update flying behavior
        /// </summary>
        private void UpdateFlyingBehavior()
        {
            if (!isFlying) return;
            
            // Maintain flight height
            Vector3 currentPos = transform.position;
            if (currentPos.y < flightTarget.y)
            {
                currentPos.y = Mathf.Lerp(currentPos.y, flightTarget.y, Time.deltaTime * 2f);
                transform.position = currentPos;
            }
            
            // Hover in place when attacking
            if (currentState == EnemyState.Attacking)
            {
                if (navAgent != null)
                {
                    navAgent.enabled = false;
                }
            }
            else
            {
                if (navAgent != null && !navAgent.enabled)
                {
                    navAgent.enabled = true;
                }
            }
        }
        
        /// <summary>
        /// Update style boost ability
        /// </summary>
        private void UpdateStyleBoost()
        {
            // Check for nearby allies to boost
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, styleBoostRadius);
            
            foreach (Collider enemy in nearbyEnemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null && enemyAI != this)
                {
                    // Apply style boost to nearby allies
                    if (!hasStyleBoost)
                    {
                        ApplyStyleBoostToAlly(enemyAI);
                    }
                }
            }
        }
        
        /// <summary>
        /// Apply style boost to ally
        /// </summary>
        private void ApplyStyleBoostToAlly(EnemyAI ally)
        {
            // Increase ally's damage and speed
            ally.attackDamage *= styleBoostMultiplier;
            if (ally.navAgent != null)
            {
                ally.navAgent.speed *= styleBoostMultiplier;
            }
            
            Debug.Log($"{enemyName} boosted {ally.enemyName} with Gucci style!");
        }
        
        protected override void PerformAttack()
        {
            if (player == null) return;
            
            // Gucci Pixies shoot designer bullets
            ShootDesignerBullet();
            
            base.PerformAttack();
        }
        
        /// <summary>
        /// Shoot designer bullet at player
        /// </summary>
        private void ShootDesignerBullet()
        {
            if (player == null || bulletSpawnPoint == null) return;
            
            // Calculate direction to player
            Vector3 direction = (player.position - bulletSpawnPoint.position).normalized;
            
            // Create designer bullet
            if (designerBulletPrefab != null)
            {
                GameObject bullet = Instantiate(designerBulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(direction));
                
                // Configure bullet
                DesignerBulletController bulletController = bullet.GetComponent<DesignerBulletController>();
                if (bulletController != null)
                {
                    bulletController.Initialize(direction, designerBulletSpeed, designerBulletDamage, brand);
                }
                else
                {
                    // Add basic bullet controller if not present
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = direction * designerBulletSpeed;
                    }
                }
                
                // Destroy bullet after 5 seconds
                Destroy(bullet, 5f);
            }
            
            Debug.Log($"{enemyName} shot a designer bullet!");
        }
        
        protected override void ExecuteSpecialAbility()
        {
            // "Fashion Week Fury" - Rapid fire designer bullets
            StartCoroutine(FashionWeekFury());
        }
        
        /// <summary>
        /// Fashion Week Fury - Rapid fire attack
        /// </summary>
        private System.Collections.IEnumerator FashionWeekFury()
        {
            int bulletCount = 8;
            float delay = 0.1f;
            
            for (int i = 0; i < bulletCount; i++)
            {
                ShootDesignerBullet();
                yield return new WaitForSeconds(delay);
            }
            
            Debug.Log($"{enemyName} unleashed Fashion Week Fury!");
        }
        
        /// <summary>
        /// "Designer Dominance" - Summon Gucci reinforcements
        /// </summary>
        protected override void ExecuteUltimateAbility()
        {
            // Summon 3 additional Gucci Pixies
            for (int i = 0; i < 3; i++)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 5f;
                spawnPosition.y = transform.position.y;
                
                // Create new Gucci Pixie
                GameObject newPixie = Instantiate(gameObject, spawnPosition, transform.rotation);
                GucciPixieAI newPixieAI = newPixie.GetComponent<GucciPixieAI>();
                
                // Make it temporary (disappears after 30 seconds)
                Destroy(newPixie, 30f);
            }
            
            Debug.Log($"{enemyName} summoned Gucci reinforcements!");
        }
        
        protected override float ApplyWeaponDamageModifier(float damage, FitnessEquipmentType weaponType)
        {
            float modifier = base.ApplyWeaponDamageModifier(damage, weaponType);
            
            // Gucci Pixies are vulnerable to Shakes-Spear (dramatic effect)
            if (weaponType == FitnessEquipmentType.ShakesSpear)
            {
                modifier *= 1.3f;
            }
            
            // But resistant to Thigh Master (too crude for their taste)
            if (weaponType == FitnessEquipmentType.ThighMaster)
            {
                modifier *= 0.8f;
            }
            
            return modifier;
        }
        
        protected override void PlayAttackSound()
        {
            if (audioSource != null)
            {
                // Play high-pitched designer attack sound
                Debug.Log($"{enemyName} attack sound: 'Fabulous!'");
            }
        }
        
        protected override void PlayDamageEffect()
        {
            // Create designer sparkle effect
            GameObject sparkleEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sparkleEffect.transform.position = transform.position;
            sparkleEffect.transform.localScale = Vector3.one * 0.5f;
            sparkleEffect.GetComponent<Renderer>().material.color = Color.gold;
            sparkleEffect.GetComponent<Collider>().enabled = false;
            
            Destroy(sparkleEffect, 0.5f);
        }
        
        protected override void PlayDeathSound()
        {
            if (audioSource != null)
            {
                // Play dramatic death sound
                Debug.Log($"{enemyName} death sound: 'This is so not fabulous!'");
            }
        }
        
        protected override void DropLoot()
        {
            base.DropLoot();
            
            // Gucci Pixies drop designer accessories
            Debug.Log($"{enemyName} dropped Gucci accessories!");
        }
        
        private void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Draw style boost radius
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, styleBoostRadius);
            
            // Draw flight height
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * flightHeight);
        }
    }
    
    /// <summary>
    /// Controller for designer bullets
    /// </summary>
    public class DesignerBulletController : MonoBehaviour
    {
        private Vector3 direction;
        private float speed;
        private float damage;
        private FashionBrand brand;
        private bool hasHit = false;
        
        public void Initialize(Vector3 bulletDirection, float bulletSpeed, float bulletDamage, FashionBrand bulletBrand)
        {
            direction = bulletDirection;
            speed = bulletSpeed;
            damage = bulletDamage;
            brand = bulletBrand;
            
            // Add rigidbody and launch
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            rb.velocity = direction * speed;
            
            // Set bullet appearance based on brand
            SetBulletAppearance();
        }
        
        /// <summary>
        /// Set bullet appearance based on brand
        /// </summary>
        private void SetBulletAppearance()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                switch (brand)
                {
                    case FashionBrand.Gucci:
                        renderer.material.color = Color.green;
                        break;
                    case FashionBrand.Prada:
                        renderer.material.color = Color.black;
                        break;
                    case FashionBrand.Versace:
                        renderer.material.color = Color.yellow;
                        break;
                    case FashionBrand.Chanel:
                        renderer.material.color = Color.white;
                        break;
                    default:
                        renderer.material.color = Color.gray;
                        break;
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            
            // Check if hit player
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    // Apply damage to player
                    // This would integrate with player health system
                    Debug.Log($"Player hit by {brand} designer bullet for {damage} damage!");
                    
                    hasHit = true;
                    Destroy(gameObject);
                }
            }
            
            // Check if hit environment
            if (other.CompareTag("Environment"))
            {
                hasHit = true;
                Destroy(gameObject);
            }
        }
    }
}
