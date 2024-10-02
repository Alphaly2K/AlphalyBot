using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphalyBot.Tool
{
    internal class RandomR
    {
        public static List<int> GenerateUniqueRandomNumbers(int lowerBound, int upperBound, int count)
        {
            if (count > upperBound - lowerBound + 1)
            {
                throw new ArgumentException("范围内的数字不足以生成指定个数的不重复随机数。");
            }

            List<int> availableNumbers = new List<int>();
            for (int i = lowerBound; i <= upperBound; i++)
            {
                availableNumbers.Add(i);
            }

            Random random = new Random();
            List<int> result = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int index = random.Next(availableNumbers.Count);
                result.Add(availableNumbers[index]);
                availableNumbers.RemoveAt(index);  // 移除已使用的数字
            }

            return result;
        }
    }
}
