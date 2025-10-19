using UnityEngine;

namespace Whisperwood
{
    /// <summary>
    /// Bow-Flex of Unbreakable Will - The machine that changed everything!
    /// Special Ability: Home Gym Advantage - Increased damage when near residential buildings
    /// Ultimate: "Total Body Transformation" - Transform into a human catapult for massive AOE damage
    /// </summary>
    public class BowFlexWeapon : FitnessEquipmentWeapon
    {
        [Header("Bow-Flex Specific")]
        public float resistanceBuildUp = 1f;
        public float maxResistance = 3f;
        public float homeGymBonusRadius = 10f;
        public float homeGymDamageMultiplier = 2f;
        public GameObject catapultProjectilePrefab;
        
        private float currentResistance = 1f;
        private bool isBuildingResistance = false;
        private float resistanceBuildTime = 0f;
        
        protected override void InitializeWeapon()
        {
            weaponName = "Bow-Flex of Unbreakable Will";
            weaponType = FitnessEquipmentType.BowFlex;
            baseDamage = 40f;
            attackSpeed = 1.5f;
            range = 4f;
            staminaCost = 15f;
            adaptiveTriggerResistance = 1.2f;
            hapticIntensity = 1f;
            specialAbilityCooldown = 20f;
            ultimateAbilityCooldown = 60f;
        }
        
        public override void PerformBasicAttack()
        {
            if (!CanAttack()) return;
            
            // Start building resistance for progressive damage
            StartBuildingResistance();
            base.PerformBasicAttack();
        }
        
        /// <summary>
        /// Start building resistance for progressive damage
        /// </summary>
        private void StartBuildingResistance()
        {
            isBuildingResistance = true;
            resistanceBuildTime = 0f;
            
            Debug.Log("Bow-Flex Attack: Resistance Training - Building up power!");
        }
        
        private void Update()
        {
            if (isBuildingResistance)
            {
                resistanceBuildTime += Time.deltaTime;
                currentResistance = Mathf.Lerp(1f, maxResistance, resistanceBuildTime / 2f);
                
                // Apply progressive haptic feedback
                float feedbackIntensity = Mathf.Lerp(hapticIntensity, hapticIntensity * 1.5f, resistanceBuildTime / 2f);
                TriggerHapticFeedback(feedbackIntensity, 0.1f);
                
                // Apply progressive adaptive trigger resistance
                if (enableAdaptiveTriggers)
                {
                    ApplyAdaptiveTriggerResistance(adaptiveTriggerResistance * currentResistance);
                }
                
                // Stop building resistance after 3 seconds
                if (resistanceBuildTime >= 3f)
                {
                    StopBuildingResistance();
                }
            }
        }
        
        /// <summary>
        /// Stop building resistance and release attack
        /// </summary>
        private void StopBuildingResistance()
        {
            isBuildingResistance = false;
            resistanceBuildTime = 0f;
            currentResistance = 1f;
        }
        
        protected override float CalculateDamage()
        {
            float damage = base.CalculateDamage();
            
            // Apply resistance multiplier
            damage *= currentResistance;
            
            // Apply home gym advantage if near residential buildings
            if (IsNearHomeGym())
            {
                damage *= homeGymDamageMultiplier;
                Debug.Log("Home Gym Advantage: Double damage bonus!");
            }
            
            return damage;
        }
        
        /// <summary>
        /// Check if player is near residential buildings (home gym advantage)
        /// </summary>
        private bool IsNearHomeGym()
        {
            // Check for residential building tags or components
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, homeGymBonusRadius);
            
            foreach (Collider obj in nearbyObjects)
            {
                // Check if object is tagged as residential
                if (obj.CompareTag("Residential") || obj.CompareTag("HomeGym"))
                {
                    return true;
                }
                
                // Check for home gym components
                HomeGymComponent homeGym = obj.GetComponent<HomeGymComponent>();
                if (homeGym != null)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        protected override void ExecuteSpecialAbility()
        {
            // Home Gym Advantage - Create temporary home gym area
            CreateTemporaryHomeGym();
            
            Debug.Log("Bow-Flex Special: Home Gym Advantage - 'It's the machine that changed everything!'");
        }
        
        /// <summary>
        /// Create temporary home gym area for bonus damage
        /// </summary>
        private void CreateTemporaryHomeGym()
        {
            // Create temporary home gym zone
            GameObject homeGymZone = new GameObject("TemporaryHomeGym");
            homeGymZone.transform.position = transform.position;
            homeGymZone.transform.localScale = Vector3.one * homeGymBonusRadius * 2f;
            
            // Add home gym component
            HomeGymComponent homeGym = homeGymZone.AddComponent<HomeGymComponent>();
            homeGym.Initialize(homeGymBonusRadius, 15f); // 15 second duration
            
            // Add visual effect
            GameObject visualEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualEffect.transform.SetParent(homeGymZone.transform);
            visualEffect.transform.localScale = Vector3.one * homeGymBonusRadius * 2f;
            visualEffect.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f, 0.3f);
            visualEffect.GetComponent<Collider>().enabled = false;
            
            // Destroy after duration
            Destroy(homeGymZone, 15f);
        }
        
