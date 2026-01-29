using UnityEngine;

public abstract class AbstractRangeWeapon : AbstractHandItem
{
    [SerializeField] protected Transform ShootPoint;
    public abstract Transform LeftHand { get; }
    public abstract Transform RightHand { get; }
}