namespace Objects
{
    using Controllers;
    using Data;
    using DG.Tweening;
    using Enums;
    using Managers;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections;

    public class Factory : MonoBehaviour
    {
        [Header("Production Settings")]
        [SerializeField] private ProductionType _resourceType;

        [Header("Visual Indicators")]
        [SerializeField] private Image _progressFillImage;
        [SerializeField] private Transform _indicatorTransform;
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private TextMeshProUGUI _resourceAmountText;
        [SerializeField] private TextMeshProUGUI _timerText;

        [Header("Animation Settings")]
        [SerializeField] private float _popScale = 1.2f;
        [SerializeField] private float _popDuration = 0.3f;
        [SerializeField] private Ease _fillEase = Ease.Linear;
        [SerializeField] private Ease _popEase = Ease.OutBack;

        [Header("Interaction Settings")]
        [SerializeField] private float _interactionDistance = 2f;

        private PlayerMovementController _playerMovement;
        private ProductionManager _productionManager;
        private ResourceDatabase _resourceDatabase;

        private bool _isReadyToCollect;
        private bool _isInitialized = false;
        private Sequence _currentAnimation;
        private float _remainingTime;

        private float _productionTime;
        private int _productionAmount;
        private Color _resourceColor;

        public void Initialize(PlayerMovementController playerMovement, ProductionManager productionManager, ResourceDatabase resourceDatabase)
        {
            _playerMovement = playerMovement;
            _productionManager = productionManager;
            _resourceDatabase = resourceDatabase;
            _isInitialized = true;
            _playerMovement.OnDestinationReached += HandleFactoryReached;

            LoadResourceData();

            _progressFillImage.fillAmount = 0f;

            UpdateResourceDisplay();

            StartCoroutine(ProductionCycle());
        }

        private void LoadResourceData()
        {
            _productionTime = _resourceDatabase.GetProductionTime(_resourceType);
            _productionAmount = _resourceDatabase.GetProductionAmount(_resourceType);
        }

        private void UpdateResourceDisplay()
        {
            Sprite iconSprite = _resourceDatabase.GetResourceIcon(_resourceType);
            _resourceIcon.sprite = iconSprite;
            _resourceAmountText.text = $"x{_productionAmount}";
        }

        private void Update()
        {
            if (!_isReadyToCollect)
            {
                UpdateTimerText();
            }
        }

        private void UpdateTimerText()
        {
            int seconds = Mathf.CeilToInt(_remainingTime);
            _timerText.text = seconds.ToString();
        }

        private void OnMouseDown()
        {
            if (_playerMovement != null)
            {
                _playerMovement.MoveToPosition(transform.position);
            }
        }

        private void HandleFactoryReached()
        {
            if (IsPlayerCloseEnough() && _isReadyToCollect)
            {
                CollectResource();
            }
        }

        private bool IsPlayerCloseEnough()
        {
            return Vector3.Distance(transform.position, _playerMovement.transform.position) < _interactionDistance;
        }

        private void CollectResource()
        {
            _productionManager.AddResource(_resourceType, _productionAmount);
            _isReadyToCollect = false;
            _progressFillImage.fillAmount = 0f;
            _timerText.SetText(Mathf.CeilToInt(_productionTime).ToString());

            if (_currentAnimation != null)
            {
                _currentAnimation.Kill();
            }

            StartCoroutine(ProductionCycle());
        }

        private IEnumerator ProductionCycle()
        {
            AnimateProgressFill(0f, 1f, _productionTime);
            _resourceAmountText.SetText(string.Empty);
            _remainingTime = _productionTime;
            float startTime = Time.time;

            while (Time.time - startTime < _productionTime)
            {
                _remainingTime = _productionTime - (Time.time - startTime);
                yield return null;
            }

            _isReadyToCollect = true;
            _remainingTime = 0;
            _timerText.SetText(string.Empty);
            _resourceAmountText.SetText($"x{_productionAmount}");
            
            PlayReadyAnimation();
        }

        private void AnimateProgressFill(float fromValue, float toValue, float duration)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Kill();
            }

            _currentAnimation = DOTween.Sequence();
            _progressFillImage.fillAmount = fromValue;
            _currentAnimation.Append(
                _progressFillImage.DOFillAmount(toValue, duration)
                    .SetEase(_fillEase)
            );
        }

        private void PlayReadyAnimation()
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Kill();
            }

            _currentAnimation = DOTween.Sequence();

            _currentAnimation.Append(
                _indicatorTransform.DOScale(Vector3.one * _popScale, _popDuration / 2)
                    .SetEase(_popEase)
            );

            _currentAnimation.Append(
                _indicatorTransform.DOScale(Vector3.one, _popDuration / 2)
                    .SetEase(_popEase)
            );

            _currentAnimation.SetLoops(2);
        }

        private void OnDestroy()
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Kill();
            }

            if (_playerMovement != null)
                _playerMovement.OnDestinationReached -= HandleFactoryReached;
        }
    }
}