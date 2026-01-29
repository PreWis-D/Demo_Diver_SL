using System.Collections.Generic;
using UnityEngine;

public class VacuumController
{
    private float _quickPickupRadius = 3f;
    private float _quickPickupDuration = 0.3f;
    private float _maxVacuumRadius = 10f;
    private float _vacuumGrowthSpeed = 5f;
    private float _vacuumForce = 15f;
    private LayerMask _itemLayer;

    private HashSet<ResourceItem> _attractedItems = new HashSet<ResourceItem>();

    private enum VacuumMode { None, QuickPickup, Vacuuming }
    private VacuumMode _currentMode = VacuumMode.None;

    private float _buttonPressTime;
    private float _currentVacuumRadius;
    private bool _isActive;

    private PlayerCharacter _playerCharacter;
    private Collider[] _overlapResults = new Collider[50];

    private InputProcessingService _input;

    public VacuumController(PlayerCharacter playerCharacter, PlayerData playerData)
    {
        _quickPickupRadius = playerData.VacuumData.QuickPickupRadius;
        _quickPickupDuration = playerData.VacuumData.QuickPickupDuration;
        _maxVacuumRadius = playerData.VacuumData.MaxVacuumRadius;
        _vacuumGrowthSpeed = playerData.VacuumData.VacuumGrowthSpeed;
        _vacuumForce = playerData.VacuumData.VacuumForce;
        _itemLayer = playerData.VacuumData.itemLayer;

        _playerCharacter = playerCharacter;

        _input = SL.Get<InputProcessingService>();
        _input.InteractStarted += OnInteractStarted;
        _input.InteractCanceled += OnInteractCanceled;
    }

    public void ChangeActiveState(bool isActive)
    {
        _isActive = isActive;
    }

    public void Update()
    {
        if (_isActive == false)
            return;

        if (_currentMode == VacuumMode.QuickPickup && Time.time - _buttonPressTime > _quickPickupDuration)
            StartVacuum();

        if (_currentMode == VacuumMode.Vacuuming)
            UpdateVacuumRadius();
    }

    private void OnInteractStarted()
    {
        _buttonPressTime = Time.time;
        _currentMode = VacuumMode.QuickPickup;
    }

    private void OnInteractCanceled()
    {
        float pressDuration = Time.time - _buttonPressTime;

        if (pressDuration <= _quickPickupDuration && _currentMode == VacuumMode.QuickPickup)
        {
            PerformQuickPickup();
        }
        else if (_currentMode == VacuumMode.Vacuuming)
        {
            StopAllAttraction();
        }

        _currentMode = VacuumMode.None;
    }

    private void PerformQuickPickup()
    {
        int count = Physics.OverlapSphereNonAlloc(
            _playerCharacter.RigidbodyModel.Spine.transform.position,
            _quickPickupRadius,
            _overlapResults,
            _itemLayer
        );

        ResourceItem nearestItem = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (_overlapResults[i].TryGetComponent<ResourceItem>(out var item))
            {
                float distance = Vector3.Distance(
                     _playerCharacter.RigidbodyModel.Spine.transform.position,
                    _overlapResults[i].transform.position
                );

                if (distance < nearestDistance && item.CanInteract(_playerCharacter))
                {
                    nearestDistance = distance;
                    nearestItem = item;
                }
            }
        }

        if (nearestItem != null)
        {
            ResourceMagnetState state = nearestItem.StateMachine.GetState<ResourceMagnetState>();
            state.SetTarget(_playerCharacter.RigidbodyModel.Spine.transform);
            nearestItem.StateMachine.SetState<ResourceMagnetState>();
            _attractedItems.Add(nearestItem);
        }
    }

    private void StartVacuum()
    {
        _currentMode = VacuumMode.Vacuuming;
        _currentVacuumRadius = _quickPickupRadius;

        FindAndAttractItems();
    }

    private void UpdateVacuumRadius()
    {
        _currentVacuumRadius = Mathf.Min(
            _currentVacuumRadius + _vacuumGrowthSpeed * Time.deltaTime,
            _maxVacuumRadius
        );

        FindAndAttractItems();
    }

    private void FindAndAttractItems()
    {
        int count = Physics.OverlapSphereNonAlloc(
             _playerCharacter.RigidbodyModel.Spine.transform.position,
            _currentVacuumRadius,
            _overlapResults,
            _itemLayer
        );

        for (int i = 0; i < count; i++)
        {
            if (_overlapResults[i].TryGetComponent<ResourceItem>(out var item))
            {
                if (item.CanInteract(_playerCharacter) && !_attractedItems.Contains(item))
                {
                    ResourceMagnetState state = item.StateMachine.GetState<ResourceMagnetState>();
                    state.SetTarget(_playerCharacter.RigidbodyModel.Spine.transform);
                    item.StateMachine.SetState<ResourceMagnetState>();
                    _attractedItems.Add(item);
                }
            }
        }
    }

    private void StopAllAttraction()
    {
        foreach (var item in _attractedItems)
        {
            if (item != null)
                item.StateMachine.SetState<ResourceWaterState>();
        }

        _attractedItems.Clear();
    }

    public void OnItemCollected(ResourceItem item)
    {
        _attractedItems.Remove(item);
    }

    public void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && _currentMode == VacuumMode.Vacuuming)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_playerCharacter.RigidbodyModel.Spine.transform.position, _currentVacuumRadius);
        }
    }

    public void OnDestroy()
    {
        _input.InteractStarted -= OnInteractStarted;
        _input.InteractCanceled -= OnInteractCanceled;
    }
}