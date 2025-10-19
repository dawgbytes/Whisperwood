using UnityEngine;

namespace Whisperwood
{
    /// <summary>
    /// Shakes-Spear of Dramatic Effect - To glow, or not to glow... that is the question!
    /// Special Ability: Night Glow - Spear illuminates the battlefield and reveals hidden treasures
    /// Ultimate: "Bard's Radiance" - Create a massive light explosion that heals allies and damages enemies
    /// </summary>
    public class ShakesSpearWeapon : FitnessEquipmentWeapon
    {
        [Header("Shakes-Spear Specific")]
        public float glowIntensity = 1f;
        public float glowRadius = 8f;
        public float monologueDuration = 3f;
        public float monologueDamageMultiplier = 1.5f;
        public float treasureRevealRadius = 10f;
        
        [Header("Lighting Effects")]
        public Light spearLight;
        public GameObject glowEffect;
        public Color dayGlowColor = Color.white;
        public Color nightGlowColor = Color.blue;
        
        private bool isGlowing = false;
        private bool isMonologuing = false;
        private float monologueStartTime = 0f;
        private System.Collections.Generic.List<GameObject> revealedTreasures;
        
        protected override void InitializeWeapon()
        {
            weaponName = "Shakes-Spear of Dramatic Effect";
            weaponType = FitnessEquipmentType.ShakesSpear;
            baseDamage = 45f;
            attackSpeed = 1.2f;
            range = 3f;
            staminaCost = 12f;
            adaptiveTriggerResistance = 0.3f;
            hapticIntensity = 0.6f;
            specialAbilityCooldown = 12f;
            ultimateAbilityCooldown = 40f;
            
            revealedTreasures = new System.Collections.Generic.List<GameObject>();
            
            // Setup lighting
            SetupLighting();
        }
        
        /// <summary>
        /// Setup lighting effects
        /// </summary>
        private void SetupLighting()
        {
            // Create spear light if not assigned
            if (spearLight == null)
            {
                GameObject lightObject = new GameObject("SpearLight");
                lightObject.transform.SetParent(transform);
                lightObject.transform.localPosition = Vector3.forward * 0.5f;
                
                spearLight = lightObject.AddComponent<Light>();
                spearLight.type = LightType.Point;
                spearLight.range = glowRadius;
                spearLight.intensity = glowIntensity;
                spearLight.color = dayGlowColor;
                spearLight.enabled = false;
            }
            
            // Create glow effect if not assigned
            if (glowEffect == null)
            {
                glowEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                glowEffect.transform.SetParent(transform);
                glowEffect.transform.localPosition = Vector3.forward * 0.5f;
                glowEffect.transform.localScale = Vector3.one * 0.3f;
                
                Renderer renderer = glowEffect.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = dayGlowColor;
                renderer.material.SetFloat("_Metallic", 0.8f);
                renderer.material.SetFloat("_Smoothness", 0.9f);
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", dayGlowColor * 0.5f);
                
                glowEffect.SetActive(false);
            }
        }
        
        private void Update()
        {
            // Update lighting based on time of day
            UpdateLighting();
            
            // Update monologue effects
            UpdateMonologue();
        }
        
        /// <summary>
        /// Update lighting based on time of day
        /// </summary>
        private void UpdateLighting()
        {
            if (spearLight == null) return;
            
            // Determine if it's night (this would integrate with day/night cycle)
            bool isNight = IsNightTime();
            
            if (isGlowing)
            {
                spearLight.enabled = true;
                spearLight.color = isNight ? nightGlowColor : dayGlowColor;
                spearLight.intensity = glowIntensity * (isNight ? 1.5f : 1f);
                
                if (glowEffect != null)
                {
                    glowEffect.SetActive(true);
                    Renderer renderer = glowEffect.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = isNight ? nightGlowColor : dayGlowColor;
                        renderer.material.SetColor("_EmissionColor", (isNight ? nightGlowColor : dayGlowColor) * 0.5f);
                    }
                }
            }
        }
        
        /// <summary>
        /// Check if it's night time
        /// </summary>
        private bool IsNightTime()
        {
            // This would integrate with day/night cycle system
            // For now, simulate based on time
            float timeOfDay = (Time.time % 60f) / 60f; // 60 second day cycle
            return timeOfDay > 0.5f; // Night after 30 seconds
        }
        
