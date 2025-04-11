namespace Data
{
    using Enums;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ResourceDatabase", menuName = "Game/Resource Database")]
    public class ResourceDatabase : ScriptableObject
    {
        [SerializeField] private List<ResourceData> _resources = new List<ResourceData>();
        
        private Dictionary<ProductionType, ResourceData> _resourceLookup;

        private void OnEnable()
        {
            InitializeLookup();
        }

        private void InitializeLookup()
        {
            _resourceLookup = new Dictionary<ProductionType, ResourceData>();
            
            foreach (var resource in _resources)
            {
                if (resource != null)
                {
                    _resourceLookup[resource.ResourceType] = resource;
                }
            }
        }

        public ResourceData GetResourceData(ProductionType resourceType)
        {
            if (_resourceLookup == null)
            {
                InitializeLookup();
            }

            return _resourceLookup.GetValueOrDefault(resourceType);

        }

        public Sprite GetResourceIcon(ProductionType resourceType)
        {
            ResourceData data = GetResourceData(resourceType);
            return data?.Icon;
        }
        
        public string GetResourceName(ProductionType resourceType)
        {
            ResourceData data = GetResourceData(resourceType);
            return data?.ResourceName;
        }

        public float GetProductionTime(ProductionType resourceType)
        {
            ResourceData data = GetResourceData(resourceType);
            return data != null ? data.ProductionTime : 5f; 
        }

        public int GetProductionAmount(ProductionType resourceType)
        {
            ResourceData data = GetResourceData(resourceType);
            return data != null ? data.ProductionAmount : 1; 
        }
    }
}