namespace Controllers
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.UI;
    using VContainer;

    public class AssetLoadController 
    {
        private Dictionary<string, AsyncOperationHandle> _loadedAssets = new();
        private CancellationTokenSource _globalCts;
        
        [Inject]
        public void Construct()
        {
            _globalCts = new CancellationTokenSource();
        }

        public async UniTask<T> Load<T>(AssetReference assetRef, CancellationToken ct = default)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_globalCts.Token, ct);
            var linkedToken = linkedCts.Token;
            
            string key = assetRef.RuntimeKey.ToString();

            if (_loadedAssets.TryGetValue(key, out AsyncOperationHandle handle)) //Cache check
            {
                if (handle.IsDone)
                    return (T)handle.Result;

                await handle.WithCancellation(linkedToken);
                return (T)handle.Result;
            }

            try //Cache not found, load asset
            {
                var newHandle = Addressables.LoadAssetAsync<T>(assetRef);
                _loadedAssets.Add(key, newHandle);

                await newHandle.WithCancellation(linkedToken);
                return newHandle.Result;
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log($"Loading of asset {key} was cancelled");
                _loadedAssets.Remove(key);
                return default;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load asset {key}: {e.Message}");
                _loadedAssets.Remove(key);
                return default;
            }
        }

        public async UniTask SetSprite(Image image, AssetReference spriteRef, CancellationToken ct = default)
        {
            CancellationToken effectiveCt = ct == default ? _globalCts.Token : ct;
            image.sprite = await Load<Sprite>(spriteRef, effectiveCt);
        }

        public void ReleaseAsset(AssetReference assetRef)
        {
            string key = assetRef.RuntimeKey.ToString();
            if (_loadedAssets.TryGetValue(key, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
                _loadedAssets.Remove(key);
            }
        }
        
        public void ReleaseAssetsByLabel(AssetLabelReference labelRef)
        {
            var keysToRemove = new List<string>();
            
            foreach (var pair in _loadedAssets)
            {
                if (pair.Value.Result is IKeyEvaluator asset && 
                    asset.RuntimeKeyIsValid())
                {
                    Addressables.Release(pair.Value);
                    keysToRemove.Add(pair.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                _loadedAssets.Remove(key);
            }
        }

        private void OnDestroy()
        {
            _globalCts?.Cancel();
            _globalCts?.Dispose();

            foreach (var handle in _loadedAssets.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _loadedAssets.Clear();
        }
    }
}