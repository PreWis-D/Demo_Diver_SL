using System.Collections.Generic;
using UnityEngine;
using System;

public class FactoriesService : IService
{
    private Dictionary<InteractionType, Type> _handlerTypes =
       new Dictionary<InteractionType, Type>
   {
        { InteractionType.Instant, typeof(InstantInteractionHandler) },
        { InteractionType.Hold, typeof(HoldInteractionHandler) },
        { InteractionType.Automatic, typeof(AutomaticInteractionHandler) },
        { InteractionType.Proximity, typeof(ProximityInteractionHandler) }
   };

    private Dictionary<InteractionType, AbstractInteractionHandler> _cachedHandlers =
        new Dictionary<InteractionType, AbstractInteractionHandler>();

    public void Initialize() { }
    public void Cleanup() { }

    public AbstractInteractionHandler CreateHandler(InteractionType type)
    {
        if (_cachedHandlers.TryGetValue(type, out var cachedHandler))
            return cachedHandler;

        if (_handlerTypes.TryGetValue(type, out var handlerType))
        {
            var handler = (AbstractInteractionHandler)Activator.CreateInstance(handlerType);
            _cachedHandlers[type] = handler;
            return handler;
        }

        return new InstantInteractionHandler();
    }

    public void RegisterHandler(InteractionType type, Type handlerType)
    {
        if (!typeof(AbstractInteractionHandler).IsAssignableFrom(handlerType))
        {
            Debug.LogError($"Handler type must inherit from InteractionHandler: {handlerType}");
            return;
        }

        _handlerTypes[type] = handlerType;
        _cachedHandlers.Remove(type);
    }
}