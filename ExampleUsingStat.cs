using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 调用数学类和统计类
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入你要输入数字个数：");
            int n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("请输入一列数，用逗号分隔：");
            string number_series = Console.ReadLine();
            char[] separator = { ',' };
            string[] numbers = number_series.Split(separator);
            BigNumber[] x = new BigNumber[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = new BigNumber(numbers[i]);
            }
            Console.WriteLine(Stat.Mean(x).ToString());
            Stat.Sort(x.Length, x);
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("{0}", x[i]);
            }
            Console.WriteLine("数列的中位数是：{0}", Stat.Quantile(x, 0.5).ToString());
            Console.WriteLine("这列数的方差是：{0}", Stat.Variance(x));
            Console.WriteLine("对方差进行四舍五入，取3位小数：{0}", MathV.round(Stat.Variance(x).ToString(), 3, 0));
            Console.ReadKey();
        }
    }
}
