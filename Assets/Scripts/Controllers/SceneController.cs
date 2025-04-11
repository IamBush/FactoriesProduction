namespace Controllers
{
    using Data;
    using Managers;
    using Objects;
    using UnityEngine;
    using UnityEngine.Serialization;
    using VContainer;
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private ResourceDatabase resourceDatabase;
        [SerializeField] private AdvancedButton _soundButton;
        [SerializeField] private AdvancedButton _musicButton;
        [SerializeField] private AdvancedButton _runButton;

        private AssetLoadController _assetLoadController;
        private SoundManager _soundManager;
        private PlayerMovementController _playerMovementController;

        [Inject]
        public void Construct(AssetLoadController assetLoadController, SoundManager soundManager, PlayerMovementController playerMovementController)
        {
            _assetLoadController = assetLoadController;
            _soundManager = soundManager;
            _playerMovementController = playerMovementController;
            InitializeButtons();
        }
        
        private void Start()
        {
            InitializeScene();
        }
    
        private async void InitializeScene()
        {
            await _soundButton.SetAction(_soundManager.ToggleSfx).SetBackground();
            await _musicButton.SetAction(_soundManager.ToggleMusic).SetBackground();
            _runButton.SetAction(() => _playerMovementController.OnRunButtonSwitch(!_runButton.IsActive));
            
            _soundButton.ToggleState(!_soundManager.IsSfxEnabled);
            _musicButton.ToggleState(!_soundManager.IsMusicEnabled);
            
            uiManager.Initialize(resourceDatabase);
        }
        
        private void InitializeButtons()
        {
            _soundButton.Initialize(_assetLoadController, _soundManager);
            _musicButton.Initialize(_assetLoadController, _soundManager);
        }
    }
}