        /// <summary>
        /// Update monologue effects
        /// </summary>
        private void UpdateMonologue()
        {
            if (isMonologuing)
            {
                float monologueProgress = (Time.time - monologueStartTime) / monologueDuration;
                
                if (monologueProgress >= 1f)
                {
                    EndMonologue();
                }
                else
                {
                    // Apply monologue effects to nearby enemies
                    ApplyMonologueEffects(monologueProgress);
                }
            }
        }
        
        public override void PerformBasicAttack()
        {
            if (!CanAttack()) return;
            
            // Shakes-Spear basic attack with dramatic flair
            PerformDramaticThrust();
            base.PerformBasicAttack();
        }
        
        /// <summary>
        /// Perform dramatic thrust attack
        /// </summary>
        private void PerformDramaticThrust()
        {
            // Trigger dramatic lighting effect
            TriggerDramaticLighting();
            
            // Apply damage with dramatic timing
            float damage = CalculateDamage();
            ApplyDamage(damage);
            
            Debug.Log("Shakes-Spear Attack: To be, or not to be... DEFEATED!");
        }
        
        /// <summary>
        /// Trigger dramatic lighting effect
        /// </summary>
        private void TriggerDramaticLighting()
        {
            if (spearLight != null)
            {
                StartCoroutine(DramaticLightingSequence());
            }
        }
        
        /// <summary>
        /// Dramatic lighting sequence
        /// </summary>
        private System.Collections.IEnumerator DramaticLightingSequence()
        {
            float originalIntensity = spearLight.intensity;
            Color originalColor = spearLight.color;
            
            // Flash bright
            spearLight.intensity = originalIntensity * 3f;
            spearLight.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            
            // Return to normal
            spearLight.intensity = originalIntensity;
            spearLight.color = originalColor;
        }
        
        protected override void ExecuteSpecialAbility()
        {
            // Night Glow - Illuminate battlefield and reveal treasures
            ActivateNightGlow();
            
            Debug.Log("Shakes-Spear Special: Night Glow - 'Light, give me light!'");
        }
        
        /// <summary>
        /// Activate night glow ability
        /// </summary>
        private void ActivateNightGlow()
        {
            isGlowing = true;
            
            // Reveal hidden treasures
            RevealHiddenTreasures();
            
            // Illuminate the battlefield
            IlluminateBattlefield();
            
            // Glow for 15 seconds
            Invoke(nameof(DeactivateNightGlow), 15f);
        }
        
        /// <summary>
        /// Reveal hidden treasures
        /// </summary>
        private void RevealHiddenTreasures()
        {
            // Find hidden treasure objects
            Collider[] objects = Physics.OverlapSphere(transform.position, treasureRevealRadius);
            
            foreach (Collider obj in objects)
            {
                if (obj.CompareTag("HiddenTreasure"))
                {
                    // Make treasure visible
                    obj.gameObject.SetActive(true);
                    revealedTreasures.Add(obj.gameObject);
                    
                    // Add glow effect
                    AddTreasureGlow(obj.gameObject);
                }
            }
        }
        
        /// <summary>
        /// Add glow effect to treasure
        /// </summary>
        private void AddTreasureGlow(GameObject treasure)
        {
            GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            glow.transform.SetParent(treasure.transform);
            glow.transform.localPosition = Vector3.zero;
            glow.transform.localScale = Vector3.one * 1.2f;
            
            Renderer renderer = glow.GetComponent<Renderer>();
            renderer.material.color = Color.yellow;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", Color.yellow * 0.8f);
            
            glow.GetComponent<Collider>().enabled = false;
        }
        
        /// <summary>
        /// Illuminate battlefield
        /// </summary>
        private void IlluminateBattlefield()
        {
            if (spearLight != null)
            {
                spearLight.range = glowRadius * 2f;
                spearLight.intensity = glowIntensity * 2f;
            }
        }
        
