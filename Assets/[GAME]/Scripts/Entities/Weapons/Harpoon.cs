using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Harpoon : AbstractRangeWeapon
{
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    private GeneralComponentsService _componentsService;
    private HarpoonBullet _harpoonBullet;
    private CancellationTokenSource _cancellationTokenSource;
    private float _startDamage;
    private float _bulletSpeed;
    private float _reloadTime;
    private bool _isAvailable;

    public override HandItemType Type => HandItemType.Harpoon;
    public override Transform LeftHand => _leftHand;
    public override Transform RightHand => _rightHand;

    public override void Init(HandItemData itemData)
    {
        base.Init(itemData);

        HarpoonData data = itemData as HarpoonData;

        _componentsService = SL.Get<GeneralComponentsService>();

        _startDamage = data.StartDamage;
        _bulletSpeed = data.BulletSpeed;
        _reloadTime = data.ReloadTime;

        PrepareBullet();
    }

    private void PrepareBullet()
    {
        var bulletGO =
            PoolManager.GetPool(
                _componentsService.GetPrefab(PrefabType.HarpoonBullet), _shootPoint.position, Quaternion.identity);

        bulletGO.transform.SetParent(_shootPoint.transform);
        bulletGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

        _harpoonBullet = bulletGO.GetComponent<HarpoonBullet>();
        _harpoonBullet.Init(_startDamage, _bulletSpeed);
        _harpoonBullet.enabled = false;
        _isAvailable = true;
    }

    public override bool CanInteract()
    {
        return View.gameObject.activeSelf && Input.IsFiring && _isAvailable;
    }

    public void Interact()
    {
        _isAvailable = false;
        _harpoonBullet.transform.SetParent(null, true);
        _harpoonBullet.enabled = true;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        Reload(_cancellationTokenSource.Token).Forget();
    }

    protected override void HandleVisible()
    {
        View.gameObject.SetActive(Input.IsAim);
    }

    private async UniTask Reload(CancellationToken token)
    {
        await UniTask.Delay((int)(_reloadTime * 1000), cancellationToken: token);

        PrepareBullet();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }
}