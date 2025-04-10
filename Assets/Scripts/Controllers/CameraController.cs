namespace Controllers
{
    using Unity.Cinemachine;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        [Header("Camera Movement")]
        [SerializeField] private float _moveSpeed = 20f;
        [SerializeField] private Vector2 _horizontalLimits = new Vector2(-10f, 10f);
        [SerializeField] private Vector2 _verticalLimits = new Vector2(-10f, 10f);
    
        [Header("Camera Controls")]
        [SerializeField] private KeyCode _moveUpKey = KeyCode.W;
        [SerializeField] private KeyCode _moveDownKey = KeyCode.S;
        [SerializeField] private KeyCode _moveLeftKey = KeyCode.A;
        [SerializeField] private KeyCode _moveRightKey = KeyCode.D;
    
        [Header("Edge Scrolling")]
        [SerializeField] private bool _useEdgeScrolling = true;
        [SerializeField] private float _edgeScrollThreshold = 40f;
        
        [Header("Zoom Settings")]
        [SerializeField] private float _zoomSpeed = 2f;
        [SerializeField] private float _minOrthographicSize = 3f;
        [SerializeField] private float _maxOrthographicSize = 10f;
        [SerializeField] private float _zoomSmoothness = 10f;
    
        [Header("References")]
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private float _cameraRotationY = 45f;
        [SerializeField] private CinemachineCamera _virtualCamera;
        
        private float _targetOrthographicSize;
    
        private void Awake()
        {
            _targetOrthographicSize = _virtualCamera.Lens.OrthographicSize;
        }
        
        private void Update()
        {
            HandleKeyboardInput();
            
            if (_useEdgeScrolling)
            {
                HandleMouseEdgeScrolling();
            }
            
            HandleZoomInput();
            UpdateZoom();
        }
    
        private void HandleKeyboardInput()
        {
            Vector3 moveDirection = Vector3.zero;
        
            if (Input.GetKey(_moveUpKey))
                moveDirection += Vector3.forward;
            
            if (Input.GetKey(_moveDownKey))
                moveDirection += Vector3.back;
            
            if (Input.GetKey(_moveRightKey))
                moveDirection += Vector3.right;
            
            if (Input.GetKey(_moveLeftKey))
                moveDirection += Vector3.left;
            
            if (moveDirection != Vector3.zero)
            {
                MoveCamera(moveDirection);
            }
        }
    
        private void HandleMouseEdgeScrolling()
        {
            Vector3 moveDirection = Vector3.zero;
        
            Vector3 mousePosition = Input.mousePosition;
        
            if (mousePosition.x < _edgeScrollThreshold)
                moveDirection += Vector3.left;
            
            if (mousePosition.x > Screen.width - _edgeScrollThreshold)
                moveDirection += Vector3.right;
            
            if (mousePosition.y < _edgeScrollThreshold)
                moveDirection += Vector3.back;
            
            if (mousePosition.y > Screen.height - _edgeScrollThreshold)
                moveDirection += Vector3.forward;
            
            if (moveDirection != Vector3.zero)
            {
                MoveCamera(moveDirection);
            }
        }
        
        private void HandleZoomInput()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            
            if (scrollInput != 0 && _virtualCamera != null)
            {
                _targetOrthographicSize -= scrollInput * _zoomSpeed;
                _targetOrthographicSize = Mathf.Clamp(_targetOrthographicSize, _minOrthographicSize, _maxOrthographicSize);
            }
        }
        
        private void UpdateZoom()
        {
            float currentSize = _virtualCamera.Lens.OrthographicSize;
            float newSize = Mathf.Lerp(currentSize, _targetOrthographicSize, Time.deltaTime * _zoomSmoothness);
            _virtualCamera.Lens.OrthographicSize = newSize;
        }
    
        private void MoveCamera(Vector3 direction)
        {
            if (_cameraTarget == null)
                return;
        
            Vector3 rotatedDirection = Quaternion.Euler(0, _cameraRotationY, 0) * direction.normalized;
            Vector3 moveVector = rotatedDirection * _moveSpeed * Time.deltaTime;
            Vector3 newPosition = _cameraTarget.position + moveVector;
        
            newPosition.x = Mathf.Clamp(newPosition.x, _horizontalLimits.x, _horizontalLimits.y);
            newPosition.z = Mathf.Clamp(newPosition.z, _verticalLimits.x, _verticalLimits.y);
        
            _cameraTarget.position = newPosition;
        }
    }
}