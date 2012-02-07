using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Crypt
{
    public static class FirstClass
    {
        static public double timeToDo;
        public static long SimpleNumberTest(long number)
        {
            DateTime start = DateTime.Now;
            if (number <= 1)
                throw new Exception("Invalid argument. It must be greater then 1");
            long k = 2;
            while (k <= Math.Sqrt(number) + 1)
            {
                long d = GreatesCommonDivisor(number, k);
                if (d > 1)
                {
                    TimeSpan t = DateTime.Now - start;
                    timeToDo = t.Ticks;
                    return k;
                }
                k++;
            }
            TimeSpan t2 = DateTime.Now - start;
            timeToDo = t2.Ticks;

            return 1;
        }

        public static long gcd(long a, long b)
        {
            DateTime start = DateTime.Now;
            while (b != 0)
            {
                long d = a / b;
                long tmp = b;
                b = a - b * d;
                a = tmp;
            }
            TimeSpan t = DateTime.Now - start;
            timeToDo = t.Ticks;
            return a;
        }

        public static long GreatesCommonDivisor(long number1, long number2)
        {
            DateTime start = DateTime.Now;
            long a = number1, b = number2, c;
            while (b != 0) { c = a % b; a = b; b = c; }
            TimeSpan t = DateTime.Now - start;
            timeToDo = t.Ticks;
            return Math.Abs(a);
        }

        public static long pollard(long P, long B)
        {
            Func<long, long> f = (x => Convert.ToInt64((Math.Pow(x, 2))) % P);
            Random r = new Random();
            long x0 = B, x1;
            long y0 = x0, y1;
            long d;
            do
            {
                x1 = f(x0);
                y1 = f(f(y0));
                x0 = x1;
                y0 = y1;
                d = GreatesCommonDivisor(Math.Abs(x1 - y1), P);
                if (d == P)
                    return 0;
            } while (d == 1);
            return d;
        }

        public static long phi(long n)
        {
            long ret = 1, p;
            for (p = 2; p * p <= n; p++)
            {
                if (n % p == 0)
                {
                    n /= p;
                    while (n % p == 0)
                    {
                        n /= p;
                        ret *= p;
                    }
                    ret *= p - 1;
                }
            }
            return n > 1 ? ret * (n - 1) : ret;
        }

        public static long[] Primes(long n)
        {
            List<long> primes = new List<long>();
            for (long p = 2; p * p <= n; p++)
            {
                if (n % p == 0)
                {
                    n /= p;
                    primes.Add(p);
                    while (n % p == 0)
                    {
                        n /= p;
                        primes.Add(p);
                    }
                }
            }
            primes.Add(n);
            return primes.ToArray();
        }

        public static bool pasd(long p, long g)
        {
            return Primes(p - 1).Distinct().ToList().FindAll(p1 => PowMod(g, (p - 1) / p1, p) == 1).Count == 0;
        }

        public static bool QuadraticResidue(long x, long p)
        {
            return PowMod(x, (p - 1) / 2, p) == 1;
        }

        public static long SqrtMod(long x, long p)
        {
            //p = 4m + 3
            if ((p - 3) % 4 == 0)
            {
                long m = (p - 3) / 4;
                return PowMod(x, m + 1, p);
            }
            //p = 8m + 5
            if ((p - 5) % 8 == 0)
            {
                long m = (p - 5) / 8;
                if (PowMod(x, 2 * m + 1, p) == p-1)
                {
                    return PowMod(x, m + 1, p) * PowMod(2, 2 * m + 1, p);
                }
                else
                {
                    return PowMod(x, m + 1, p);
                }
            }
            //p = 8m + 1
            if ((p - 1) % 8 == 0)
            {
                long t = (p - 1) / 8;
                long s = 3;
                while (t % 2 == 0)
                {
                    t /= 2;
                    s++;
                }
                int a;
                for (a = 2; a < p; a++)
                {
                    if (!QuadraticResidue(a, p))
                        break;
                }
                if (PowMod(x,Convert.ToInt64(Math.Pow(2, s - 2) * t), p) == p - 1)
                {
                    long dt = 1;
                    long modX = PowMod(x, Convert.ToInt64(Math.Pow(2, s - 3) * t), p);
                    while ((PowMod(a, Convert.ToInt64(Math.Pow(2, s - 2) * dt * t), p) * modX) % p == p - 1)
                    {
                        //Console.WriteLine((PowMod(a, Convert.ToInt64(Math.Pow(2, s - 2) * dt * t), p) * modX) % p + " " + dt);
                        //если а в той степени, что там в вайле == -1, то умножаем всю эту штуку на a в степени 2^(s-1) * 
                        //а это тоже самое, что добавить к коэффициенту при t двойку
                        //можешь проверить, делаем это пока не будет +1, хотя поидее 1 иттерации хватит
                        dt += 2;
                    }
                    Console.WriteLine("{0}^{1}*{2}^{3}",x,(Math.Pow(2, s - 3) * t + 1) / 2,a,Math.Pow(2, s - 3) * dt*t);
                    return (PowMod(x, Convert.ToInt64((Math.Pow(2, s - 3) * t + 1) / 2), p) * PowMod(a, Convert.ToInt64(Math.Pow(2, s - 3) * dt* t), p)) % p;
                }
                return -1;
            }
            return 0;
        }

        public static long PowMod(long x, long e, long n)
        {
            long r = 1;
            while (e > 0)
            {
                if ((e % 2) == 1) { r = (r * x) % n; }
                e = e / 2;
                x = (x * x) % n;
            }
            return r;
        }
    }
}
