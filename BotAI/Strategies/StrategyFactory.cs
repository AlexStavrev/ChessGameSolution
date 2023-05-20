namespace BotAI.Strategies;
public static class StrategyFactory
{
    private static readonly Random _random = new Random();

    public static IBotStrategy GetRandomStrategy()
    {
        return _random.Next(0, 4) switch
        {
            0 => new FirstInMindStrategy(),
            //1 => new EvasiveStrategy(),
            2 => new GoodMovesStrategy(),
            _ => new RandomMoveStrategy(),
        };
    }
}
