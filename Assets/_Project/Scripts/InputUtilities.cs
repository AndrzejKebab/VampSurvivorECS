namespace AndrzejKebab;

public struct FixedInputEvent
{
    private byte wasEverSet;
    private uint lastSetTick;

    public void Set(uint tick)
    {
        lastSetTick = tick;
        wasEverSet  = 1;
    }

    public readonly bool IsSet(uint tick)
    {
        if (wasEverSet == 1)
        {
            return tick == lastSetTick;
        }

        return false;
    }
}