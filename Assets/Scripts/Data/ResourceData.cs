namespace Data
{
    using Enums;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ResourceData", menuName = "Game/Resource Data")]
    public class ResourceData : ScriptableObject
    {
        [Header("Resource Information")]
        [SerializeField] private ProductionType _resourceType;
        [SerializeField] private string _resourceName;
        [SerializeField] private Sprite _icon;
        
        [Header("Production Settings")]
        [SerializeField] private float _productionTime = 5f;
        [SerializeField] private int _productionAmount = 1;

        public ProductionType ResourceType => _resourceType;
        public string ResourceName => _resourceName;
        public Sprite Icon => _icon;
        public float ProductionTime => _productionTime;
        public int ProductionAmount => _productionAmount;
    }
}