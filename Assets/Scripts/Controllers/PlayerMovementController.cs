namespace Controllers
{
    using System;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.EventSystems;
    using VContainer;

    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        private const float _walkSpeed = 3f;
        private const float _runSpeed = 7f;
        private const float _stoppingDistance = 0.2f;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _factoryLayer;
        [SerializeField] private KeyCode _runKey = KeyCode.LeftShift;

        [Header("Animation Settings")]
        [SerializeField] private string _isMovingParameter = "IsMoving";
        [SerializeField] private string _isRunningParameter = "IsRunning";
        [SerializeField] private string _velocityParameter = "Velocity";
        private const float _minMoveSpeed = 0.5f;

        [Header("Sound Settings")]
        [SerializeField] private float _footstepInterval = 0.5f;
        private float _lastFootstepTime;

        const float RaycastDistance = 1000f;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        private Camera _mainCamera;
        private bool _isMoving;
        private bool _isRunning;
        private bool _isRunButtonPressed;
        private SoundManager _soundManager;
        public event Action OnDestinationReached;

        [Inject]
        public void Construct(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            _navMeshAgent.speed = _walkSpeed;
            _navMeshAgent.stoppingDistance = _stoppingDistance;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                HandleMovementInput(Input.mousePosition);
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                HandleMovementInput(Input.GetTouch(0).position);
            }

            HandleRunInput();
            UpdateAnimationState();
            HandleMovementSounds();
        }

        private void HandleMovementInput(Vector2 screenPosition)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
            
            if (Physics.Raycast(ray, out RaycastHit factoryHit, RaycastDistance, _factoryLayer))
            {
                return;
            }

            if (!Physics.Raycast(ray, out RaycastHit hit, RaycastDistance, _groundLayer)) return;

            MoveToPosition(hit.point);
        }

        private void HandleRunInput()
        {
            bool wasRunning = _isRunning;
            _isRunning = (_isRunButtonPressed || Input.GetKey(_runKey)) && _isMoving;

            if (_isRunning != wasRunning)
            {
                _navMeshAgent.speed = _isRunning ? _runSpeed : _walkSpeed;
            }
        }
        
        public void OnRunButtonSwitch(bool isActive)
        {
            _isRunButtonPressed = isActive;
        }

        private void UpdateAnimationState()
        {
            if (_animator == null)
                return;

            bool hasReachedDestination =
                !_navMeshAgent.pathPending &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;

            if (hasReachedDestination)
            {
                StopMovement();
                return;
            }

            bool isCurrentlyMoving = _navMeshAgent.velocity.magnitude > _minMoveSpeed;
            UpdateAnimatorParameters(isCurrentlyMoving);
        }

        private void UpdateAnimatorParameters(bool isCurrentlyMoving)
        {
            _animator.SetBool(_isMovingParameter, isCurrentlyMoving);
            _animator.SetBool(_isRunningParameter, _isRunning && isCurrentlyMoving);
            float normalizedSpeed = _navMeshAgent.velocity.magnitude / (_isRunning ? _runSpeed : _walkSpeed);
            _animator.SetFloat(_velocityParameter, normalizedSpeed);
        }

        private void HandleMovementSounds()
        {
            if (_soundManager == null || !_isMoving)
                return;

            bool isCurrentlyMoving = _navMeshAgent.velocity.magnitude > _minMoveSpeed;
            if (!isCurrentlyMoving)
                return;

            float currentInterval = _footstepInterval;
            if (_isRunning)
                currentInterval *= 0.6f;

            if (!(Time.time - _lastFootstepTime >= currentInterval)) return;
            
            _lastFootstepTime = Time.time;

            _soundManager.PlayStepEffect();
        }

        public void MoveToPosition(Vector3 position)
        {
            _navMeshAgent.SetDestination(position);
            _isMoving = true;
        }

        public void StopMovement()
        {
            _animator.SetBool(_isMovingParameter, false);
            _animator.SetBool(_isRunningParameter, false);
            _animator.SetFloat(_velocityParameter, 0);
            _isMoving = false;
            _isRunning = false;
            _navMeshAgent.ResetPath();
            _navMeshAgent.speed = _walkSpeed;
            OnDestinationReached?.Invoke();
        }
    }
}