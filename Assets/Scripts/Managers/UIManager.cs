using UnityEngine;
using TMPro;
using Enums;
using System.Collections;
using UnityEngine.UI;
using Data;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Managers
{
    using System;
    public class UIManager : MonoBehaviour
    {
        [Header("Resource UI Elements")]
        [SerializeField] private TextMeshProUGUI _ironText;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _oilText;
        [SerializeField] private TextMeshProUGUI _woodText;
        [SerializeField] private TextMeshProUGUI _stoneText;

        [SerializeField] private Image _ironIcon;
        [SerializeField] private Image _goldIcon;
        [SerializeField] private Image _oilIcon;
        [SerializeField] private Image _woodIcon;
        [SerializeField] private Image _stoneIcon;

        [Header("Popup Settings")]
        [SerializeField] private GameObject _popupPanel;
        [SerializeField] private TextMeshProUGUI _popupText;
        [SerializeField] private Image _popupIcon;
        [SerializeField] private float _popupDuration = 2f;

        private CancellationTokenSource _popupCancellationTokenSource;
        private ProductionType _lastCollectedResourceType;
        private int _lastCollectedAmount;
        private ResourceDatabase _resourceDatabase;

        public void Initialize(ResourceDatabase resourceDatabase)
        {
            _resourceDatabase = resourceDatabase;
            UpdateResourcesUI(0, 0, 0, 0, 0);
            InitializeResourceIcons();
        }

        private void InitializeResourceIcons()
        {
            if (_resourceDatabase == null)
            {
                Debug.LogError("ResourceDatabase not initialized in UIManager");
                return;
            }

            _ironIcon.sprite = _resourceDatabase.GetResourceIcon(ProductionType.Iron);
            _goldIcon.sprite = _resourceDatabase.GetResourceIcon(ProductionType.Gold);
            _oilIcon.sprite = _resourceDatabase.GetResourceIcon(ProductionType.Oil);
            _woodIcon.sprite = _resourceDatabase.GetResourceIcon(ProductionType.Wood);
            _stoneIcon.sprite = _resourceDatabase.GetResourceIcon(ProductionType.Stone);
        }

        private void UpdateResourcesUI(int iron, int gold, int oil, int wood, int stone)
        {
            _ironText.SetText($"{iron}");
            _goldText.SetText($"{gold}");
            _oilText.SetText($"{oil}");
            _woodText.SetText($"{wood}");
            _stoneText.SetText($"{stone}");
        }

        public void UpdateResourceUI(ProductionType resourceType, int amount)
        {
            switch (resourceType)
            {
                case ProductionType.Iron:
                    _ironText.SetText($"{amount}");
                    break;
                case ProductionType.Gold:
                    _goldText.SetText($"{amount}");
                    break;
                case ProductionType.Oil:
                    _oilText.SetText($"{amount}");
                    break;
                case ProductionType.Wood:
                    _woodText.SetText($"{amount}");
                    break;
                case ProductionType.Stone:
                    _stoneText.SetText($"{amount}");
                    break;
                default:
                    Debug.LogWarning($"Unknown resource type: {resourceType}");
                    break;
            }
        }

        public void ShowResourceCollectedPopup(ProductionType resourceType, int amount)
        {
            string resourceName = _resourceDatabase.GetResourceName(resourceType);
            Sprite resourceIcon = _resourceDatabase.GetResourceIcon(resourceType);

            ShowPopup($"Collected: {amount} {resourceName}!", resourceIcon);

            _lastCollectedResourceType = resourceType;
            _lastCollectedAmount = amount;
        }

        private void ShowPopup(string message, Sprite icon)
        {
            _popupCancellationTokenSource?.Cancel();
            _popupCancellationTokenSource = new CancellationTokenSource();

            _popupText.text = message;
            _popupIcon.sprite = icon;

            _popupPanel.SetActive(true);
            HidePopupAfterDelayAsync(_popupCancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid HidePopupAfterDelayAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay((int)(_popupDuration * 1000), cancellationToken: cancellationToken);
            _popupPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            _popupCancellationTokenSource?.Cancel();
            _popupCancellationTokenSource?.Dispose();
        }
    }
}