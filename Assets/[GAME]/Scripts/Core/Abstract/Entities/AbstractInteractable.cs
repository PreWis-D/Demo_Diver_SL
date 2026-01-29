using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class AbstractInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public InteractionType InteractionType { get; private set; }
    [field: SerializeField] public InputType InputType { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public float HoldDuration { get; private set; }
    [SerializeField] private float _interactDistance = 0.5f;

    protected Collider Collider;

    public FloatParameter FloatParameter { get; private set; }

    private void Awake()
    {
        Collider = GetComponent<Collider>();
    }

    public virtual void Init()
    {
        FloatParameter = new FloatParameter(HoldDuration, 0);
        FloatParameter.SetToMin();
    }

    public virtual void Execute()
    {
        if (InteractionType == InteractionType.Hold)
        {
            FloatParameter.IncreaseValue(Time.deltaTime);
            if (FloatParameter.IsAtMaxValue())
                ExecuteDone();
        }
        else
        {
            ExecuteDone();
        }
    }

    public virtual void Interrupt()
    {
        FloatParameter.SetToMin();
        Collider.enabled = true;
    }

    public virtual bool CanInteract(PlayerCharacter player)
    {
        if (player == null || Collider.enabled == false) return false;

        float distance = Vector3.Distance(
            Transform?.position ?? transform.position,
            player.transform.position
        );

        return distance <= _interactDistance;
    }

    protected abstract void ExecuteDone();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Transform.position, _interactDistance);
    }
}