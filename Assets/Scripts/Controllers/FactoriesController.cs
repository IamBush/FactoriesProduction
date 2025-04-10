namespace Controllers
{
    using Data;
    using Objects;
    using Managers;
    using UnityEngine;
    using VContainer;
    using System.Collections.Generic;

    public class FactoriesController : MonoBehaviour
    {
        [SerializeField] private List<Factory> _factories = new List<Factory>();
        [SerializeField] private ResourceDatabase _resourceDatabase;
        
        private PlayerMovementController _playerMovement;
        private ProductionManager _productionManager;
        
        [Inject]
        public void Construct(PlayerMovementController playerMovement, ProductionManager productionManager)
        {
            _playerMovement = playerMovement;
            _productionManager = productionManager;
            
            InitializeFactories();
        }
        
        private void InitializeFactories()
        {
            foreach (var factory in _factories)
            {
                factory.Initialize(_playerMovement, _productionManager, _resourceDatabase);
            }
        }
    }
}