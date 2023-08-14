using KDY;

public class Fluid : InGameItem
{
    public override void ApplyStatus(InGamePlayer player)
    {
        if (player.bombPower >= InGamePlayer.limitbombPower)
            return;

        player.bombPower += 1;
    }

    public override void RemoveStatus(InGamePlayer player)
    {
        player.bombPower -= 1;
    }
}