using System;
using System.Collections.Generic;
using UnityEngine;

// SL - Service Locator
public static class SL
{
    private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
    private static readonly object _lock = new object();
    private static bool _isInitialized = false;

    public static void Init()
    {
        if (_isInitialized) return;

        lock (_lock)
        {
            if (_isInitialized) return;

            _services.Clear();
            _isInitialized = true;
        }
    }

    public static void Register<T>(T service) where T : class, IService
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        var type = typeof(T);

        lock (_lock)
        {
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"the service {type.Name} is already registered");
                _services[type].Cleanup();
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
            }

            service.Initialize();
        }
    }

    public static T Get<T>() where T : class, IService
    {
        var type = typeof(T);

        if (!_services.TryGetValue(type, out var service))
        {
            throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
        }

        return service as T;
    }

    public static void Unregister<T>() where T : class, IService
    {
        var type = typeof(T);

        lock (_lock)
        {
            if (_services.TryGetValue(type, out var service))
            {
                service.Cleanup();
                _services.Remove(type);
            }
        }
    }

    public static void UnregisterAll()
    {
        lock (_lock)
        {
            foreach (var service in _services.Values)
            {
                service.Cleanup();
            }

            _services.Clear();
            _isInitialized = false;
        }
    }
}