namespace Objects
{
    using Controllers;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using Enums;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using VContainer;

    public class AdvancedButton : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] protected Image _buttonImage;
        [SerializeField] private RectTransform _buttonRect;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] protected TextMeshProUGUI _buttonText;
        [SerializeField] private AudioClip _clickSound;

        [EnumToggleButtons]
        public ButtonType ButtonType;

        [ShowIf("ButtonType", ButtonType.Switch)]
        [EnumToggleButtons]
        public ButtonActionType ActionType;
       
        [ShowIf("IsPush")]
        [SerializeField] private AssetReference _spriteReference;

        [ShowIf("IsActivateObjectAction")]
        [SerializeField] private Image _activeObject;
        [ShowIf("IsActivateObjectAction")]
        [SerializeField] private AssetReference _activeObjectSpriteReference;

        [ShowIf("IsChangeSpriteAction")]
        [SerializeField] private AssetReference _inactiveSprite;
        
        [ShowIf("IsChangeSpriteAction")]
        [SerializeField] private AssetReference _activeSprite;
        public bool IsActive;
        
        [Header("Events")] 
        [SerializeField] private UnityEvent _onClickAction = new();

        [Header("Animation Settings")]
        [SerializeField] private Vector3 _punchScale = Vector3.one * 0.3f;
        [SerializeField] private float _animationDuration = 0.3f;

        private string _currentAssetKey;
        private Tween _currentTween;
        private UnityAction _currentAnimationHandler;
        
        private AssetLoadController _assetLoadController;
        private SoundManager _soundManager;
        
        public void Initialize(AssetLoadController assetLoadController, SoundManager soundManager)
        {
            _assetLoadController = assetLoadController;
            _soundManager = soundManager;
        }
        
        private void Start()
        {
            SetupAnimationHandler();
            _button.onClick.AddListener(OnClick);
            
            if (_clickSound != null) _soundManager.LoadSoundEffect("ButtonClick", _clickSound);
        }
        
        private bool IsActivateObjectAction()
        {
            return ButtonType == ButtonType.Switch && ActionType == ButtonActionType.ActivateObject;
        }
        
        private bool IsPush()
        {
            return ButtonType == ButtonType.Push;
        }

        private bool IsChangeSpriteAction()
        {
            return ButtonType == ButtonType.Switch && ActionType == ButtonActionType.ChangeSprite;
        }
        
        private async UniTask<Sprite> LoadButtonAssets(AssetReference spriteToLoad = null)
        {
            var reference = spriteToLoad ?? (IsChangeSpriteAction() ? _inactiveSprite : _spriteReference);
            return await _assetLoadController.Load<Sprite>(reference);
        }
        
        public AdvancedButton SetText(string text)
        {
            _buttonText.SetText(text);
            return this;
        }
        
        public TextMeshProUGUI GetText()
        {
            return _buttonText;
        }

        public AdvancedButton SetAction(UnityAction action)
        {
            _onClickAction.RemoveAllListeners();
            _onClickAction.AddListener(action);
            return this;
        }

        public async UniTask<AdvancedButton> SetBackground()
        {
            _canvasGroup.alpha = 0;
            _buttonImage.sprite = await LoadButtonAssets();
            _canvasGroup.DOFade(1f, _animationDuration);
            return this;
        }
        
        public async UniTask SetTextAndBackground(string text)
        {
            await SetBackground();
            SetText(text);
        }
        
        public async UniTask<AdvancedButton> SetBackground(AssetReference assetReference)
        {
            _canvasGroup.alpha = 0;
            _buttonImage.sprite = await LoadButtonAssets(assetReference);
            _canvasGroup.DOFade(1f, _animationDuration);
            return this;
        }
        
        private void SetupAnimationHandler()
        {
            _currentAnimationHandler = PlayPunchAnimation;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
            _currentTween?.Kill();
        }

        private void OnClick()
        {
            if (!_button.interactable) return;
            SetInteractiveState(false);
            ResetButtonState();

            HandleButtonAction();
            _currentAnimationHandler?.Invoke();
            if (_clickSound != null) _soundManager.PlaySoundEffect("ButtonClick");
            _currentTween.OnKill(OnAnimationComplete);
        }

        private void HandleButtonAction()
        {
            _onClickAction?.Invoke();

            switch (ButtonType)
            {
                case ButtonType.Switch:
                    ToggleState();
                    break;
            }
        }
        
        private void ToggleState()
        {
            IsActive = !IsActive;
            UpdateVisualState();
        }
        
        public void ToggleState(bool toggleState)
        {
            IsActive = toggleState;
            UpdateVisualState();
        }

        private async void UpdateVisualState()
        {
            switch (ActionType)
            {
                case ButtonActionType.ActivateObject:
                    if (_activeObject != null)
                    {
                        _activeObject.sprite = await LoadButtonAssets(_activeObjectSpriteReference);
                        _activeObject.gameObject.SetActive(IsActive);
                    }
                    break;
                
                case ButtonActionType.ChangeSprite:
                    await UpdateButtonSprite();
                    break;
            }
        }

        private async UniTask UpdateButtonSprite()
        {
            var newSprite = IsActive 
                ? await LoadButtonAssets(_activeSprite) 
                : await LoadButtonAssets(_inactiveSprite);

            if (_buttonImage != null && newSprite != null)
                _buttonImage.sprite = newSprite;
        }

        private void PlayPunchAnimation()
        {
            _currentTween = transform.DOPunchScale(_punchScale, _animationDuration)
                .SetEase(Ease.OutQuad);
        }

        private void OnAnimationComplete()
        {
            SetInteractiveState(true);
            ResetButtonState();
        }
        
        public void SetInteractiveState(bool state)
        {
            _canvasGroup.alpha = state ? 1f : 0.5f;
            _canvasGroup.blocksRaycasts = state;
        }

        private void ResetButtonState()
        {
            _currentTween?.Kill();
            transform.localScale = Vector3.one;
        }

    }
}