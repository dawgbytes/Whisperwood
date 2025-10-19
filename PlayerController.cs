using UnityEngine;

namespace Whisperwood
{
    /// <summary>
    /// Types of fitness equipment weapons in Gucci Goblins
    /// </summary>
    public enum FitnessEquipmentType
    {
        ShakeWeight,    // Shake-Weight of Infinite Gains
        BowFlex,        // Bow-Flex of Unbreakable Will
        ThighMaster,    // Thigh Master of Crushing Victory
        ShakesSpear     // Shakes-Spear of Dramatic Effect
    }
    
    /// <summary>
    /// Player controller for Whisperwood PS5 game
    /// Handles player movement, input, and PS5-specific features
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float sprintSpeed = 8f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;
        public float groundCheckDistance = 0.4f;
        
        [Header("PS5 DualSense Features")]
        public bool enableAdaptiveTriggers = true;
        public bool enableHapticFeedback = true;
        public float hapticIntensity = 1f;
        
        [Header("Fitness Equipment Combat")]
        public FitnessEquipmentType currentEquipment = FitnessEquipmentType.ShakeWeight;
        public float equipmentResistance = 1f;
        public float workoutIntensity = 1f;
        
        [Header("Camera Settings")]
        public Transform cameraTransform;
        public float mouseSensitivity = 2f;
        public float controllerSensitivity = 3f;
        
        // Components
        private CharacterController characterController;
        private Vector3 velocity;
        private bool isGrounded;
        
        // Input
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpInput;
        private bool sprintInput;
        
        // PS5 Specific
        private bool isUsingController = false;
        
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            // Lock cursor to center of screen
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private void Start()
        {
            SetupPS5Features();
        }
        
        private void Update()
        {
            HandleInput();
            HandleMovement();
            HandleCamera();
            CheckGround();
        }
        
        /// <summary>
        /// Setup PS5-specific features
        /// </summary>
        private void SetupPS5Features()
        {
            // Detect if using DualSense controller
            string[] joysticks = Input.GetJoystickNames();
            isUsingController = joysticks.Length > 0 && !string.IsNullOrEmpty(joysticks[0]);
            
            if (isUsingController)
            {
                Debug.Log("DualSense controller detected");
                SetupDualSenseFeatures();
            }
        }
        
        /// <summary>
        /// Setup DualSense controller features
        /// </summary>
        private void SetupDualSenseFeatures()
        {
            if (enableAdaptiveTriggers)
            {
                // Setup adaptive triggers for different actions
                SetupAdaptiveTriggers();
            }
            
            if (enableHapticFeedback)
            {
                // Setup haptic feedback for movement and actions
                SetupHapticFeedback();
            }
        }
        
        /// <summary>
        /// Setup adaptive triggers for different actions
        /// </summary>
        private void SetupAdaptiveTriggers()
        {
            // R2 for shooting/interacting (resistance based on action)
            // L2 for aiming/special abilities (variable resistance)
            Debug.Log("Adaptive triggers configured");
        }
        
        /// <summary>
        /// Setup haptic feedback for different actions
        /// </summary>
        private void SetupHapticFeedback()
        {
            // Footsteps, impacts, environmental interactions
            Debug.Log("Haptic feedback configured");
        }
        
        /// <summary>
        /// Handle player input
        /// </summary>
        private void HandleInput()
        {
            // Movement input
            moveInput = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
            
            // Look input
            if (isUsingController)
            {
                lookInput = new Vector2(
                    Input.GetAxis("Mouse X") * controllerSensitivity,
                    Input.GetAxis("Mouse Y") * controllerSensitivity
                );
            }
            else
            {
                lookInput = new Vector2(
                    Input.GetAxis("Mouse X") * mouseSensitivity,
                    Input.GetAxis("Mouse Y") * mouseSensitivity
                );
            }
            
            // Action inputs
            jumpInput = Input.GetButtonDown("Jump");
            sprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire3");
            
            // PS5 specific inputs
            HandlePS5Inputs();
        }
        
