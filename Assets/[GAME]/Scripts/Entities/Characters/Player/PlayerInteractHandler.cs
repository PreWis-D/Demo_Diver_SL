using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractHandler
{
    private PlayerCharacter _playerCharacter;
    private PlayerData _playerData;
    private FactoriesService _factoriesService;

    private List<IInteractable> _nearbyInteractables = new List<IInteractable>();
    private IInteractable _currentInteractable;
    private AbstractInteractionHandler _currentHandler;

    public PlayerInteractHandler(PlayerCharacter playerCharacter, PlayerData playerData)
    {
        _playerCharacter = playerCharacter;
        _playerData = playerData;
        _factoriesService = SL.Get<FactoriesService>();
    }

    public void Update()
    {
        FindNearbyInteractables();

        IInteractable closest = GetClosestInteractable();

        if (_currentInteractable != closest)
        {
            if (_currentInteractable != null)
            {
                SL.Get<EventProcessingService>().InteractableFoundedInvoke(_currentInteractable, false);
                _currentHandler?.Reset();
            }

            if (_currentHandler is ProximityInteractionHandler)
                _playerCharacter.ChangeVacuumActiveState(false);

            _currentInteractable = closest;
            _currentHandler = null;

            if (_currentInteractable != null)
            {
                _currentHandler = _factoriesService.CreateHandler(
                    _currentInteractable.InteractionType
                );
                _currentHandler.Init(_currentInteractable);

                if (_currentInteractable.InteractionType != InteractionType.Automatic)
                {
                    SL.Get<EventProcessingService>().InteractableFoundedInvoke(_currentInteractable, true);
                }
            }
        }

        _currentHandler?.HandleInteraction();
    }

    private void FindNearbyInteractables()
    {
        _nearbyInteractables.Clear();

        Collider[] colliders = Physics.OverlapSphere(
            _playerCharacter.RigidbodyModel.Spine.transform.position,
            _playerData.VacuumData.MaxVacuumRadius,
            _playerData.VacuumData.itemLayer
        );

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract(_playerCharacter))
            {
                _nearbyInteractables.Add(interactable);
            }
        }
    }

    private IInteractable GetClosestInteractable()
    {
        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        foreach (var interactable in _nearbyInteractables)
        {
            float distance = Vector3.Distance(
                _playerCharacter.RigidbodyModel.Spine.transform.position,
                (interactable as MonoBehaviour).transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        return closest;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_playerCharacter.RigidbodyModel.Spine.position, _playerData.VacuumData.MaxVacuumRadius);
    }
}