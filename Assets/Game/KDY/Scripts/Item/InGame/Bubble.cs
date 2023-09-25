using KDY;

public class Bubble : InGameItem
{
    public override void ApplyStatus(InGamePlayer player)
    {
        if (player.maxbombCount >= InGamePlayer.limitbombCount)
            return;

        player.maxbombCount += 1;
    }

    public override void RemoveStatus(InGamePlayer player)
    {
        player.maxbombCount -= 1;
    }
}