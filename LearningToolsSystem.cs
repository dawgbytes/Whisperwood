using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Whisperwood
{
    /// <summary>
    /// Learning Tools System for Whisperwood
    /// Makes creativity fun and easy to learn with interactive tutorials and educational content
    /// </summary>
    public class LearningToolsSystem : MonoBehaviour
    {
        [Header("Learning System Settings")]
        public bool enableTutorialSystem = true;
        public bool enableInteractiveLessons = true;
        public bool enableCodingTutorials = true;
        public bool enableArtTutorials = true;
        public bool enableGameDesignTutorials = true;
        public bool enableProgressTracking = true;
        public bool enableAchievementSystem = true;
        
        [Header("Difficulty Settings")]
        public bool enableAdaptiveDifficulty = true;
        public bool enableSkillBasedLearning = true;
        public bool enablePersonalizedContent = true;
        public bool enableLearningAnalytics = true;
        
        [Header("Creative Learning")]
        public bool enableVisualProgramming = true;
        public bool enableNodeBasedScripting = true;
        public bool enableDragAndDropLogic = true;
        public bool enableTemplateBasedLearning = true;
        
        [Header("Social Learning")]
        public bool enableCollaborativeLearning = true;
        public bool enablePeerReview = true;
        public bool enableMentorshipProgram = true;
        public bool enableLearningCommunities = true;
        
        // Learning components
        private TutorialManager tutorialManager;
        private InteractiveLessonManager lessonManager;
        private CodingTutorialManager codingManager;
        private ArtTutorialManager artManager;
        private GameDesignTutorialManager gameDesignManager;
        private ProgressTracker progressTracker;
        private AchievementSystem achievementSystem;
        private LearningAnalytics learningAnalytics;
        
        // Creative learning tools
        private VisualProgrammingTool visualProgramming;
        private NodeBasedScriptingTool nodeScripting;
        private DragAndDropLogicTool dragDropLogic;
        private TemplateLearningTool templateLearning;
        
        // Social learning tools
        private CollaborativeLearningTool collaborativeLearning;
        private PeerReviewTool peerReview;
        private MentorshipProgram mentorshipProgram;
        private LearningCommunityManager communityManager;
        
        // Learning state
        private UserLearningProfile userProfile;
        private List<LearningSession> learningSessions;
        private Dictionary<string, LearningProgress> learningProgress;
        private List<Achievement> unlockedAchievements;
        
        // Events
        public System.Action<string> OnTutorialStarted;
        public System.Action<string> OnTutorialCompleted;
        public System.Action<string> OnAchievementUnlocked;
        public System.Action<string> OnSkillLevelUp;
        public System.Action<string> OnLessonCompleted;
        
        private void Awake()
        {
            InitializeLearningToolsSystem();
        }
        
        private void Start()
        {
            SetupLearningEnvironment();
            InitializeLearningTools();
            LoadUserLearningData();
        }
        
        /// <summary>
        /// Initialize learning tools system
        /// </summary>
        public void Initialize(bool tutorialSystem, PlayStationGeneration playStationGeneration)
        {
            enableTutorialSystem = tutorialSystem;
            
            // Adjust features based on PlayStation generation
            AdjustFeaturesForPlayStation(playStationGeneration);
            
            InitializeLearningToolsSystem();
        }
        
        /// <summary>
        /// Adjust features for PlayStation generation
        /// </summary>
        private void AdjustFeaturesForPlayStation(PlayStationGeneration generation)
        {
            switch (generation)
            {
                case PlayStationGeneration.PS1:
                case PlayStationGeneration.PS2:
                    // Limited features for older consoles
                    enableCodingTutorials = false;
                    enableVisualProgramming = false;
                    enableNodeBasedScripting = false;
                    enableDragAndDropLogic = false;
                    enableCollaborativeLearning = false;
                    enableLearningAnalytics = false;
                    break;
                    
                case PlayStationGeneration.PS3:
                    // Basic features
                    enableCodingTutorials = true;
                    enableVisualProgramming = false;
                    enableNodeBasedScripting = false;
                    enableDragAndDropLogic = true;
                    enableCollaborativeLearning = true;
                    enableLearningAnalytics = true;
                    break;
                    
                case PlayStationGeneration.PS4:
                    // Most features
                    enableCodingTutorials = true;
                    enableVisualProgramming = true;
                    enableNodeBasedScripting = false;
                    enableDragAndDropLogic = true;
                    enableCollaborativeLearning = true;
                    enableLearningAnalytics = true;
                    break;
                    
                case PlayStationGeneration.PS5:
                    // All features
                    enableCodingTutorials = true;
                    enableVisualProgramming = true;
                    enableNodeBasedScripting = true;
                    enableDragAndDropLogic = true;
                    enableCollaborativeLearning = true;
                    enableLearningAnalytics = true;
                    break;
            }
        }
        
        /// <summary>
        /// Initialize learning tools system
        /// </summary>
        private void InitializeLearningToolsSystem()
        {
            learningSessions = new List<LearningSession>();
            learningProgress = new Dictionary<string, LearningProgress>();
            unlockedAchievements = new List<Achievement>();
            
            // Initialize user profile
            userProfile = new UserLearningProfile();
            
            Debug.Log("Learning Tools System initialized!");
        }
        
        /// <summary>
        /// Setup learning environment
        /// </summary>
        private void SetupLearningEnvironment()
        {
            // Initialize core learning components
            if (enableTutorialSystem)
            {
                tutorialManager = gameObject.AddComponent<TutorialManager>();
                tutorialManager.Initialize();
            }
            
            if (enableInteractiveLessons)
            {
                lessonManager = gameObject.AddComponent<InteractiveLessonManager>();
                lessonManager.Initialize();
            }
            
            if (enableCodingTutorials)
            {
                codingManager = gameObject.AddComponent<CodingTutorialManager>();
                codingManager.Initialize();
            }
            
            if (enableArtTutorials)
            {
                artManager = gameObject.AddComponent<ArtTutorialManager>();
                artManager.Initialize();
            }
            
            if (enableGameDesignTutorials)
            {
                gameDesignManager = gameObject.AddComponent<GameDesignTutorialManager>();
                gameDesignManager.Initialize();
            }
            
            if (enableProgressTracking)
            {
                progressTracker = gameObject.AddComponent<ProgressTracker>();
                progressTracker.Initialize();
            }
            
            if (enableAchievementSystem)
            {
                achievementSystem = gameObject.AddComponent<AchievementSystem>();
                achievementSystem.Initialize();
            }
            
            if (enableLearningAnalytics)
            {
                learningAnalytics = gameObject.AddComponent<LearningAnalytics>();
                learningAnalytics.Initialize();
            }
            
            Debug.Log("Learning environment setup complete");
        }
        
        /// <summary>
        /// Initialize learning tools
        /// </summary>
        private void InitializeLearningTools()
        {
            // Initialize creative learning tools
            if (enableVisualProgramming)
            {
                visualProgramming = gameObject.AddComponent<VisualProgrammingTool>();
                visualProgramming.Initialize();
            }
            
            if (enableNodeBasedScripting)
            {
                nodeScripting = gameObject.AddComponent<NodeBasedScriptingTool>();
                nodeScripting.Initialize();
            }
            
            if (enableDragAndDropLogic)
            {
                dragDropLogic = gameObject.AddComponent<DragAndDropLogicTool>();
                dragDropLogic.Initialize();
            }
            
            if (enableTemplateBasedLearning)
            {
                templateLearning = gameObject.AddComponent<TemplateLearningTool>();
                templateLearning.Initialize();
            }
            
            // Initialize social learning tools
            if (enableCollaborativeLearning)
            {
                collaborativeLearning = gameObject.AddComponent<CollaborativeLearningTool>();
                collaborativeLearning.Initialize();
            }
            
            if (enablePeerReview)
            {
                peerReview = gameObject.AddComponent<PeerReviewTool>();
                peerReview.Initialize();
            }
            
            if (enableMentorshipProgram)
            {
                mentorshipProgram = gameObject.AddComponent<MentorshipProgram>();
                mentorshipProgram.Initialize();
            }
            
            if (enableLearningCommunities)
            {
                communityManager = gameObject.AddComponent<LearningCommunityManager>();
                communityManager.Initialize();
            }
            
            Debug.Log("Learning tools initialized");
        }
        
        /// <summary>
        /// Load user learning data
        /// </summary>
        private void LoadUserLearningData()
        {
            // Load user profile
            userProfile = LoadUserProfile();
            
            // Load learning progress
            learningProgress = LoadLearningProgress();
            
            // Load achievements
            unlockedAchievements = LoadUnlockedAchievements();
            
            // Load learning sessions
            learningSessions = LoadLearningSessions();
            
            Debug.Log("User learning data loaded");
        }
        
        /// <summary>
        /// Start creative tutorial
        /// </summary>
        public void StartCreativeTutorial()
        {
            if (!enableTutorialSystem)
            {
                Debug.LogWarning("Tutorial system not enabled");
                return;
            }
            
            // Start with basic building tutorial
            StartTutorial("BasicBuilding");
        }
        
        /// <summary>
        /// Start tutorial
        /// </summary>
        public void StartTutorial(string tutorialId)
        {
            if (tutorialManager != null)
            {
                tutorialManager.StartTutorial(tutorialId);
                OnTutorialStarted?.Invoke(tutorialId);
                
                // Track learning session
                StartLearningSession(tutorialId, LearningType.Tutorial);
            }
        }
        
        /// <summary>
        /// Complete tutorial
        /// </summary>
        public void CompleteTutorial(string tutorialId)
        {
            if (tutorialManager != null)
            {
                tutorialManager.CompleteTutorial(tutorialId);
                OnTutorialCompleted?.Invoke(tutorialId);
                
                // Update learning progress
                UpdateLearningProgress(tutorialId, 100f);
                
                // Check for achievements
                CheckTutorialAchievements(tutorialId);
                
                // End learning session
                EndLearningSession(tutorialId);
            }
        }
        
        /// <summary>
        /// Start interactive lesson
        /// </summary>
        public void StartInteractiveLesson(string lessonId)
        {
            if (lessonManager != null)
            {
                lessonManager.StartLesson(lessonId);
                
                // Track learning session
                StartLearningSession(lessonId, LearningType.InteractiveLesson);
            }
        }
        
        /// <summary>
        /// Complete interactive lesson
        /// </summary>
        public void CompleteInteractiveLesson(string lessonId, float score)
        {
            if (lessonManager != null)
            {
                lessonManager.CompleteLesson(lessonId, score);
                OnLessonCompleted?.Invoke(lessonId);
                
                // Update learning progress
                UpdateLearningProgress(lessonId, score);
                
                // Check for achievements
                CheckLessonAchievements(lessonId, score);
                
                // End learning session
                EndLearningSession(lessonId);
            }
        }
        
        /// <summary>
        /// Start coding tutorial
        /// </summary>
        public void StartCodingTutorial(string codingId)
        {
            if (codingManager != null)
            {
                codingManager.StartCodingTutorial(codingId);
                
                // Track learning session
                StartLearningSession(codingId, LearningType.Coding);
            }
        }
        
        /// <summary>
        /// Start art tutorial
        /// </summary>
        public void StartArtTutorial(string artId)
        {
            if (artManager != null)
            {
                artManager.StartArtTutorial(artId);
                
                // Track learning session
                StartLearningSession(artId, LearningType.Art);
            }
        }
        
        /// <summary>
        /// Start game design tutorial
        /// </summary>
        public void StartGameDesignTutorial(string gameDesignId)
        {
            if (gameDesignManager != null)
            {
                gameDesignManager.StartGameDesignTutorial(gameDesignId);
                
                // Track learning session
                StartLearningSession(gameDesignId, LearningType.GameDesign);
            }
        }
        
        /// <summary>
        /// Get available tutorials
        /// </summary>
        public List<TutorialInfo> GetAvailableTutorials()
        {
            List<TutorialInfo> tutorials = new List<TutorialInfo>();
            
            if (tutorialManager != null)
            {
                tutorials.AddRange(tutorialManager.GetAvailableTutorials());
            }
            
            if (lessonManager != null)
            {
                tutorials.AddRange(lessonManager.GetAvailableLessons());
            }
            
            if (codingManager != null)
            {
                tutorials.AddRange(codingManager.GetAvailableCodingTutorials());
            }
            
            if (artManager != null)
            {
                tutorials.AddRange(artManager.GetAvailableArtTutorials());
            }
            
            if (gameDesignManager != null)
            {
                tutorials.AddRange(gameDesignManager.GetAvailableGameDesignTutorials());
            }
            
            return tutorials;
        }
        
        /// <summary>
        /// Get learning progress
        /// </summary>
        public LearningProgress GetLearningProgress(string topicId)
        {
            if (learningProgress.ContainsKey(topicId))
            {
                return learningProgress[topicId];
            }
            
            return new LearningProgress
            {
                topicId = topicId,
                progress = 0f,
                skillLevel = 1,
                experiencePoints = 0,
                lastAccessed = System.DateTime.Now
            };
        }
        
        /// <summary>
        /// Get user learning profile
        /// </summary>
        public UserLearningProfile GetUserLearningProfile()
        {
            return userProfile;
        }
        
        /// <summary>
        /// Get unlocked achievements
        /// </summary>
        public List<Achievement> GetUnlockedAchievements()
        {
            return unlockedAchievements;
        }
        
        /// <summary>
        /// Get learning statistics
        /// </summary>
        public LearningStatistics GetLearningStatistics()
        {
            return new LearningStatistics
            {
                totalTutorialsCompleted = GetTutorialsCompleted(),
                totalLessonsCompleted = GetLessonsCompleted(),
                totalCodingTutorialsCompleted = GetCodingTutorialsCompleted(),
                totalArtTutorialsCompleted = GetArtTutorialsCompleted(),
                totalGameDesignTutorialsCompleted = GetGameDesignTutorialsCompleted(),
                totalTimeSpentLearning = GetTotalTimeSpentLearning(),
                totalAchievementsUnlocked = unlockedAchievements.Count,
                averageScore = GetAverageScore(),
                skillLevel = GetOverallSkillLevel()
            };
        }
        
        /// <summary>
        /// Start learning session
        /// </summary>
        private void StartLearningSession(string topicId, LearningType type)
        {
            LearningSession session = new LearningSession
            {
                id = GenerateSessionId(),
                topicId = topicId,
                type = type,
                startTime = System.DateTime.Now,
                endTime = System.DateTime.Now,
                duration = 0f,
                score = 0f,
                isActive = true
            };
            
            learningSessions.Add(session);
            
            // Track analytics
            if (learningAnalytics != null)
            {
                learningAnalytics.TrackSessionStart(session);
            }
        }
        
        /// <summary>
        /// End learning session
        /// </summary>
        private void EndLearningSession(string topicId)
        {
            LearningSession session = learningSessions.FirstOrDefault(s => s.topicId == topicId && s.isActive);
            
            if (session != null)
            {
                session.isActive = false;
                session.endTime = System.DateTime.Now;
                session.duration = (float)(session.endTime - session.startTime).TotalMinutes;
                
                // Track analytics
                if (learningAnalytics != null)
                {
                    learningAnalytics.TrackSessionEnd(session);
                }
            }
        }
        
        /// <summary>
        /// Update learning progress
        /// </summary>
        private void UpdateLearningProgress(string topicId, float progress)
        {
            if (!learningProgress.ContainsKey(topicId))
            {
                learningProgress[topicId] = new LearningProgress
                {
                    topicId = topicId,
                    progress = 0f,
                    skillLevel = 1,
                    experiencePoints = 0,
                    lastAccessed = System.DateTime.Now
                };
            }
            
            LearningProgress learningProgressData = learningProgress[topicId];
            learningProgressData.progress = Mathf.Max(learningProgressData.progress, progress);
            learningProgressData.lastAccessed = System.DateTime.Now;
            
            // Add experience points
            int experienceGained = Mathf.RoundToInt(progress * 10f);
            learningProgressData.experiencePoints += experienceGained;
            
            // Check for skill level up
            int newSkillLevel = CalculateSkillLevel(learningProgressData.experiencePoints);
            if (newSkillLevel > learningProgressData.skillLevel)
            {
                learningProgressData.skillLevel = newSkillLevel;
                OnSkillLevelUp?.Invoke(topicId);
            }
            
            // Update progress tracker
            if (progressTracker != null)
            {
                progressTracker.UpdateProgress(topicId, learningProgressData);
            }
        }
        
        /// <summary>
        /// Check tutorial achievements
        /// </summary>
        private void CheckTutorialAchievements(string tutorialId)
        {
            if (achievementSystem != null)
            {
                List<Achievement> newAchievements = achievementSystem.CheckTutorialAchievements(tutorialId);
                
                foreach (Achievement achievement in newAchievements)
                {
                    if (!unlockedAchievements.Contains(achievement))
                    {
                        unlockedAchievements.Add(achievement);
                        OnAchievementUnlocked?.Invoke(achievement.name);
                    }
                }
            }
        }
        
        /// <summary>
        /// Check lesson achievements
        /// </summary>
        private void CheckLessonAchievements(string lessonId, float score)
        {
            if (achievementSystem != null)
            {
                List<Achievement> newAchievements = achievementSystem.CheckLessonAchievements(lessonId, score);
                
                foreach (Achievement achievement in newAchievements)
                {
                    if (!unlockedAchievements.Contains(achievement))
                    {
                        unlockedAchievements.Add(achievement);
                        OnAchievementUnlocked?.Invoke(achievement.name);
                    }
                }
            }
        }
        
        /// <summary>
        /// Calculate skill level
        /// </summary>
        private int CalculateSkillLevel(int experiencePoints)
        {
            // Simple skill level calculation
            return Mathf.FloorToInt(experiencePoints / 100f) + 1;
        }
        
        /// <summary>
        /// Generate session ID
        /// </summary>
        private string GenerateSessionId()
        {
            return System.Guid.NewGuid().ToString();
        }
        
        // Helper methods for statistics
        private int GetTutorialsCompleted()
        {
            return learningSessions.Count(s => s.type == LearningType.Tutorial && !s.isActive);
        }
        
        private int GetLessonsCompleted()
        {
            return learningSessions.Count(s => s.type == LearningType.InteractiveLesson && !s.isActive);
        }
        
        private int GetCodingTutorialsCompleted()
        {
            return learningSessions.Count(s => s.type == LearningType.Coding && !s.isActive);
        }
        
        private int GetArtTutorialsCompleted()
        {
            return learningSessions.Count(s => s.type == LearningType.Art && !s.isActive);
        }
        
        private int GetGameDesignTutorialsCompleted()
        {
            return learningSessions.Count(s => s.type == LearningType.GameDesign && !s.isActive);
        }
        
        private float GetTotalTimeSpentLearning()
        {
            return learningSessions.Where(s => !s.isActive).Sum(s => s.duration);
        }
        
        private float GetAverageScore()
        {
            var completedSessions = learningSessions.Where(s => !s.isActive && s.score > 0);
            return completedSessions.Any() ? completedSessions.Average(s => s.score) : 0f;
        }
        
        private int GetOverallSkillLevel()
        {
            return learningProgress.Values.Any() ? 
                Mathf.RoundToInt(learningProgress.Values.Average(p => p.skillLevel)) : 1;
        }
        
        // Placeholder methods for data loading/saving
        private UserLearningProfile LoadUserProfile()
        {
            // This would load user profile from persistent storage
            return new UserLearningProfile();
        }
        
        private Dictionary<string, LearningProgress> LoadLearningProgress()
        {
            // This would load learning progress from persistent storage
            return new Dictionary<string, LearningProgress>();
        }
        
        private List<Achievement> LoadUnlockedAchievements()
        {
            // This would load unlocked achievements from persistent storage
            return new List<Achievement>();
        }
        
        private List<LearningSession> LoadLearningSessions()
        {
            // This would load learning sessions from persistent storage
            return new List<LearningSession>();
        }
    }
    
    /// <summary>
    /// User learning profile
    /// </summary>
    [System.Serializable]
    public class UserLearningProfile
    {
        public string userId;
        public string userName;
        public int overallSkillLevel;
        public int totalExperiencePoints;
        public List<string> preferredLearningStyles;
        public List<string> interests;
        public System.DateTime profileCreated;
        public System.DateTime lastActive;
    }
    
    /// <summary>
    /// Learning session
    /// </summary>
    [System.Serializable]
    public class LearningSession
    {
        public string id;
        public string topicId;
        public LearningType type;
        public System.DateTime startTime;
        public System.DateTime endTime;
        public float duration;
        public float score;
        public bool isActive;
    }
    
    /// <summary>
    /// Learning progress
    /// </summary>
    [System.Serializable]
    public class LearningProgress
    {
        public string topicId;
        public float progress;
        public int skillLevel;
        public int experiencePoints;
        public System.DateTime lastAccessed;
    }
    
    /// <summary>
    /// Achievement
    /// </summary>
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        public string description;
        public string icon;
        public int experienceReward;
        public System.DateTime unlockedDate;
    }
    
    /// <summary>
    /// Learning statistics
    /// </summary>
    [System.Serializable]
    public class LearningStatistics
    {
        public int totalTutorialsCompleted;
        public int totalLessonsCompleted;
        public int totalCodingTutorialsCompleted;
        public int totalArtTutorialsCompleted;
        public int totalGameDesignTutorialsCompleted;
        public float totalTimeSpentLearning;
        public int totalAchievementsUnlocked;
        public float averageScore;
        public int skillLevel;
    }
    
    /// <summary>
    /// Learning types
    /// </summary>
    public enum LearningType
    {
        Tutorial,
        InteractiveLesson,
        Coding,
        Art,
        GameDesign
    }
}