        /// <summary>
        /// Handle PS5-specific inputs
        /// </summary>
        private void HandlePS5Inputs()
        {
            // DualSense specific button mappings
            if (Input.GetButtonDown("Fire1")) // Square button
            {
                HandlePrimaryAction();
            }
            
            if (Input.GetButtonDown("Fire2")) // Triangle button
            {
                HandleSecondaryAction();
            }
            
            if (Input.GetButtonDown("Fire3")) // Circle button
            {
                HandleTertiaryAction();
            }
            
            // Adaptive trigger feedback
            if (enableAdaptiveTriggers)
            {
                HandleAdaptiveTriggers();
            }
        }
        
        /// <summary>
        /// Handle primary action (Square button)
        /// </summary>
        private void HandlePrimaryAction()
        {
            // Implement primary action (interact, attack, etc.)
            TriggerHapticFeedback(0.5f, 0.1f); // Light haptic feedback
        }
        
        /// <summary>
        /// Handle secondary action (Triangle button)
        /// </summary>
        private void HandleSecondaryAction()
        {
            // Implement secondary action
            TriggerHapticFeedback(0.8f, 0.2f); // Strong haptic feedback
        }
        
        /// <summary>
        /// Handle tertiary action (Circle button)
        /// </summary>
        private void HandleTertiaryAction()
        {
            // Implement tertiary action
            TriggerHapticFeedback(0.3f, 0.05f); // Subtle haptic feedback
        }
        
        /// <summary>
        /// Handle adaptive trigger feedback
        /// </summary>
        private void HandleAdaptiveTriggers()
        {
            // R2 trigger (right trigger)
            float rightTrigger = Input.GetAxis("Fire1");
            if (rightTrigger > 0)
            {
                // Apply resistance based on action
                ApplyTriggerResistance(1, rightTrigger * 0.5f); // Right trigger, 50% resistance
            }
            
            // L2 trigger (left trigger)
            float leftTrigger = Input.GetAxis("Fire2");
            if (leftTrigger > 0)
            {
                // Apply resistance based on action
                ApplyTriggerResistance(0, leftTrigger * 0.3f); // Left trigger, 30% resistance
            }
        }
        
        /// <summary>
        /// Apply adaptive trigger resistance based on fitness equipment
        /// </summary>
        private void ApplyTriggerResistance(int triggerIndex, float resistance)
        {
            // Apply resistance based on current fitness equipment
            float equipmentResistance = GetEquipmentResistance();
            float finalResistance = resistance * equipmentResistance * workoutIntensity;
            
            // This would integrate with PlayStation SDK
            Debug.Log($"Trigger {triggerIndex} resistance: {finalResistance} (Equipment: {currentEquipment})");
        }
        
        /// <summary>
        /// Get resistance value based on current fitness equipment
        /// </summary>
        private float GetEquipmentResistance()
        {
            switch (currentEquipment)
            {
                case FitnessEquipmentType.ShakeWeight:
                    return 0.5f; // Light resistance for shaking motion
                case FitnessEquipmentType.BowFlex:
                    return 1.2f; // Progressive resistance building up
                case FitnessEquipmentType.ThighMaster:
                    return 0.8f; // Squeeze resistance
                case FitnessEquipmentType.ShakesSpear:
                    return 0.3f; // Dramatic but light resistance
                default:
                    return 1f;
            }
        }
        
        /// <summary>
        /// Switch to different fitness equipment
        /// </summary>
        public void SwitchEquipment(FitnessEquipmentType newEquipment)
        {
            currentEquipment = newEquipment;
            Debug.Log($"Switched to {newEquipment}");
            
            // Trigger haptic feedback for equipment change
            TriggerHapticFeedback(0.6f, 0.2f);
        }
        
