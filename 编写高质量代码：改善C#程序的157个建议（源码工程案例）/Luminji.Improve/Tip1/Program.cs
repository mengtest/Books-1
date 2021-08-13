using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tip1
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        private static void NewMethod1()
        {
            string s1 = "abc";
            s1 = "123" + s1 + "456";    //以上两行代码创建了3个
            //字符串对象，并执行了一次string.Contact方法
        }

        private static void NewMethod6()
        {
            string re6 = 9 + "456";     //该代码发生一次装箱，并调
            //用一次string.Contact方法
        }

        private static void NewMethod2()
        {
            string re2 = "123" + "abc" + "456"; //该代码等效于 
            //string re2 = "123abc456";
        }

        private static void NewMethod9()
        {
            const string a = "t";
            string re1 = "abc" + a;     //因为a是一个常量，所以
            //该代码等效于 string re1 = "abc" + "t"; 
            //最终等效于string re1 = "abct";
        }

        private static void NewMethod8()
        {
            string a = "t";
            a += "e";
            a += "s";
            a += "t";
        }

        private static void NewMethod7()
        {
            string a = "t";
            string b = "e";
            string c = "s";
            string d = "t";
            string result = a + b + c + d;
        }

        private static void NewMethod10()
        {
            //为了演示必要，定义了4个变量
            string a = "t";
            string b = "e";
            string c = "s";
            string d = "t";
            StringBuilder sb = new StringBuilder(a);
            sb.Append(b);
            sb.Append(c);
            sb.Append(d);
            //再次提示，是运行时，所以没有使用下面的代码
            //StringBuilder sb = new StringBuilder("t");
            //sb.Append("e");
            //sb.Append("s");
            //sb.Append("t");
            string result = sb.ToString();
        }

        private static void NewMethod11()
        {
            //为了演示必要，定义了4个变量
            string a = "t";
            string b = "e";
            string c = "s";
            string d = "t";
            string.Format("{0}{1}{2}{3}", a, b, c, d);
        }


    }
}