        /// <summary>
        /// Deactivate night glow
        /// </summary>
        private void DeactivateNightGlow()
        {
            isGlowing = false;
            
            if (spearLight != null)
            {
                spearLight.enabled = false;
                spearLight.range = glowRadius;
                spearLight.intensity = glowIntensity;
            }
            
            if (glowEffect != null)
            {
                glowEffect.SetActive(false);
            }
            
            // Hide revealed treasures
            foreach (GameObject treasure in revealedTreasures)
            {
                if (treasure != null)
                {
                    treasure.SetActive(false);
                }
            }
            revealedTreasures.Clear();
        }
        
        protected override void ExecuteUltimateAbility()
        {
            // "Bard's Radiance" - Massive light explosion
            StartCoroutine(BardsRadiance());
            
            Debug.Log("Shakes-Spear Ultimate: Bard's Radiance - 'All the world's a stage!'");
        }
        
        /// <summary>
        /// Bard's Radiance ultimate ability
        /// </summary>
        private System.Collections.IEnumerator BardsRadiance()
        {
            float explosionRadius = 15f;
            float explosionDuration = 3f;
            
            // Create massive light explosion
            GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            explosion.transform.position = transform.position;
            explosion.transform.localScale = Vector3.zero;
            explosion.GetComponent<Collider>().enabled = false;
            
            Renderer explosionRenderer = explosion.GetComponent<Renderer>();
            explosionRenderer.material = new Material(Shader.Find("Standard"));
            explosionRenderer.material.color = Color.white;
            explosionRenderer.material.EnableKeyword("_EMISSION");
            explosionRenderer.material.SetColor("_EmissionColor", Color.white * 2f);
            
            // Animate explosion
            float startTime = Time.time;
            while (Time.time - startTime < explosionDuration)
            {
                float progress = (Time.time - startTime) / explosionDuration;
                explosion.transform.localScale = Vector3.one * explosionRadius * progress;
                
                // Apply damage to enemies
                Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRadius * progress);
                foreach (Collider enemy in enemies)
                {
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        float damage = baseDamage * 2f * (1f - progress); // Damage decreases over time
                        enemyAI.TakeDamage(damage, weaponType);
                    }
                }
                
                // Heal allies
                Collider[] allies = Physics.OverlapSphere(transform.position, explosionRadius * progress);
                foreach (Collider ally in allies)
                {
                    if (ally.CompareTag("Player"))
                    {
                        // This would integrate with player health system
                        // ally.GetComponent<PlayerController>().Heal(baseDamage * 0.5f);
                    }
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            
            Destroy(explosion);
        }
        
        /// <summary>
        /// Apply monologue effects to nearby enemies
        /// </summary>
        private void ApplyMonologueEffects(float progress)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, range * 2f);
            
            foreach (Collider enemy in enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // Apply "mesmerized" debuff
                    enemyAI.ApplyDebuff("Mesmerized", 1f, progress);
                }
            }
        }
        
        /// <summary>
        /// Start monologue
        /// </summary>
        public void StartMonologue()
        {
            if (isMonologuing) return;
            
            isMonologuing = true;
            monologueStartTime = Time.time;
            
            // Trigger dramatic lighting
            TriggerDramaticLighting();
            
            // Apply monologue damage multiplier
            float monologueDamage = CalculateDamage() * monologueDamageMultiplier;
            ApplyDamage(monologueDamage);
            
            Debug.Log("Shakes-Spear Monologue: 'To be, or not to be... that is the question!'");
        }
        
        /// <summary>
        /// End monologue
        /// </summary>
        private void EndMonologue()
        {
            isMonologuing = false;
        }
        
        protected override float CalculateDamage()
        {
            float damage = base.CalculateDamage();
            
            // Night time bonus
            if (IsNightTime())
            {
                damage *= 1.3f;
            }
            
            // Monologue bonus
            if (isMonologuing)
            {
                damage *= monologueDamageMultiplier;
            }
            
            return damage;
        }
        
        /// <summary>
        /// Get Shakes-Spear specific weapon info
        /// </summary>
        public override WeaponInfo GetWeaponInfo()
        {
            WeaponInfo info = base.GetWeaponInfo();
            info.name = "Shakes-Spear of Dramatic Effect";
            info.damage = CalculateDamage();
            info.attackSpeed = attackSpeed;
            return info;
        }
        
        private void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Draw glow radius
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, glowRadius);
            
            // Draw treasure reveal radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, treasureRevealRadius);
        }
    }
}
