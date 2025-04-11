namespace Managers
{
    using System.Collections.Generic;
    using Enums;
    using VContainer;
    using System;

    public class ProductionManager 
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

            UpdateUI(type);

            _uiManager.ShowResourceCollectedPopup(type, amount);
        }
        
        private void UpdateUI(ProductionType resourceType)
        {
            int amount = GetResourceAmount(resourceType);
            _uiManager.UpdateResourceUI(resourceType, amount);
        }

        public int GetResourceAmount(ProductionType type)
        {
            return _resources.GetValueOrDefault(type, 0);
        }
    }
}