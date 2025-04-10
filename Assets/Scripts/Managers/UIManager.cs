using UnityEngine;
using TMPro;
using Enums;
using System.Collections;

namespace Managers
{
    using System;
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _ironText;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _oilText;
        [SerializeField] private TextMeshProUGUI _woodText;

        [SerializeField] private GameObject _popupPanel;
        [SerializeField] private TextMeshProUGUI _popupText;
        [SerializeField] private float _popupDuration = 2f;

        private Coroutine _currentPopupCoroutine;
        private ProductionType _lastCollectedResourceType;
        private int _lastCollectedAmount;

        private void Awake()
        {
            UpdateResourcesUI(0, 0, 0, 0);
        }

        public void UpdateResourcesUI(int iron, int gold, int oil, int wood)
        {
            _ironText.SetText($"Iron: {iron}");
            _goldText.SetText($"Gold: {gold}");
            _oilText.SetText($"Oil: {oil}");
            _woodText.SetText($"Wood: {wood}");
        }

        public void ShowResourceCollectedPopup(ProductionType resourceType, int amount)
        {
            string resourceName = GetResourceName(resourceType);
            ShowPopup($"Collected: {amount} {resourceName}!");

            _lastCollectedResourceType = resourceType;
            _lastCollectedAmount = amount;
        }

        private string GetResourceName(ProductionType resourceType)
        {
            switch (resourceType)
            {
                case ProductionType.Iron:
                    return "Iron";
                case ProductionType.Gold:
                    return "Gold";
                case ProductionType.Oil:
                    return "Oil";
                case ProductionType.Wood:
                    return "Wood";
                case ProductionType.Stone:
                    return "Stone";
                default:
                    return "Resource";
            }
        }

        private void ShowPopup(string message)
        {
            if (_currentPopupCoroutine != null)
            {
                StopCoroutine(_currentPopupCoroutine);
            }

            _popupText.text = message;
            _popupPanel.SetActive(true);
            _currentPopupCoroutine = StartCoroutine(HidePopupAfterDelay());
        }

        private IEnumerator HidePopupAfterDelay()
        {
            yield return new WaitForSeconds(_popupDuration);
            _popupPanel.SetActive(false);
            _currentPopupCoroutine = null;
        }
    }
}