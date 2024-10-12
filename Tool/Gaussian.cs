namespace AlphalyBot.Tool;

public class Gaussian
{
    private readonly Random rand;

    public Gaussian(int seed)
    {
        rand = new Random(seed);
    }

    //标准正态分布
    public double Normal()
    {
        double s = 0, u = 0;
        while (s is > 1 or 0)
        {
            u = rand.NextDouble() * 2 - 1;
            var v = rand.NextDouble() * 2 - 1;

            s = u * u + v * v;
        }

        var z = Math.Sqrt(-2 * Math.Log(s) / s) * u;
        return z;
    }

    public double RandomNormal(double mu, double sigma)
    {
        var z = Normal() * sigma + mu;
        return z;
    }
}