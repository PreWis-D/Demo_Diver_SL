using System.Collections.Generic;
using UnityEngine;

public class ElementsResourceProducer : AbstractResourceProducer
{
    [SerializeField] private float _startHealth;
    [SerializeField] private float _percentToDestroy = 0.25f;
    [SerializeField] private HitFx _hitVFXPrefab;
    [SerializeField] private HitFx _dieFXPrefab;
    [SerializeField] private bool _isCanResourcesGenerated;

    [field: SerializeField] public Transform HitTargetPoint { get; private set; }
    [field: SerializeField] public ResourceType Type { get; private set; }
    [field: SerializeField] public float Resistance { get; private set; }

    private List<ResourceProducerElement> _elements = new List<ResourceProducerElement>();
    private List<ResourceProducerElement> _currentElements = new List<ResourceProducerElement>();

    private void Awake()
    {
        _elements.AddRange(transform.GetComponentsInChildren<ResourceProducerElement>());
    }

    public override void Init()
    {
        _currentElements.AddRange(_elements);

        foreach (var element in _currentElements)
        {
            element.Init(_startHealth / _elements.Count, HitTargetPoint, _hitVFXPrefab, _dieFXPrefab, Type, Resistance, _isCanResourcesGenerated);
            element.gameObject.SetActive(true);
            element.ResourceGenerated += OnResourceGenerated;
            element.ElementDied += OnElementDied;
        }
    }

    private void OnResourceGenerated(ResourceItem item) { }

    private void OnElementDied(IDamagable damagable)
    {
        ResourceProducerElement element = damagable as ResourceProducerElement;
        element.ElementDied -= OnElementDied;
        element.ResourceGenerated -= OnResourceGenerated;
        _currentElements.Remove(element);

        float currentPercentRemain = (float)_currentElements.Count / (float)_elements.Count;
        if (currentPercentRemain < _percentToDestroy)
        {
            foreach (var currentElement in _currentElements)
                currentElement.GenerateResources(transform.position);

            _currentElements.Clear();
        }
    }
}