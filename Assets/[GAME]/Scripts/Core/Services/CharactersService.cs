using System.Linq;

public class CharactersService : IService
{
    private PlayerData _playerData;
    private PlayerCharacter _playerCharacter;
    private PlayerConfig[] _configs;

    public CharactersService(PlayerConfig[] configs)
    {
        _configs = configs;
    }

    public void Initialize()
    {
        _playerData = new PlayerData(_configs.FirstOrDefault(c => c.EntityType == EntityType.Player));
    }

    public void Cleanup()
    {

    }

    public void SetPlayerCharacter(PlayerCharacter playerCharacter)
    {
        _playerCharacter = playerCharacter;
    }

    public PlayerData GetPlayerData()
    {
        return _playerData;
    }

    public PlayerCharacter GetPlayerCharacter()
    {
        return _playerCharacter;
    }
}