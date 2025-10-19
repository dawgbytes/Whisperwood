using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Whisperwood
{
    /// <summary>
    /// Whisperwood Scene Manager
    /// Handles world loading, scene transitions, and environmental effects for the Gucci Goblins world
    /// </summary>
    public class WhisperwoodSceneManager : MonoBehaviour
    {
        [Header("Scene Settings")]
        public string mainHubSceneName = "WhisperwoodHub";
        public string battleRoyaleSceneName = "MallOfEternalShopping";
        public string campaignSceneName = "WhisperwoodCampaign";
        public string tutorialSceneName = "WhisperwoodTutorial";
        
        [Header("Loading Settings")]
        public float loadingFadeTime = 2f;
        public GameObject loadingScreenPrefab;
        public AudioClip loadingMusic;
        
        [Header("Environmental Effects")]
        public bool enableDayNightCycle = true;
        public float dayDuration = 300f; // 5 minutes per day
        public Gradient dayNightGradient;
        public Light sunLight;
        public Light moonLight;
        
        [Header("Weather Effects")]
        public bool enableWeatherSystem = true;
        public float weatherChangeInterval = 120f;
        public GameObject[] weatherEffects;
        
        [Header("Fashion Week Events")]
        public bool enableFashionWeekEvents = true;
        public float fashionWeekInterval = 600f; // 10 minutes
        public GameObject[] fashionWeekDecorations;
        
        // Scene state
        private string currentSceneName;
        private bool isLoading = false;
        private GameObject currentLoadingScreen;
        
        // Environmental state
        private float currentTimeOfDay = 0f;
        private bool isDayTime = true;
        private WeatherType currentWeather = WeatherType.Clear;
        private bool isFashionWeek = false;
        
        // Events
        public System.Action<string> OnSceneLoaded;
        public System.Action<string> OnSceneUnloaded;
        public System.Action<float> OnTimeOfDayChanged;
        public System.Action<WeatherType> OnWeatherChanged;
        public System.Action<bool> OnFashionWeekChanged;
        
        // Singleton
        public static WhisperwoodSceneManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSceneManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            SetupEnvironmentalEffects();
            StartCoroutine(UpdateEnvironmentalEffects());
        }
        
        private void Update()
        {
            UpdateInput();
        }
        
        /// <summary>
        /// Initialize scene manager
        /// </summary>
        private void InitializeSceneManager()
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            
            Debug.Log($"Whisperwood Scene Manager initialized! Current scene: {currentSceneName}");
        }
        
        /// <summary>
        /// Setup environmental effects
        /// </summary>
        private void SetupEnvironmentalEffects()
        {
            // Setup lighting
            if (sunLight == null)
            {
                GameObject sunObject = new GameObject("Sun");
                sunObject.transform.SetParent(transform);
                sunLight = sunObject.AddComponent<Light>();
                sunLight.type = LightType.Directional;
                sunLight.color = Color.white;
                sunLight.intensity = 1f;
            }
            
            if (moonLight == null)
            {
                GameObject moonObject = new GameObject("Moon");
                moonObject.transform.SetParent(transform);
                moonLight = moonObject.AddComponent<Light>();
                moonLight.type = LightType.Directional;
                moonLight.color = Color.blue;
                moonLight.intensity = 0.3f;
                moonLight.enabled = false;
            }
            
            // Setup weather effects
            if (weatherEffects.Length == 0)
            {
                weatherEffects = new GameObject[3];
                
                // Create rain effect
                GameObject rain = new GameObject("Rain");
                rain.transform.SetParent(transform);
                rain.SetActive(false);
                weatherEffects[0] = rain;
                
                // Create snow effect
                GameObject snow = new GameObject("Snow");
                snow.transform.SetParent(transform);
                snow.SetActive(false);
                weatherEffects[1] = snow;
                
                // Create fog effect
                GameObject fog = new GameObject("Fog");
                fog.transform.SetParent(transform);
                fog.SetActive(false);
                weatherEffects[2] = fog;
            }
        }
        
        /// <summary>
        /// Update input handling
        /// </summary>
        private void UpdateInput()
        {
            // Handle scene switching (for testing)
            if (Input.GetKeyDown(KeyCode.F1))
            {
                LoadMainHub();
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                LoadBattleRoyale();
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                LoadCampaign();
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                LoadTutorial();
            }
        }
        
        /// <summary>
        /// Load main hub scene
        /// </summary>
        public void LoadMainHub()
        {
            LoadScene(mainHubSceneName);
        }
        
        /// <summary>
        /// Load battle royale scene
        /// </summary>
        public void LoadBattleRoyale()
        {
            LoadScene(battleRoyaleSceneName);
        }
        
        /// <summary>
        /// Load campaign scene
        /// </summary>
        public void LoadCampaign()
        {
            LoadScene(campaignSceneName);
        }
        
        /// <summary>
        /// Load tutorial scene
        /// </summary>
        public void LoadTutorial()
        {
            LoadScene(tutorialSceneName);
        }
        
        /// <summary>
        /// Load scene with loading screen
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (isLoading) return;
            
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
        
        /// <summary>
        /// Load scene coroutine
        /// </summary>
        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            isLoading = true;
            
            // Show loading screen
            ShowLoadingScreen();
            
            // Fade out
            yield return StartCoroutine(FadeOut());
            
            // Load scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                UpdateLoadingProgress(asyncLoad.progress);
                yield return null;
            }
            
            // Update current scene
            currentSceneName = sceneName;
            
            // Hide loading screen
            HideLoadingScreen();
            
            // Fade in
            yield return StartCoroutine(FadeIn());
            
            isLoading = false;
            
            OnSceneLoaded?.Invoke(sceneName);
            
            Debug.Log($"Scene loaded: {sceneName}");
        }
        
        /// <summary>
        /// Show loading screen
        /// </summary>
        private void ShowLoadingScreen()
        {
            if (loadingScreenPrefab != null)
            {
                currentLoadingScreen = Instantiate(loadingScreenPrefab);
            }
        }
        
        /// <summary>
        /// Hide loading screen
        /// </summary>
        private void HideLoadingScreen()
        {
            if (currentLoadingScreen != null)
            {
                Destroy(currentLoadingScreen);
                currentLoadingScreen = null;
            }
        }
        
        /// <summary>
        /// Update loading progress
        /// </summary>
        private void UpdateLoadingProgress(float progress)
        {
            // Update loading screen progress
            if (currentLoadingScreen != null)
            {
                // This would update loading screen UI
                Debug.Log($"Loading progress: {progress * 100f}%");
            }
        }
        
        /// <summary>
        /// Fade out effect
        /// </summary>
        private IEnumerator FadeOut()
        {
            // This would implement screen fade out
            yield return new WaitForSeconds(loadingFadeTime / 2f);
        }
        
        /// <summary>
        /// Fade in effect
        /// </summary>
        private IEnumerator FadeIn()
        {
            // This would implement screen fade in
            yield return new WaitForSeconds(loadingFadeTime / 2f);
        }
        
        /// <summary>
        /// Update environmental effects
        /// </summary>
        private IEnumerator UpdateEnvironmentalEffects()
        {
            while (true)
            {
                if (enableDayNightCycle)
                {
                    UpdateDayNightCycle();
                }
                
                if (enableWeatherSystem)
                {
                    UpdateWeatherSystem();
                }
                
                if (enableFashionWeekEvents)
                {
                    UpdateFashionWeekEvents();
                }
                
                yield return new WaitForSeconds(1f);
            }
        }
        
        /// <summary>
        /// Update day/night cycle
        /// </summary>
        private void UpdateDayNightCycle()
        {
            currentTimeOfDay += Time.deltaTime / dayDuration;
            
            if (currentTimeOfDay >= 1f)
            {
                currentTimeOfDay = 0f;
            }
            
            // Update lighting based on time of day
            float sunAngle = currentTimeOfDay * 360f;
            sunLight.transform.rotation = Quaternion.Euler(sunAngle - 90f, 0f, 0f);
            
            // Update light intensity and color
            float intensity = Mathf.Lerp(0.1f, 1f, Mathf.Sin(currentTimeOfDay * Mathf.PI));
            sunLight.intensity = intensity;
            
            // Update ambient light
            Color ambientColor = dayNightGradient.Evaluate(currentTimeOfDay);
            RenderSettings.ambientLight = ambientColor;
            
            // Update skybox
            RenderSettings.skybox.SetFloat("_Exposure", intensity);
            
            // Check if it's day or night
            bool wasDayTime = isDayTime;
            isDayTime = currentTimeOfDay < 0.5f;
            
            if (wasDayTime != isDayTime)
            {
                sunLight.enabled = isDayTime;
                moonLight.enabled = !isDayTime;
            }
            
            OnTimeOfDayChanged?.Invoke(currentTimeOfDay);
        }
        
        /// <summary>
        /// Update weather system
        /// </summary>
        private void UpdateWeatherSystem()
        {
            // Randomly change weather
            if (Random.Range(0f, 1f) < Time.deltaTime / weatherChangeInterval)
            {
                WeatherType newWeather = (WeatherType)Random.Range(0, 3);
                SetWeather(newWeather);
            }
        }
        
        /// <summary>
        /// Set weather type
        /// </summary>
        private void SetWeather(WeatherType weather)
        {
            if (currentWeather == weather) return;
            
            // Disable current weather effects
            if (weatherEffects.Length > (int)currentWeather)
            {
                weatherEffects[(int)currentWeather].SetActive(false);
            }
            
            // Enable new weather effects
            if (weatherEffects.Length > (int)weather)
            {
                weatherEffects[(int)weather].SetActive(true);
            }
            
            currentWeather = weather;
            OnWeatherChanged?.Invoke(weather);
            
            Debug.Log($"Weather changed to: {weather}");
        }
        
        /// <summary>
        /// Update fashion week events
        /// </summary>
        private void UpdateFashionWeekEvents()
        {
            // Randomly trigger fashion week events
            if (Random.Range(0f, 1f) < Time.deltaTime / fashionWeekInterval)
            {
                ToggleFashionWeek();
            }
        }
        
        /// <summary>
        /// Toggle fashion week mode
        /// </summary>
        private void ToggleFashionWeek()
        {
            isFashionWeek = !isFashionWeek;
            
            // Enable/disable fashion week decorations
            foreach (GameObject decoration in fashionWeekDecorations)
            {
                if (decoration != null)
                {
                    decoration.SetActive(isFashionWeek);
                }
            }
            
            OnFashionWeekChanged?.Invoke(isFashionWeek);
            
            Debug.Log($"Fashion Week: {(isFashionWeek ? "Active" : "Inactive")}");
        }
        
        /// <summary>
        /// Get current scene name
        /// </summary>
        public string GetCurrentSceneName()
        {
            return currentSceneName;
        }
        
        /// <summary>
        /// Get current time of day
        /// </summary>
        public float GetCurrentTimeOfDay()
        {
            return currentTimeOfDay;
        }
        
        /// <summary>
        /// Get current weather
        /// </summary>
        public WeatherType GetCurrentWeather()
        {
            return currentWeather;
        }
        
        /// <summary>
        /// Check if fashion week is active
        /// </summary>
        public bool IsFashionWeekActive()
        {
            return isFashionWeek;
        }
        
        /// <summary>
        /// Check if it's day time
        /// </summary>
        public bool IsDayTime()
        {
            return isDayTime;
        }
        
        /// <summary>
        /// Set time of day
        /// </summary>
        public void SetTimeOfDay(float timeOfDay)
        {
            currentTimeOfDay = Mathf.Clamp01(timeOfDay);
        }
        
        /// <summary>
        /// Set weather type
        /// </summary>
        public void SetWeatherType(WeatherType weather)
        {
            SetWeather(weather);
        }
        
        /// <summary>
        /// Set fashion week mode
        /// </summary>
        public void SetFashionWeekMode(bool active)
        {
            if (isFashionWeek != active)
            {
                ToggleFashionWeek();
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw scene boundaries
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 100f);
        }
    }
    
    /// <summary>
    /// Weather types
    /// </summary>
    public enum WeatherType
    {
        Clear,
        Rain,
        Snow,
        Fog
    }
}
