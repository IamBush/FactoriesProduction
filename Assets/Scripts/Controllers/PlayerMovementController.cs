namespace Controllers
{
    using System;
    using UnityEngine;
    using UnityEngine.AI;
    
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        private const float _walkSpeed = 3f;
        private const float _runSpeed = 7f;
        private const float _stoppingDistance = 0.2f;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private KeyCode _runKey = KeyCode.LeftShift;

        [Header("Animation Settings")]
        [SerializeField] private string _isMovingParameter = "IsMoving";
        [SerializeField] private string _isRunningParameter = "IsRunning";
        [SerializeField] private string _velocityParameter = "Velocity";
        private const float _minMoveSpeed = 0.5f;

        const float RaycastDistance = 1000f;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        private Camera _mainCamera;
        private bool _isMoving;
        private bool _isRunning;
    
        public event Action OnDestinationReached;
    
        private void Awake()
        {
            _mainCamera = Camera.main;
            _navMeshAgent.speed = _walkSpeed;
            _navMeshAgent.stoppingDistance = _stoppingDistance;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMovementInput();
            }
    
            HandleRunInput();
            UpdateAnimationState();
        }

        private void HandleMovementInput()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, RaycastDistance, _groundLayer)) return;
        
            MoveToPosition(hit.point);
        }

        private void HandleRunInput()
        {
            bool wasRunning = _isRunning;
            _isRunning = Input.GetKey(_runKey) && _isMoving;
    
            if (_isRunning != wasRunning)
            {
                _navMeshAgent.speed = _isRunning ? _runSpeed : _walkSpeed;
            }
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
            OnDestinationReached?.Invoke();
        }
    }
}