        protected override void ExecuteUltimateAbility()
        {
            // "Total Body Transformation" - Transform into human catapult
            TransformIntoHumanCatapult();
            
            Debug.Log("Bow-Flex Ultimate: Total Body Transformation - HUMAN CATAPULT MODE!");
        }
        
        /// <summary>
        /// Transform into human catapult for massive AOE damage
        /// </summary>
        private void TransformIntoHumanCatapult()
        {
            // Launch multiple projectiles in all directions
            int projectileCount = 8;
            float angleStep = 360f / projectileCount;
            
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i * angleStep;
                Vector3 direction = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    0f,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );
                
                LaunchCatapultProjectile(direction);
            }
            
            // Apply massive haptic feedback
            TriggerHapticFeedback(hapticIntensity * 3f, 2f);
        }
        
        /// <summary>
        /// Launch catapult projectile in specified direction
        /// </summary>
        private void LaunchCatapultProjectile(Vector3 direction)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * 2f;
            
            if (catapultProjectilePrefab != null)
            {
                GameObject projectile = Instantiate(catapultProjectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
                
                // Configure projectile
                CatapultProjectileController controller = projectile.GetComponent<CatapultProjectileController>();
                if (controller != null)
                {
                    controller.Initialize(direction, baseDamage * 2f, 15f);
                }
                else
                {
                    // Add basic projectile controller if not present
                    Rigidbody rb = projectile.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = direction * 20f;
                    }
                }
                
                // Destroy after 10 seconds
                Destroy(projectile, 10f);
            }
        }
        
        protected override void EndAttack()
        {
            base.EndAttack();
            StopBuildingResistance();
        }
        
        /// <summary>
        /// Get Bow-Flex specific weapon info
        /// </summary>
        public override WeaponInfo GetWeaponInfo()
        {
            WeaponInfo info = base.GetWeaponInfo();
            info.name = "Bow-Flex of Unbreakable Will";
            info.damage = baseDamage * currentResistance;
            info.attackSpeed = attackSpeed;
            return info;
        }
    }
    
    /// <summary>
    /// Component for temporary home gym zones
    /// </summary>
    public class HomeGymComponent : MonoBehaviour
    {
        public float radius;
        public float duration;
        
        public void Initialize(float zoneRadius, float zoneDuration)
        {
            radius = zoneRadius;
            duration = zoneDuration;
            
            // Add trigger collider
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.radius = radius;
            trigger.isTrigger = true;
            
            // Destroy after duration
            Destroy(gameObject, duration);
        }
    }
    
    /// <summary>
    /// Controller for catapult projectiles
    /// </summary>
    public class CatapultProjectileController : MonoBehaviour
    {
        private Vector3 direction;
        private float damage;
        private float lifetime;
        private bool hasHit = false;
        
        public void Initialize(Vector3 launchDirection, float projectileDamage, float projectileLifetime)
        {
            direction = launchDirection;
            damage = projectileDamage;
            lifetime = projectileLifetime;
            
            // Add rigidbody and launch
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            rb.velocity = direction * 20f;
            
            // Destroy after lifetime
            Destroy(gameObject, lifetime);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, FitnessEquipmentType.BowFlex);
                hasHit = true;
                
                // Create explosion effect
                CreateExplosionEffect();
                
                Destroy(gameObject);
            }
        }
        
        private void CreateExplosionEffect()
        {
            // Create explosion particle effect
            GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            explosion.transform.position = transform.position;
            explosion.transform.localScale = Vector3.one * 3f;
            explosion.GetComponent<Renderer>().material.color = Color.red;
            explosion.GetComponent<Collider>().enabled = false;
            
            Destroy(explosion, 0.5f);
        }
    }
}
