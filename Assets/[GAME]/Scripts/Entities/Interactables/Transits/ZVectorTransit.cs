using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ZVectorTransit : AbstractInteractable
{
    private CancellationTokenSource _cancellationTokenSource;

    public bool IndicatorShowed { get; private set; } = true;

    public override void Interrupt() 
    {
        base.Interrupt();
        Collider.enabled = true;
    }

    private async UniTask Restart(CancellationToken token)
    {
        await UniTask.Delay(1000, cancellationToken: token);

        Interrupt();
    }

    protected override void ExecuteDone()
    {
        PlayerCharacter playerCharacter = SL.Get<CharactersService>().GetPlayerCharacter();
        playerCharacter.StateMachine.SetState<PlayerSwitchVectorZState>();
        Collider.enabled = false;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        Restart(_cancellationTokenSource.Token).Forget();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }
}