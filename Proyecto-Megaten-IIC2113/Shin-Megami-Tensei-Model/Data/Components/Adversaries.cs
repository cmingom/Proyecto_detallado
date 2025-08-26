namespace Shin_Megami_Tensei_Model.Data.Components;

public class Adversaries
{
    private readonly List<Player> _players = [new(1), new(2)];
    private int _roundNumber = 1;
    private bool _isFirstPlayerTurn = true;

    public Player Attacker => _players.First();

    public Player Defender => _players.Last();
    
    public Player Player1 => _players[0];
    
    public Player Player2 => _players[1];

    public bool IsOnePlayerDefeated()
    {
        return Attacker.IsDefeated() || Defender.IsDefeated() || Attacker.HasSurrendered || Defender.HasSurrendered;
    }

    public void SwapRoles()
    {
        _players.Reverse();
        if (_isFirstPlayerTurn)
        {
            _isFirstPlayerTurn = false;
        }
        else
        {
            _isFirstPlayerTurn = true;
            _roundNumber++;
        }
    }
    
    public int GetRoundNumber()
    {
        return _roundNumber;
    }
    
    public Player GetOpponent()
    {
        return Defender;
    }
}
