using KDY;

public class Roller : InGameItem
{
    public override void ApplyStatus(InGamePlayer player)
    {
        if (player.moveSpeed >= InGamePlayer.limitmoveSpeed)
            return;

        player.moveSpeed += 0.15f;
    }

    public override void RemoveStatus(InGamePlayer player)
    {
        player.moveSpeed -= 0.15f;
    }
}