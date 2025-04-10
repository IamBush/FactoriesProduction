namespace Managers
{
    using UnityEngine;
    using System.Collections.Generic;
    using Enums;
    using VContainer;
    using System;
    
    public class ProductionManager : MonoBehaviour
    {
        private Dictionary<ProductionType, int> _resources = new Dictionary<ProductionType, int>();
        private UIManager _uiManager;

        [Inject]
        public void Construct(UIManager uiManager)
        {
            _uiManager = uiManager;

            foreach (ProductionType type in Enum.GetValues(typeof(ProductionType)))
            {
                _resources[type] = 0;
            }
        }

        public void AddResource(ProductionType type, int amount)
        {
            _resources.TryAdd(type, 0);
            _resources[type] += amount;

            UpdateUI();

            _uiManager.ShowResourceCollectedPopup(type, amount);
        }

        private void UpdateUI()
        {
            if (_uiManager != null)
            {
                int iron = GetResourceAmount(ProductionType.Iron);
                int gold = GetResourceAmount(ProductionType.Gold);
                int oil = GetResourceAmount(ProductionType.Oil);
                int wood = GetResourceAmount(ProductionType.Wood);

                _uiManager.UpdateResourcesUI(iron, gold, oil, wood);
            }
        }

        public int GetResourceAmount(ProductionType type)
        {
            return _resources.GetValueOrDefault(type, 0);
        }
    }
}