        /// <summary>
        /// Perform fitness equipment attack
        /// </summary>
        public void PerformEquipmentAttack()
        {
            switch (currentEquipment)
            {
                case FitnessEquipmentType.ShakeWeight:
                    PerformShakeWeightAttack();
                    break;
                case FitnessEquipmentType.BowFlex:
                    PerformBowFlexAttack();
                    break;
                case FitnessEquipmentType.ThighMaster:
                    PerformThighMasterAttack();
                    break;
                case FitnessEquipmentType.ShakesSpear:
                    PerformShakesSpearAttack();
                    break;
            }
        }
        
        private void PerformShakeWeightAttack()
        {
            // "Shake and Bake" - Continuous shaking motion
            TriggerHapticFeedback(0.8f, 0.3f);
            Debug.Log("Shake-Weight Attack: Shake and Bake!");
        }
        
        private void PerformBowFlexAttack()
        {
            // "Resistance Training" - Variable damage based on hold time
            TriggerHapticFeedback(1f, 0.5f);
            Debug.Log("Bow-Flex Attack: Resistance Training!");
        }
        
        private void PerformThighMasterAttack()
        {
            // "Feel the Burn" - Damage over time effect
            TriggerHapticFeedback(0.7f, 0.4f);
            Debug.Log("Thigh Master Attack: Feel the Burn!");
        }
        
        private void PerformShakesSpearAttack()
        {
            // "Dramatic Thrust" - Standard melee with dramatic flair
            TriggerHapticFeedback(0.5f, 0.2f);
            Debug.Log("Shakes-Spear Attack: To be, or not to be... DEFEATED!");
        }
        
        /// <summary>
        /// Handle player movement
        /// </summary>
        private void HandleMovement()
        {
            // Calculate movement direction
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            
            // Apply speed
            float currentSpeed = sprintInput ? sprintSpeed : moveSpeed;
            moveDirection *= currentSpeed;
            
            // Apply gravity
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            // Handle jumping
            if (jumpInput && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                TriggerHapticFeedback(0.7f, 0.15f); // Jump haptic feedback
            }
            
            velocity.y += gravity * Time.deltaTime;
            
            // Apply movement
            characterController.Move((moveDirection + velocity * Vector3.up) * Time.deltaTime);
            
            // Footstep haptic feedback
            if (moveInput.magnitude > 0.1f && isGrounded)
            {
                TriggerHapticFeedback(0.2f, 0.05f); // Subtle footstep feedback
            }
        }
        
        /// <summary>
        /// Handle camera movement
        /// </summary>
        private void HandleCamera()
        {
            if (cameraTransform == null) return;
            
            // Rotate player horizontally
            transform.Rotate(Vector3.up * lookInput.x);
            
            // Rotate camera vertically
            Vector3 cameraRotation = cameraTransform.localEulerAngles;
            cameraRotation.x -= lookInput.y;
            cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);
            cameraTransform.localEulerAngles = cameraRotation;
        }
        
        /// <summary>
        /// Check if player is grounded
        /// </summary>
        private void CheckGround()
        {
            isGrounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                groundCheckDistance
            );
        }
        
        /// <summary>
        /// Trigger haptic feedback
        /// </summary>
        private void TriggerHapticFeedback(float intensity, float duration)
        {
            if (!enableHapticFeedback || !isUsingController) return;
            
            // This would integrate with PlayStation SDK
            // For now, we'll log the haptic feedback
            Debug.Log($"Haptic feedback: Intensity={intensity}, Duration={duration}");
        }
        
        /// <summary>
        /// Get current movement state
        /// </summary>
        public bool IsMoving()
        {
            return moveInput.magnitude > 0.1f;
        }
        
        /// <summary>
        /// Get current sprint state
        /// </summary>
        public bool IsSprinting()
        {
            return sprintInput && IsMoving();
        }
        
        /// <summary>
        /// Get grounded state
        /// </summary>
        public bool IsGrounded()
        {
            return isGrounded;
        }
    }
}
