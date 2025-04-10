namespace Controllers
{
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
    
        [Header("References")]
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private float _cameraRotationY = 45f;
    
        private void Update()
        {
            HandleKeyboardInput();
            HandleMouseEdgeScrolling();
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