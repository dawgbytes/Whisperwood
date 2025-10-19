using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Whisperwood
{
    /// <summary>
    /// Manager for Fashion Week Massacre battle royale mode
    /// Handles game state, player spawning, storm mechanics, and victory conditions
    /// </summary>
    public class FashionWeekMassacreManager : MonoBehaviour
    {
        [Header("Battle Royale Settings")]
        public int maxPlayers = 60;
        public int minPlayersToStart = 10;
        public float gameStartDelay = 10f;
        public float matchDuration = 20f; // 20 minutes
        public float stormDamagePerSecond = 10f;
        
        [Header("Storm Settings")]
        public float initialStormRadius = 100f;
        public float finalStormRadius = 10f;
        public float stormShrinkSpeed = 1f;
        public float stormWarningTime = 30f;
        public GameObject stormVisualEffect;
        
        [Header("Spawn Settings")]
        public Transform[] playerSpawnPoints;
        public Transform[] lootSpawnPoints;
        public Transform[] enemySpawnPoints;
        public float spawnProtectionTime = 5f;
        
        [Header("Loot Settings")]
        public GameObject[] fitnessEquipmentLoot;
        public GameObject[] designerWeaponLoot;
        public GameObject[] luxuryAccessoryLoot;
        public float lootSpawnInterval = 30f;
        public int maxLootPerSpawn = 5;
        
        [Header("Enemy Settings")]
        public GameObject[] luxuryFashionEnemies;
        public float enemySpawnInterval = 45f;
        public int maxEnemiesPerSpawn = 8;
        
        // Game state
        private BattleRoyaleGameState currentGameState;
        private float gameStartTime;
        private float currentStormRadius;
        private int alivePlayers;
        private int totalPlayers;
        private List<PlayerController> players;
        private List<GameObject> activeLoot;
        private List<GameObject> activeEnemies;
        
        // Events
        public System.Action<BattleRoyaleGameState> OnGameStateChanged;
        public System.Action<int> OnPlayerCountChanged;
        public System.Action<float> OnStormRadiusChanged;
        public System.Action<string> OnGameEnded;
        
        // Singleton
        public static FashionWeekMassacreManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeBattleRoyale();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            StartBattleRoyale();
        }
        
        private void Update()
        {
            UpdateGameState();
            UpdateStorm();
            UpdateLootSpawns();
            UpdateEnemySpawns();
        }
        
        /// <summary>
        /// Initialize battle royale system
        /// </summary>
        private void InitializeBattleRoyale()
        {
            currentGameState = BattleRoyaleGameState.WaitingForPlayers;
            currentStormRadius = initialStormRadius;
            alivePlayers = 0;
            totalPlayers = 0;
            players = new List<PlayerController>();
            activeLoot = new List<GameObject>();
            activeEnemies = new List<GameObject>();
            
            Debug.Log("Fashion Week Massacre Battle Royale initialized!");
        }
        
        /// <summary>
        /// Start battle royale match
        /// </summary>
        private void StartBattleRoyale()
        {
            // Start waiting for players
            TransitionToState(BattleRoyaleGameState.WaitingForPlayers);
            
            // Start game after delay
            Invoke(nameof(StartGame), gameStartDelay);
        }
        
        /// <summary>
        /// Start the actual game
        /// </summary>
        private void StartGame()
        {
            if (totalPlayers < minPlayersToStart)
            {
                Debug.Log("Not enough players to start game. Waiting...");
                Invoke(nameof(StartGame), 5f);
                return;
            }
            
            TransitionToState(BattleRoyaleGameState.InProgress);
            gameStartTime = Time.time;
            
            // Spawn all players
            SpawnAllPlayers();
            
            // Start storm shrinking
            StartCoroutine(ShrinkStorm());
            
            Debug.Log("Fashion Week Massacre has begun!");
        }
        
        /// <summary>
        /// Transition to new game state
        /// </summary>
        private void TransitionToState(BattleRoyaleGameState newState)
        {
            currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            Debug.Log($"Battle Royale state changed to: {newState}");
        }
        
        /// <summary>
        /// Update game state
        /// </summary>
        private void UpdateGameState()
        {
            switch (currentGameState)
            {
                case BattleRoyaleGameState.InProgress:
                    UpdateInProgressState();
                    break;
                case BattleRoyaleGameState.Ending:
                    UpdateEndingState();
                    break;
            }
        }
        
        /// <summary>
        /// Update in progress state
        /// </summary>
        private void UpdateInProgressState()
        {
            // Check for victory conditions
            if (alivePlayers <= 1)
            {
                EndGame();
                return;
            }
            
            // Check for time limit
            if (Time.time - gameStartTime >= matchDuration * 60f)
            {
                EndGame();
                return;
            }
            
            // Update alive players count
            UpdateAlivePlayersCount();
        }
        
        /// <summary>
        /// Update ending state
        /// </summary>
        private void UpdateEndingState()
        {
            // Handle game end logic
            // This would include showing results, returning to lobby, etc.
        }
        
        /// <summary>
        /// Update alive players count
        /// </summary>
        private void UpdateAlivePlayersCount()
        {
            int newAliveCount = 0;
            
            foreach (PlayerController player in players)
            {
                if (player != null && player.gameObject.activeInHierarchy)
                {
                    // Check if player is alive (this would integrate with health system)
                    // if (player.IsAlive())
                    // {
                    //     newAliveCount++;
                    // }
                    newAliveCount++; // Temporary for testing
                }
            }
            
            if (newAliveCount != alivePlayers)
            {
                alivePlayers = newAliveCount;
                OnPlayerCountChanged?.Invoke(alivePlayers);
            }
        }
        
        /// <summary>
        /// Update storm mechanics
        /// </summary>
        private void UpdateStorm()
        {
            if (currentGameState != BattleRoyaleGameState.InProgress) return;
            
            // Apply storm damage to players outside safe zone
            ApplyStormDamage();
        }
        
        /// <summary>
        /// Apply storm damage to players outside safe zone
        /// </summary>
        private void ApplyStormDamage()
        {
            foreach (PlayerController player in players)
            {
                if (player == null) continue;
                
                float distanceFromCenter = Vector3.Distance(player.transform.position, Vector3.zero);
                
                if (distanceFromCenter > currentStormRadius)
                {
                    // Apply storm damage
                    // This would integrate with player health system
                    // player.TakeDamage(stormDamagePerSecond * Time.deltaTime);
                    
                    Debug.Log($"Player {player.name} taking storm damage!");
                }
            }
        }
        
        /// <summary>
        /// Shrink storm over time
        /// </summary>
        private System.Collections.IEnumerator ShrinkStorm()
        {
            float shrinkDuration = matchDuration * 60f; // Total match duration
            float startRadius = initialStormRadius;
            float endRadius = finalStormRadius;
            
            while (currentGameState == BattleRoyaleGameState.InProgress)
            {
                float elapsedTime = Time.time - gameStartTime;
                float progress = elapsedTime / shrinkDuration;
                
                currentStormRadius = Mathf.Lerp(startRadius, endRadius, progress);
                OnStormRadiusChanged?.Invoke(currentStormRadius);
                
                // Update storm visual effect
                UpdateStormVisualEffect();
                
                yield return new WaitForSeconds(1f);
            }
        }
        
        /// <summary>
        /// Update storm visual effect
        /// </summary>
        private void UpdateStormVisualEffect()
        {
            if (stormVisualEffect != null)
            {
                stormVisualEffect.transform.localScale = Vector3.one * currentStormRadius * 2f;
            }
        }
        
        /// <summary>
        /// Update loot spawns
        /// </summary>
        private void UpdateLootSpawns()
        {
            if (currentGameState != BattleRoyaleGameState.InProgress) return;
            
            // Spawn loot at intervals
            if (Time.time % lootSpawnInterval < Time.deltaTime)
            {
                SpawnLoot();
            }
        }
        
        /// <summary>
        /// Spawn loot items
        /// </summary>
        private void SpawnLoot()
        {
            int lootToSpawn = Random.Range(1, maxLootPerSpawn + 1);
            
            for (int i = 0; i < lootToSpawn; i++)
            {
                if (lootSpawnPoints.Length > 0)
                {
                    Transform spawnPoint = lootSpawnPoints[Random.Range(0, lootSpawnPoints.Length)];
                    
                    // Choose random loot type
                    GameObject[] lootArray = GetRandomLootArray();
                    if (lootArray.Length > 0)
                    {
                        GameObject lootPrefab = lootArray[Random.Range(0, lootArray.Length)];
                        GameObject loot = Instantiate(lootPrefab, spawnPoint.position, spawnPoint.rotation);
                        
                        activeLoot.Add(loot);
                        
                        // Remove loot after 5 minutes
                        Destroy(loot, 300f);
                    }
                }
            }
        }
        
        /// <summary>
        /// Get random loot array
        /// </summary>
        private GameObject[] GetRandomLootArray()
        {
            int randomType = Random.Range(0, 3);
            
            switch (randomType)
            {
                case 0:
                    return fitnessEquipmentLoot;
                case 1:
                    return designerWeaponLoot;
                case 2:
                    return luxuryAccessoryLoot;
                default:
                    return fitnessEquipmentLoot;
            }
        }
        
        /// <summary>
        /// Update enemy spawns
        /// </summary>
        private void UpdateEnemySpawns()
        {
            if (currentGameState != BattleRoyaleGameState.InProgress) return;
            
            // Spawn enemies at intervals
            if (Time.time % enemySpawnInterval < Time.deltaTime)
            {
                SpawnEnemies();
            }
        }
        
        /// <summary>
        /// Spawn luxury fashion enemies
        /// </summary>
        private void SpawnEnemies()
        {
            int enemiesToSpawn = Random.Range(1, maxEnemiesPerSpawn + 1);
            
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (enemySpawnPoints.Length > 0)
                {
                    Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
                    
                    if (luxuryFashionEnemies.Length > 0)
                    {
                        GameObject enemyPrefab = luxuryFashionEnemies[Random.Range(0, luxuryFashionEnemies.Length)];
                        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                        
                        activeEnemies.Add(enemy);
                        
                        // Remove enemy after 10 minutes
                        Destroy(enemy, 600f);
                    }
                }
            }
        }
        
        /// <summary>
        /// Spawn all players
        /// </summary>
        private void SpawnAllPlayers()
        {
            if (playerSpawnPoints.Length == 0) return;
            
            for (int i = 0; i < totalPlayers; i++)
            {
                Transform spawnPoint = playerSpawnPoints[i % playerSpawnPoints.Length];
                
                // Spawn player at spawn point
                // This would integrate with player spawning system
                Debug.Log($"Spawning player {i + 1} at {spawnPoint.position}");
            }
        }
        
        /// <summary>
        /// Add player to battle royale
        /// </summary>
        public void AddPlayer(PlayerController player)
        {
            if (players.Contains(player)) return;
            
            players.Add(player);
            totalPlayers++;
            alivePlayers++;
            
            OnPlayerCountChanged?.Invoke(alivePlayers);
            
            Debug.Log($"Player {player.name} joined Fashion Week Massacre!");
        }
        
        /// <summary>
        /// Remove player from battle royale
        /// </summary>
        public void RemovePlayer(PlayerController player)
        {
            if (!players.Contains(player)) return;
            
            players.Remove(player);
            totalPlayers--;
            alivePlayers--;
            
            OnPlayerCountChanged?.Invoke(alivePlayers);
            
            Debug.Log($"Player {player.name} left Fashion Week Massacre!");
        }
        
        /// <summary>
        /// End the game
        /// </summary>
        private void EndGame()
        {
            TransitionToState(BattleRoyaleGameState.Ending);
            
            // Determine winner
            string winner = DetermineWinner();
            OnGameEnded?.Invoke(winner);
            
            Debug.Log($"Fashion Week Massacre ended! Winner: {winner}");
        }
        
        /// <summary>
        /// Determine game winner
        /// </summary>
        private string DetermineWinner()
        {
            if (alivePlayers == 1)
            {
                // Find the last surviving player
                foreach (PlayerController player in players)
                {
                    if (player != null && player.gameObject.activeInHierarchy)
                    {
                        return player.name;
                    }
                }
            }
            
            return "No Winner";
        }
        
        /// <summary>
        /// Get current game state
        /// </summary>
        public BattleRoyaleGameState GetGameState()
        {
            return currentGameState;
        }
        
        /// <summary>
        /// Get current storm radius
        /// </summary>
        public float GetStormRadius()
        {
            return currentStormRadius;
        }
        
        /// <summary>
        /// Get alive players count
        /// </summary>
        public int GetAlivePlayersCount()
        {
            return alivePlayers;
        }
        
        /// <summary>
        /// Get total players count
        /// </summary>
        public int GetTotalPlayersCount()
        {
            return totalPlayers;
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw storm radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.zero, currentStormRadius);
            
            // Draw spawn points
            Gizmos.color = Color.green;
            if (playerSpawnPoints != null)
            {
                foreach (Transform spawnPoint in playerSpawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawWireSphere(spawnPoint.position, 2f);
                    }
                }
            }
            
            // Draw loot spawn points
            Gizmos.color = Color.yellow;
            if (lootSpawnPoints != null)
            {
                foreach (Transform spawnPoint in lootSpawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawWireCube(spawnPoint.position, Vector3.one * 0.5f);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Battle royale game states
    /// </summary>
    public enum BattleRoyaleGameState
    {
        WaitingForPlayers,
        InProgress,
        Ending,
        Ended
    }
}
