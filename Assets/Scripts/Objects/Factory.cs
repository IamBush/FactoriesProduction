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
    using Cysharp.Threading.Tasks;
    using System;
    using System.Threading;
    using Unity.Collections;
    using UnityEngine.EventSystems;

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
        [SerializeField] private Transform _entrancePoint; 

        private PlayerMovementController _playerMovement;
        private ProductionManager _productionManager;
        private ResourceDatabase _resourceDatabase;

        private bool _isReadyToCollect;
        private Sequence _currentAnimation;
        private float _remainingTime;
        private CancellationTokenSource _productionCts;

        private float _productionTime;
        private int _productionAmount;
        private Color _resourceColor;

        public void Initialize(PlayerMovementController playerMovement, ProductionManager productionManager, ResourceDatabase resourceDatabase)
        {
            _playerMovement = playerMovement;
            _productionManager = productionManager;
            _resourceDatabase = resourceDatabase;
            _playerMovement.OnDestinationReached += HandleFactoryReached;

            LoadResourceData();

            _progressFillImage.fillAmount = 0f;

            UpdateResourceDisplay();

            _productionCts = new CancellationTokenSource();
            ProductionCycleAsync(_productionCts.Token).Forget();
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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            if (IsPlayerCloseEnough() && _isReadyToCollect)
            {
                CollectResource();
                return;
            }
            _playerMovement.MoveToPosition(_entrancePoint.position);
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
            Vector3 checkPoint = _entrancePoint != null ? _entrancePoint.position : transform.position;
            return Vector3.Distance(checkPoint, _playerMovement.transform.position) < _interactionDistance;
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

            _productionCts?.Cancel();
            _productionCts?.Dispose();
            _productionCts = new CancellationTokenSource();
            
            ProductionCycleAsync(_productionCts.Token).Forget();
        }

        private async UniTaskVoid ProductionCycleAsync(CancellationToken cancellationToken)
        {
            AnimateProgressFill(0f, 1f, _productionTime);
            _remainingTime = _productionTime;
            float startTime = Time.time;

            while (Time.time - startTime < _productionTime)
            {
                _remainingTime = _productionTime - (Time.time - startTime);
                await UniTask.Yield(cancellationToken);
            }

            _isReadyToCollect = true;
            _remainingTime = 0;
            _timerText.SetText(string.Empty);

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
    
            Sequence pulseSequence = DOTween.Sequence();
    
            pulseSequence.Append(
                _indicatorTransform.DOScale(Vector3.one * _popScale, _popDuration / 2)
                    .SetEase(_popEase)
            );
    
            pulseSequence.Append(
                _indicatorTransform.DOScale(Vector3.one, _popDuration / 2)
                    .SetEase(_popEase)
            );
    
            pulseSequence.SetLoops(2, LoopType.Restart);
    
            _currentAnimation.Append(pulseSequence);
    
            _currentAnimation.AppendInterval(1f);
    
            _currentAnimation.SetLoops(-1, LoopType.Restart);
        }

        private void OnDestroy()
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Kill();
            }

            if (_playerMovement != null)
                _playerMovement.OnDestinationReached -= HandleFactoryReached;

            _productionCts?.Cancel();
            _productionCts?.Dispose();
            _productionCts = null;
        }
    }
}