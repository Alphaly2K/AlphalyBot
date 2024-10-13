namespace AlphalyBot.Tool;

internal static class RandomR
{
    public static List<int> GenerateUniqueRandomNumbers(int lowerBound, int upperBound, int count)
    {
        if (count > upperBound - lowerBound + 1) throw new ArgumentException("范围内的数字不足以生成指定个数的不重复随机数。");

        var availableNumbers = new List<int>();
        for (var i = lowerBound; i <= upperBound; i++) availableNumbers.Add(i);

        var random = new Random();
        var result = new List<int>();

        for (var i = 0; i < count; i++)
        {
            var index = random.Next(availableNumbers.Count);
            result.Add(availableNumbers[index]);
            availableNumbers.RemoveAt(index); // 移除已使用的数字
        }

        return result;
    }
}