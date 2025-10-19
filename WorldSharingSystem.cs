using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Whisperwood
{
    /// <summary>
    /// World Sharing System for Whisperwood
    /// Enables cross-platform world upload/download with automatic format conversion
    /// </summary>
    public class WorldSharingSystem : MonoBehaviour
    {
        [Header("World Sharing Settings")]
        public bool enableWorldSharing = true;
        public bool enableCrossPlatformSharing = true;
        public bool enableAutoFormatConversion = true;
        public bool enableCompression = true;
        public bool enableEncryption = true;
        
        [Header("Platform Settings")]
        public string platformIdentifier = "PC";
        public bool enableSteamWorkshop = true;
        public bool enablePlayStationSharing = true;
        public bool enableNintendoSharing = false;
        public bool enableXboxSharing = false;
        
        [Header("World Settings")]
        public int maxWorldSizeMB = 100;
        public int maxWorldsPerUser = 50;
        public bool enableWorldValidation = true;
        public bool enableWorldModeration = true;
        
        [Header("Sharing Features")]
        public bool enableWorldRatings = true;
        public bool enableWorldComments = true;
        public bool enableWorldFavorites = true;
        public bool enableWorldSearch = true;
        public bool enableWorldCategories = true;
        
        // World sharing components
        private WorldUploader worldUploader;
        private WorldDownloader worldDownloader;
        private WorldConverter worldConverter;
        private WorldValidator worldValidator;
        private WorldModerator worldModerator;
        private WorldSearchEngine worldSearchEngine;
        
        // Platform-specific handlers
        private SteamWorkshopHandler steamWorkshopHandler;
        private PlaystationSharingHandler playstationSharingHandler;
        private NintendoSharingHandler nintendoSharingHandler;
        private XboxSharingHandler xboxSharingHandler;
        
        // Local storage
        private LocalWorldStorage localWorldStorage;
        private WorldCache worldCache;
        private UserPreferences userPreferences;
        
        // Sharing state
        private Dictionary<string, WorldMetadata> sharedWorlds;
        private Dictionary<string, WorldMetadata> downloadedWorlds;
        private List<WorldMetadata> favoriteWorlds;
        private List<WorldMetadata> myWorlds;
        
        // Events
        public System.Action<string> OnWorldUploaded;
        public System.Action<string> OnWorldDownloaded;
        public System.Action<string> OnWorldDeleted;
        public System.Action<string> OnWorldRated;
        public System.Action<string> OnWorldFavorited;
        public System.Action<string> OnWorldShared;
        
        private void Awake()
        {
            InitializeWorldSharingSystem();
        }
        
        private void Start()
        {
            SetupWorldSharing();
            LoadUserData();
            InitializePlatformHandlers();
        }
        
        /// <summary>
        /// Initialize world sharing system
        /// </summary>
        public void Initialize(bool sharing, string platform)
        {
            enableWorldSharing = sharing;
            platformIdentifier = platform;
            
            InitializeWorldSharingSystem();
        }
        
        /// <summary>
        /// Initialize world sharing system
        /// </summary>
        private void InitializeWorldSharingSystem()
        {
            sharedWorlds = new Dictionary<string, WorldMetadata>();
            downloadedWorlds = new Dictionary<string, WorldMetadata>();
            favoriteWorlds = new List<WorldMetadata>();
            myWorlds = new List<WorldMetadata>();
            
            // Initialize core components
            worldUploader = gameObject.AddComponent<WorldUploader>();
            worldDownloader = gameObject.AddComponent<WorldDownloader>();
            worldConverter = gameObject.AddComponent<WorldConverter>();
            worldValidator = gameObject.AddComponent<WorldValidator>();
            worldModerator = gameObject.AddComponent<WorldModerator>();
            worldSearchEngine = gameObject.AddComponent<WorldSearchEngine>();
            
            // Initialize local storage
            localWorldStorage = gameObject.AddComponent<LocalWorldStorage>();
            worldCache = gameObject.AddComponent<WorldCache>();
            userPreferences = gameObject.AddComponent<UserPreferences>();
            
            Debug.Log("World Sharing System initialized!");
        }
        
        /// <summary>
        /// Setup world sharing
        /// </summary>
        private void SetupWorldSharing()
        {
            // Initialize world uploader
            worldUploader.Initialize(maxWorldSizeMB, enableCompression, enableEncryption);
            
            // Initialize world downloader
            worldDownloader.Initialize(enableCompression, enableEncryption);
            
            // Initialize world converter
            if (enableAutoFormatConversion)
            {
                worldConverter.Initialize(platformIdentifier);
            }
            
            // Initialize world validator
            if (enableWorldValidation)
            {
                worldValidator.Initialize();
            }
            
            // Initialize world moderator
            if (enableWorldModeration)
            {
                worldModerator.Initialize();
            }
            
            // Initialize world search engine
            if (enableWorldSearch)
            {
                worldSearchEngine.Initialize();
            }
            
            Debug.Log("World sharing setup complete");
        }
        
        /// <summary>
        /// Load user data
        /// </summary>
        private void LoadUserData()
        {
            // Load user preferences
            userPreferences.LoadPreferences();
            
            // Load local worlds
            localWorldStorage.LoadLocalWorlds();
            
            // Load cached worlds
            worldCache.LoadCachedWorlds();
            
            Debug.Log("User data loaded");
        }
        
        /// <summary>
        /// Initialize platform handlers
        /// </summary>
        private void InitializePlatformHandlers()
        {
            if (enableSteamWorkshop)
            {
                steamWorkshopHandler = gameObject.AddComponent<SteamWorkshopHandler>();
                steamWorkshopHandler.Initialize();
            }
            
            if (enablePlayStationSharing)
            {
                playstationSharingHandler = gameObject.AddComponent<PlaystationSharingHandler>();
                playstationSharingHandler.Initialize();
            }
            
            if (enableNintendoSharing)
            {
                nintendoSharingHandler = gameObject.AddComponent<NintendoSharingHandler>();
                nintendoSharingHandler.Initialize();
            }
            
            if (enableXboxSharing)
            {
                xboxSharingHandler = gameObject.AddComponent<XboxSharingHandler>();
                xboxSharingHandler.Initialize();
            }
            
            Debug.Log("Platform handlers initialized");
        }
        
        /// <summary>
        /// Upload world
        /// </summary>
        public async Task<bool> UploadWorld(string worldName, WorldData worldData, WorldSharingOptions options)
        {
            if (!enableWorldSharing)
            {
                Debug.LogWarning("World sharing not enabled");
                return false;
            }
            
            try
            {
                // Validate world
                if (enableWorldValidation && !worldValidator.ValidateWorld(worldData))
                {
                    Debug.LogError("World validation failed");
                    return false;
                }
                
                // Create world metadata
                WorldMetadata metadata = CreateWorldMetadata(worldName, worldData, options);
                
                // Convert world format if needed
                if (enableAutoFormatConversion)
                {
                    worldData = worldConverter.ConvertWorldFormat(worldData, platformIdentifier);
                }
                
                // Upload to appropriate platform
                bool uploadSuccess = false;
                
                switch (options.targetPlatform)
                {
                    case SharingPlatform.SteamWorkshop:
                        if (steamWorkshopHandler != null)
                        {
                            uploadSuccess = await steamWorkshopHandler.UploadWorld(metadata, worldData);
                        }
                        break;
                    case SharingPlatform.PlayStation:
                        if (playstationSharingHandler != null)
                        {
                            uploadSuccess = await playstationSharingHandler.UploadWorld(metadata, worldData);
                        }
                        break;
                    case SharingPlatform.Nintendo:
                        if (nintendoSharingHandler != null)
                        {
                            uploadSuccess = await nintendoSharingHandler.UploadWorld(metadata, worldData);
                        }
                        break;
                    case SharingPlatform.Xbox:
                        if (xboxSharingHandler != null)
                        {
                            uploadSuccess = await xboxSharingHandler.UploadWorld(metadata, worldData);
                        }
                        break;
                    case SharingPlatform.CrossPlatform:
                        uploadSuccess = await UploadToAllPlatforms(metadata, worldData);
                        break;
                }
                
                if (uploadSuccess)
                {
                    // Store locally
                    localWorldStorage.SaveWorld(metadata, worldData);
                    
                    // Add to my worlds
                    myWorlds.Add(metadata);
                    
                    OnWorldUploaded?.Invoke(worldName);
                    Debug.Log($"World uploaded successfully: {worldName}");
                }
                
                return uploadSuccess;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error uploading world {worldName}: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Download world
        /// </summary>
        public async Task<WorldData> DownloadWorld(string worldId, SharingPlatform platform)
        {
            if (!enableWorldSharing)
            {
                Debug.LogWarning("World sharing not enabled");
                return null;
            }
            
            try
            {
                // Check cache first
                WorldData cachedWorld = worldCache.GetCachedWorld(worldId);
                if (cachedWorld != null)
                {
                    Debug.Log($"World loaded from cache: {worldId}");
                    return cachedWorld;
                }
                
                // Download from platform
                WorldData worldData = null;
                WorldMetadata metadata = null;
                
                switch (platform)
                {
                    case SharingPlatform.SteamWorkshop:
                        if (steamWorkshopHandler != null)
                        {
                            (metadata, worldData) = await steamWorkshopHandler.DownloadWorld(worldId);
                        }
                        break;
                    case SharingPlatform.PlayStation:
                        if (playstationSharingHandler != null)
                        {
                            (metadata, worldData) = await playstationSharingHandler.DownloadWorld(worldId);
                        }
                        break;
                    case SharingPlatform.Nintendo:
                        if (nintendoSharingHandler != null)
                        {
                            (metadata, worldData) = await nintendoSharingHandler.DownloadWorld(worldId);
                        }
                        break;
                    case SharingPlatform.Xbox:
                        if (xboxSharingHandler != null)
                        {
                            (metadata, worldData) = await xboxSharingHandler.DownloadWorld(worldId);
                        }
                        break;
                }
                
                if (worldData != null && metadata != null)
                {
                    // Convert world format if needed
                    if (enableAutoFormatConversion)
                    {
                        worldData = worldConverter.ConvertWorldFormat(worldData, platformIdentifier);
                    }
                    
                    // Cache world
                    worldCache.CacheWorld(worldId, worldData, metadata);
                    
                    // Store locally
                    localWorldStorage.SaveWorld(metadata, worldData);
                    
                    // Add to downloaded worlds
                    downloadedWorlds[worldId] = metadata;
                    
                    OnWorldDownloaded?.Invoke(metadata.name);
                    Debug.Log($"World downloaded successfully: {metadata.name}");
                }
                
                return worldData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error downloading world {worldId}: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Delete world
        /// </summary>
        public async Task<bool> DeleteWorld(string worldId, SharingPlatform platform)
        {
            try
            {
                bool deleteSuccess = false;
                
                switch (platform)
                {
                    case SharingPlatform.SteamWorkshop:
                        if (steamWorkshopHandler != null)
                        {
                            deleteSuccess = await steamWorkshopHandler.DeleteWorld(worldId);
                        }
                        break;
                    case SharingPlatform.PlayStation:
                        if (playstationSharingHandler != null)
                        {
                            deleteSuccess = await playstationSharingHandler.DeleteWorld(worldId);
                        }
                        break;
                    case SharingPlatform.Nintendo:
                        if (nintendoSharingHandler != null)
                        {
                            deleteSuccess = await nintendoSharingHandler.DeleteWorld(worldId);
                        }
                        break;
                    case SharingPlatform.Xbox:
                        if (xboxSharingHandler != null)
                        {
                            deleteSuccess = await xboxSharingHandler.DeleteWorld(worldId);
                        }
                        break;
                }
                
                if (deleteSuccess)
                {
                    // Remove from local storage
                    localWorldStorage.DeleteWorld(worldId);
                    
                    // Remove from cache
                    worldCache.RemoveCachedWorld(worldId);
                    
                    // Remove from my worlds
                    myWorlds.RemoveAll(w => w.id == worldId);
                    
                    OnWorldDeleted?.Invoke(worldId);
                    Debug.Log($"World deleted successfully: {worldId}");
                }
                
                return deleteSuccess;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error deleting world {worldId}: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Rate world
        /// </summary>
        public async Task<bool> RateWorld(string worldId, int rating, SharingPlatform platform)
        {
            try
            {
                bool rateSuccess = false;
                
                switch (platform)
                {
                    case SharingPlatform.SteamWorkshop:
                        if (steamWorkshopHandler != null)
                        {
                            rateSuccess = await steamWorkshopHandler.RateWorld(worldId, rating);
                        }
                        break;
                    case SharingPlatform.PlayStation:
                        if (playstationSharingHandler != null)
                        {
                            rateSuccess = await playstationSharingHandler.RateWorld(worldId, rating);
                        }
                        break;
                    case SharingPlatform.Nintendo:
                        if (nintendoSharingHandler != null)
                        {
                            rateSuccess = await nintendoSharingHandler.RateWorld(worldId, rating);
                        }
                        break;
                    case SharingPlatform.Xbox:
                        if (xboxSharingHandler != null)
                        {
                            rateSuccess = await xboxSharingHandler.RateWorld(worldId, rating);
                        }
                        break;
                }
                
                if (rateSuccess)
                {
                    OnWorldRated?.Invoke(worldId);
                    Debug.Log($"World rated successfully: {worldId}");
                }
                
                return rateSuccess;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error rating world {worldId}: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Favorite world
        /// </summary>
        public void FavoriteWorld(string worldId)
        {
            if (downloadedWorlds.ContainsKey(worldId))
            {
                WorldMetadata world = downloadedWorlds[worldId];
                
                if (!favoriteWorlds.Contains(world))
                {
                    favoriteWorlds.Add(world);
                    
                    OnWorldFavorited?.Invoke(world.name);
                    Debug.Log($"World favorited: {world.name}");
                }
            }
        }
        
        /// <summary>
        /// Unfavorite world
        /// </summary>
        public void UnfavoriteWorld(string worldId)
        {
            if (downloadedWorlds.ContainsKey(worldId))
            {
                WorldMetadata world = downloadedWorlds[worldId];
                
                if (favoriteWorlds.Contains(world))
                {
                    favoriteWorlds.Remove(world);
                    
                    Debug.Log($"World unfavorited: {world.name}");
                }
            }
        }
        
        /// <summary>
        /// Search worlds
        /// </summary>
        public async Task<List<WorldMetadata>> SearchWorlds(string query, WorldSearchOptions options)
        {
            if (!enableWorldSearch)
            {
                Debug.LogWarning("World search not enabled");
                return new List<WorldMetadata>();
            }
            
            try
            {
                List<WorldMetadata> results = await worldSearchEngine.SearchWorlds(query, options);
                
                Debug.Log($"Found {results.Count} worlds for query: {query}");
                return results;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error searching worlds: {e.Message}");
                return new List<WorldMetadata>();
            }
        }
        
        /// <summary>
        /// Get popular worlds
        /// </summary>
        public async Task<List<WorldMetadata>> GetPopularWorlds(int count = 20)
        {
            try
            {
                List<WorldMetadata> popularWorlds = new List<WorldMetadata>();
                
                // Get popular worlds from all platforms
                if (steamWorkshopHandler != null)
                {
                    var steamWorlds = await steamWorkshopHandler.GetPopularWorlds(count / 4);
                    popularWorlds.AddRange(steamWorlds);
                }
                
                if (playstationSharingHandler != null)
                {
                    var psWorlds = await playstationSharingHandler.GetPopularWorlds(count / 4);
                    popularWorlds.AddRange(psWorlds);
                }
                
                // Sort by popularity
                popularWorlds.Sort((a, b) => b.downloadCount.CompareTo(a.downloadCount));
                
                return popularWorlds.Take(count).ToList();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error getting popular worlds: {e.Message}");
                return new List<WorldMetadata>();
            }
        }
        
        /// <summary>
        /// Get my worlds
        /// </summary>
        public List<WorldMetadata> GetMyWorlds()
        {
            return myWorlds;
        }
        
        /// <summary>
        /// Get favorite worlds
        /// </summary>
        public List<WorldMetadata> GetFavoriteWorlds()
        {
            return favoriteWorlds;
        }
        
        /// <summary>
        /// Get downloaded worlds
        /// </summary>
        public List<WorldMetadata> GetDownloadedWorlds()
        {
            return downloadedWorlds.Values.ToList();
        }
        
        /// <summary>
        /// Create world metadata
        /// </summary>
        private WorldMetadata CreateWorldMetadata(string worldName, WorldData worldData, WorldSharingOptions options)
        {
            return new WorldMetadata
            {
                id = GenerateWorldId(worldName),
                name = worldName,
                description = options.description,
                author = GetCurrentUser(),
                version = "1.0.0",
                platform = platformIdentifier,
                category = options.category,
                tags = options.tags,
                size = CalculateWorldSize(worldData),
                downloadCount = 0,
                rating = 0f,
                ratingCount = 0,
                createdDate = System.DateTime.Now,
                modifiedDate = System.DateTime.Now,
                isPublic = options.isPublic,
                isModerated = false,
                thumbnail = options.thumbnail
            };
        }
        
        /// <summary>
        /// Upload to all platforms
        /// </summary>
        private async Task<bool> UploadToAllPlatforms(WorldMetadata metadata, WorldData worldData)
        {
            bool allSuccess = true;
            
            if (steamWorkshopHandler != null)
            {
                bool steamSuccess = await steamWorkshopHandler.UploadWorld(metadata, worldData);
                if (!steamSuccess) allSuccess = false;
            }
            
            if (playstationSharingHandler != null)
            {
                bool psSuccess = await playstationSharingHandler.UploadWorld(metadata, worldData);
                if (!psSuccess) allSuccess = false;
            }
            
            if (nintendoSharingHandler != null)
            {
                bool nintendoSuccess = await nintendoSharingHandler.UploadWorld(metadata, worldData);
                if (!nintendoSuccess) allSuccess = false;
            }
            
            if (xboxSharingHandler != null)
            {
                bool xboxSuccess = await xboxSharingHandler.UploadWorld(metadata, worldData);
                if (!xboxSuccess) allSuccess = false;
            }
            
            return allSuccess;
        }
        
        /// <summary>
        /// Generate unique world ID
        /// </summary>
        private string GenerateWorldId(string worldName)
        {
            string baseId = worldName.ToLower().Replace(" ", "_").Replace("-", "_");
            string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"{baseId}_{timestamp}";
        }
        
        /// <summary>
        /// Get current user
        /// </summary>
        private string GetCurrentUser()
        {
            return System.Environment.UserName ?? "Anonymous";
        }
        
        /// <summary>
        /// Calculate world size
        /// </summary>
        private int CalculateWorldSize(WorldData worldData)
        {
            // This would calculate the actual size of the world data
            return 0;
        }
    }
    
    /// <summary>
    /// World metadata structure
    /// </summary>
    [System.Serializable]
    public class WorldMetadata
    {
        public string id;
        public string name;
        public string description;
        public string author;
        public string version;
        public string platform;
        public string category;
        public List<string> tags;
        public int size;
        public int downloadCount;
        public float rating;
        public int ratingCount;
        public System.DateTime createdDate;
        public System.DateTime modifiedDate;
        public bool isPublic;
        public bool isModerated;
        public string thumbnail;
    }
    
    /// <summary>
    /// World sharing options
    /// </summary>
    [System.Serializable]
    public class WorldSharingOptions
    {
        public string description;
        public string category;
        public List<string> tags;
        public bool isPublic;
        public string thumbnail;
        public SharingPlatform targetPlatform;
    }
    
    /// <summary>
    /// Sharing platforms
    /// </summary>
    public enum SharingPlatform
    {
        SteamWorkshop,
        PlayStation,
        Nintendo,
        Xbox,
        CrossPlatform
    }
    
    /// <summary>
    /// World search options
    /// </summary>
    [System.Serializable]
    public class WorldSearchOptions
    {
        public string category;
        public List<string> tags;
        public SharingPlatform platform;
        public int minRating;
        public int maxResults;
        public SortOrder sortOrder;
    }
    
    /// <summary>
    /// Sort order
    /// </summary>
    public enum SortOrder
    {
        Popularity,
        Rating,
        Newest,
        Oldest,
        Size
    }
}
