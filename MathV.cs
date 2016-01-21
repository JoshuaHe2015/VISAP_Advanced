using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathV
{
    public class MathV
    {
        public static string round(string number, int digits, int type)
        {
            //type为0时四舍五入，1为ground，2为ceiling
            int NumOriginLen = number.Length;
            char[] digit_dot = { '.' };
            string[] NumberBroken;
            NumberBroken = number.Split(digit_dot);
            if (digits < 0)
            {
                digits = 0;
            }
            if (NumberBroken[0].Length == NumOriginLen)
            {
                if (digits <= 0)
                {
                    return number;
                }
                else
                {
                    return NumberBroken[0] + ".".PadRight(digits + 1, '0');
                }
            }
            else
            {
                string decimal_part = " ";
                BigNumber zero_point_one = new BigNumber("0.1");
                BigNumber one = new BigNumber("1");
                if (NumberBroken[1].Length > digits)
                {
                    if (type == 1)
                    {
                        decimal_part = NumberBroken[1].Substring(0, digits);
                    }
                    else if (type == 2)
                    {
                        BigNumber carry = new BigNumber(digits.ToString());
                        carry = zero_point_one.Power(carry, 200);
                        BigNumber number_changed = new BigNumber(number);
                        number_changed = number_changed + carry;
                        if (digits <= 0)
                        {
                            return number_changed.ToString().Substring(0, NumberBroken[0].Length);
                        }
                        return number_changed.ToString().Substring(0, NumOriginLen - (NumberBroken[1].Length - digits));
                    }
                    else
                    {
                        if (Convert.ToInt32(NumberBroken[1].Substring(digits, 1)) > 4)
                        {
                            BigNumber carry = new BigNumber(digits.ToString());
                            carry = zero_point_one.Power(carry, 200);
                            BigNumber number_changed = new BigNumber(number);
                            number_changed = number_changed + carry;
                            return number_changed.ToString().Substring(0, NumOriginLen - (NumberBroken[1].Length - digits));
                        }
                        else
                        {
                            decimal_part = NumberBroken[1].Substring(0, digits);
                        }
                    }
                }
                else
                {
                    decimal_part = NumberBroken[1].PadRight(digits, '0');
                }
                if (decimal_part == "")
                {
                    return NumberBroken[0];
                }
                return NumberBroken[0] + '.' + decimal_part;
            }
        }
        public static string NumberPolish(string Number)
        {
            //目标为控制位数在10位
            Number = Number.Trim();
            int CountDecimalPoint = 0;
            int ScientificNumber;
            string ScientificNotation = "";
            int digits_part;
            BigNumber NumberProcess = new BigNumber("0");
            string ProcessedNumber;
            char[] separator = { '.' };
            //用于分割小数点
            string[] NumberParts = new string[2];
            int IsNegative = 0;
            //用来判定数字是否为负
            int zero_count = 0;
            //用于计算小数点后有多少个零
            NumberParts = Number.Split(separator);
            if (NumberParts[0].Length + 5 > 10)
            //四位小数加上1个小数点
            {
                if (Number[0] == '-')
                {
                    IsNegative = 1;
                }
                foreach (char FindDecimalPoint in Number)
                {
                    if (FindDecimalPoint == '.')
                    {
                        CountDecimalPoint = 1;
                        break;
                    }
                }
                if (CountDecimalPoint == 0)
                {
                    //无小数点则只有整数部分
                    ScientificNumber = Number.Length - IsNegative - 1;
                    //转换为科学计数法
                    if (ScientificNumber <= 9 && ScientificNumber > 0)
                    {
                        ScientificNotation = "0" + ScientificNumber.ToString();
                        //如果科学计数法为个位要补零
                    }
                    else
                    {
                        ScientificNotation = ScientificNumber.ToString();
                    }
                    //数字正负无需考虑
                    if (ScientificNotation.Length + 1 + 2 <= 9)
                    {

                        //例如：-3.1415E+05
                        //数字负号占一位，E和科学计数法的符号占两位
                        //大于9的情况不做考虑，数字太大。
                        digits_part = 9 - (ScientificNotation.Length + 1 + 2);
                        //小数位由此算出
                        NumberProcess = new BigNumber(Number.ToString()) / (new BigNumber("10")).Power(new BigNumber(ScientificNumber.ToString()), 30);
                        ProcessedNumber = MathV.round(NumberProcess.ToString(), digits_part, 0) + "E+" + ScientificNotation;
                    }
                    else
                    {
                        //如果已经大于已经大于九位了，则不作处理
                        //这种情况极为罕见，不考虑
                        NumberProcess = new BigNumber(Number.ToString()) / (new BigNumber("10")).Power(new BigNumber(ScientificNumber.ToString()), 30);
                        ProcessedNumber = MathV.round(NumberProcess.ToString(), 0, 0) + "E+" + ScientificNotation;
                    }

                }
                else
                {
                    //有小数点的情况
                    NumberParts = Number.Split(separator);
                    if (NumberParts[0] != "0")
                    {
                        if (NumberParts[0].Length + 1 + 4 >= 9)
                        {
                            //如果保留四位小数之后依旧总长度超过九，则进行如下操作
                            //判断整数部分是否为0
                            //如果不为0，继续科学计数法的处理，舍去小数部分
                            ScientificNumber = NumberParts[0].Length - IsNegative - 1;
                            //转换为科学计数法
                            if (ScientificNumber <= 9 && ScientificNumber > 0)
                            {
                                ScientificNotation = "0" + ScientificNumber.ToString();
                                //如果科学计数法为个位要补零
                            }
                            else
                            {
                                ScientificNotation = ScientificNumber.ToString();
                            }
                            //数字正负无需考虑
                            if (ScientificNotation.Length + 1 + 2 <= 9)
                            {
                                //例如：-3.1415E+05
                                //数字负号占一位，E和科学计数法的符号占两位
                                //大于9的情况不做考虑，数字太大。
                                digits_part = 9 - (ScientificNotation.Length + 1 + 2);
                                //小数位由此算出
                                NumberProcess = new BigNumber(Number.ToString()) / (new BigNumber("10")).Power(new BigNumber(ScientificNumber.ToString()), 30);
                                ProcessedNumber = MathV.round(NumberProcess.ToString(), digits_part, 0) + "E+" + ScientificNotation;
                            }
                            else
                            {
                                //如果已经大于已经大于九位了，则不作处理
                                //这种情况极为罕见，不考虑
                                NumberProcess = new BigNumber(Number.ToString()) / (new BigNumber("10")).Power(new BigNumber(ScientificNumber.ToString()), 30);
                                ProcessedNumber = MathV.round(NumberProcess.ToString(), 0, 0) + "E+" + ScientificNotation;
                            }
                        }
                        else
                        {
                            ProcessedNumber = MathV.round(Number, 4, 0);
                        }
                    }
                    else
                    {
                        zero_count = 0;
                        foreach (char EachDigit in NumberParts[1])
                        {
                            if (EachDigit == '0')
                            {
                                zero_count++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        ScientificNumber = zero_count + 1;
                        //科学计数法的数字为零的个数加1
                        if (ScientificNumber <= 9 && ScientificNumber > 0)
                        {
                            ScientificNotation = "0" + ScientificNumber.ToString();
                            //如果科学计数法为个位要补零
                        }
                        else
                        {
                            ScientificNotation = ScientificNumber.ToString();
                        }
                        if (ScientificNotation.Length + 1 + 2 <= 9)
                        {
                            //例如：-2.654E-05
                            //数字负号占一位，E和科学计数法的符号占两位
                            //大于9的情况不做考虑，数字太大。
                            digits_part = 9 - (ScientificNotation.Length + 1 + 2);
                            //小数位由此算出
                            NumberProcess = new BigNumber(Number.ToString()) * (new BigNumber("10")).Power(new BigNumber(ScientificNumber.ToString()), 30);
                            ProcessedNumber = MathV.round(NumberProcess.ToString(), digits_part, 0) + "E-" + ScientificNotation;
                        }
                        else
                        {
                            //如果已经大于已经大于九位了，则不作处理
                            //这种情况极为罕见，不考虑
                            NumberProcess = new BigNumber(Number.ToString()) * (new BigNumber("10")).Power(new BigNumber(ScientificNumber.ToString()), 30);
                            ProcessedNumber = MathV.round(NumberProcess.ToString(), 0, 0) + "E-" + ScientificNotation;
                        }

                    }
                }
            }
            else
            {
                ProcessedNumber = MathV.round(Number, 4, 0);
                //如果本身长度就在十个单位以内，则不作处理
                //保留四位小数
            }
            if (ProcessedNumber.Length < 11)
            {
                ProcessedNumber = ProcessedNumber.PadLeft(11, ' ');
                //11个单位的长度是为了预留量,以免出现连续进位的情况
            }
            return ProcessedNumber;
        }
        public double MaxDouble(double[] NumberSeries)
        {
            //对Double型数组求最大值
            double MaxValue = NumberSeries[0];
            for (int i = 0; i < NumberSeries.Length; i++)
            {
                if (MaxValue < NumberSeries[i])
                {
                    MaxValue = NumberSeries[i];
                }
            }
            return MaxValue;
        }
        public double MinDouble(double[] NumberSeries)
        {
            //对Double型数组求最小值
            double MinValue = NumberSeries[0];
            for (int i = 0; i < NumberSeries.Length; i++)
            {
                if (MinValue > NumberSeries[i])
                {
                    MinValue = NumberSeries[i];
                }
            }
            return MinValue;
        }
        //行列式值计算
        public static BigNumber MatValue(BigNumber[,] MatrixList, int Level)  //求得|A| 如果为0 说明不可逆
        {

            BigNumber[,] dMatrix = new BigNumber[Level, Level];   //定义二维数组，行列数相同

            for (int i = 0; i < Level; i++)

                for (int j = 0; j < Level; j++)

                    dMatrix[i, j] = MatrixList[i, j];     //将参数的值，付给定义的数组


            BigNumber c, x;
            BigNumber k = new BigNumber("1");

            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {

                if (CompareNumber.Compare(dMatrix[i, j], new BigNumber("0")) == 0)   //判断对角线上的数据是否为0
                {

                    int m = i;

                    for (; CompareNumber.Compare(dMatrix[m, j], new BigNumber("0")) == 0; m++) ;  //如果对角线上数据为0，从该数据开始依次往后判断是否为0

                    if (m == Level)                      //当该行从对角线开始数据都为0 的时候 返回0

                        return new BigNumber("0");

                    else
                    {

                        // Row change between i-row and m-row

                        for (int n = j; n < Level; n++)
                        {

                            c = dMatrix[i, n];

                            dMatrix[i, n] = dMatrix[m, n];

                            dMatrix[m, n] = c;

                        }

                        // Change value pre-value

                        k = k * new BigNumber("-1");

                    }

                }

                // Set 0 to the current column in the rows after current row

                for (int s = Level - 1; s > i; s--)
                {

                    x = dMatrix[s, j];

                    for (int t = j; t < Level; t++)

                        dMatrix[s, t] -= dMatrix[i, t] * (x / dMatrix[i, j]);

                }

            }

            BigNumber sn = new BigNumber("1");

            for (int i = 0; i < Level; i++)
            {

                if (dMatrix[i, i] != new BigNumber("0"))

                    sn *= dMatrix[i, i];

                else

                    return new BigNumber("0");

            }

            return k * sn;

        }
        public static BigNumber[,] MatInv(BigNumber[,] dMatrix, int Level)
        {

            BigNumber dMatrixValue = MatValue(dMatrix, Level);

            if (CompareNumber.Compare(dMatrixValue, new BigNumber("0")) == 0) return null;       //A为该矩阵 若|A| =0 则该矩阵不可逆 返回空


            BigNumber[,] dReverseMatrix = new BigNumber[Level, 2 * Level];

            BigNumber x, c;

            // Init Reverse matrix

            for (int i = 0; i < Level; i++)     //创建一个矩阵（A|I） 以对其进行初等变换 求得其矩阵的逆
            {

                for (int j = 0; j < 2 * Level; j++)
                {

                    if (j < Level)

                        dReverseMatrix[i, j] = dMatrix[i, j];   //该 （A|I）矩阵前 Level列为矩阵A  后面为数据全部为0

                    else

                        dReverseMatrix[i, j] = new BigNumber("0");

                }

                dReverseMatrix[i, Level + i] = new BigNumber("1");


            }



            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {

                if (CompareNumber.Compare(dReverseMatrix[i, j], new BigNumber("0")) == 0)   //判断一行对角线 是否为0
                {

                    int m = i;

                    for (; CompareNumber.Compare(dMatrix[m, j], new BigNumber("0")) == 0; m++) ;

                    if (m == Level)

                        return null;  //某行对角线为0的时候 判断该行该数据所在的列在该数据后 是否为0 都为0 的话不可逆 返回空值

                    else
                    {

                        // Add i-row with m-row

                        for (int n = j; n < 2 * Level; n++)   //如果对角线为0 则该i行加上m行 m行为（初等变换要求对角线为1，0-->1先加上某行，下面在变1）

                            dReverseMatrix[i, n] += dReverseMatrix[m, n];

                    }

                }

                x = dReverseMatrix[i, j];

                if (x != new BigNumber("1"))                  //如果对角线元素不为1  执行以下
                {

                    for (int n = j; n < 2 * Level; n++)

                        if (dReverseMatrix[i, n] != new BigNumber("0"))

                            dReverseMatrix[i, n] /= x;   //相除  使i行第一个数字为1

                }

                // Set 0 to the current column in the rows after current row

                for (int s = Level - 1; s > i; s--)         //该对角线数据为1 时，这一列其他数据 要转换为0
                {

                    x = dReverseMatrix[s, j];

                    for (int t = j; t < 2 * Level; t++)

                        dReverseMatrix[s, t] -= (dReverseMatrix[i, t] * x);

                }

            }

            // Format the first matrix into unit-matrix

            for (int i = Level - 2; i >= 0; i--)

            //处理第一行二列的数据 思路如上 就是把除了对角线外的元素转换为0 
            {

                for (int j = i + 1; j < Level; j++)

                    if (dReverseMatrix[i, j] != new BigNumber("0"))
                    {

                        c = dReverseMatrix[i, j];

                        for (int n = j; n < 2 * Level; n++)

                            dReverseMatrix[i, n] -= (c * dReverseMatrix[j, n]);

                    }

            }

            BigNumber[,] dReturn = new BigNumber[Level, Level];

            for (int i = 0; i < Level; i++)

                for (int j = 0; j < Level; j++)

                    dReturn[i, j] = dReverseMatrix[i, j + Level];

            return dReturn;
        }
        public static BigNumber Double2Big(Double x_bignumber)
        {
            string NumberStr = x_bignumber.ToString().Trim();
            int E_position = -1;
            int IsNegative = 0;
            int ScientificNotation = 0;
            string ScientificNumber;
            BigNumber result = new BigNumber("0");
            string Scientificupper;
            //0为正，1为负
            for (int i = 0; i < NumberStr.Length; i++)
            {
                if (NumberStr[i] == 'E' || NumberStr[i] == 'e')
                {
                    E_position = i;
                }
            }
            if (E_position != -1)
            {
                ScientificNotation = NumberStr.Length - E_position - 1 - 1;
                ScientificNumber = NumberStr.Substring(E_position + 1 + 1, ScientificNotation);
                Scientificupper = NumberStr.Substring(0, E_position - 1);
                if (NumberStr[E_position + 1] == '-')
                {
                    IsNegative = 1;
                    result = new BigNumber("-1") * new BigNumber(Scientificupper) * (new BigNumber("10").Power(new BigNumber(ScientificNumber)));
                    return result;
                }
                else
                {
                    result = new BigNumber("1") * new BigNumber(Scientificupper) * (new BigNumber("10").Power(new BigNumber(ScientificNumber)));
                    return result;
                }

            }
            else
            {
                result = new BigNumber(NumberStr);
                return result;
            }
        }
        public static BigNumber[,] MatPlus(BigNumber[,] mat1, BigNumber[,] mat2)
        {//矩阵加法
            int len11 = mat1.GetLength(0);
            int len12 = mat1.GetLength(1);
            int len21 = mat2.GetLength(0);
            int len22 = mat2.GetLength(1);

            if (len11 == len21 && len12 == len22)
            {
                BigNumber[,] a = new BigNumber[len11, len12];
                for (int i = 0; i < len11; i++)
                {
                    for (int j = 0; j < len12; j++)
                    {
                        a[i, j] = mat1[i, j] + mat2[i, j];
                    }
                }
                return a;
            }
            else
            {
                return null;
            }
        }


        public static BigNumber[,] MatMinu(BigNumber[,] mat1, BigNumber[,] mat2)
        {//矩阵减法
            int len11 = mat1.GetLength(0);
            int len12 = mat1.GetLength(1);
            int len21 = mat2.GetLength(0);
            int len22 = mat2.GetLength(1);

            if (len11 == len21 && len12 == len22)
            {
                BigNumber[,] a = new BigNumber[len11, len12];
                for (int i = 0; i < len11; i++)
                {
                    for (int j = 0; j < len12; j++)
                    {
                        a[i, j] = mat1[i, j] - mat2[i, j];
                    }
                }
                return a;
            }
            else
            {
                return null;
            }
        }

        public static BigNumber[,] MatTimes(BigNumber[,] mat1, BigNumber[,] mat2)
        {    //矩阵乘法
            int len11 = mat1.GetLength(0);
            int len12 = mat1.GetLength(1);
            int len21 = mat2.GetLength(0);
            int len22 = mat2.GetLength(1);
            if (len12 == len21)
            {
                BigNumber[,] a = new BigNumber[len11, len22];
                for (int i = 0; i < len11; i++)
                {
                    for (int j = 0; j < len22; j++)
                    {
                        for (int u = 0; u < len12; u++)
                        {
                            a[i, j] += mat1[i, u] * mat2[u, j];
                        }
                    }
                }
                return a;
            }
            else
            {
                return null;
            }
        }

        public static BigNumber[,] MatTrans(BigNumber[,] mat)
        {
            //矩阵转置
            int len1 = mat.GetLength(0);
            int len2 = mat.GetLength(1);
            BigNumber[,] a = new BigNumber[len2, len1];
            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    a[j, i] = mat[i, j];
                }
            }
            return a;
        }

    }
}
