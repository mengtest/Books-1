using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tip54
{
    class Program
    {
        static void Main()
        {

            //序列化到文件
            Person mike = new Person() { Age = 21, Name = "Mike" };
            mike.NameChanged += new EventHandler(mike_NameChanged);
            BinarySerializer.SerializeToFile(mike, @"c:\", "person.txt");
            Person p = BinarySerializer.DeserializeFromFile<Person>(@"c:\person.txt");
            Console.WriteLine($"序列化到文件 反序列化 1======{p.Age}  {p.Name}");
            p.Name = "Rose";
            Console.WriteLine($"序列化到文件 反序列化 2======{p.Age}  {p.Name}");

            //序列化到字符串
            Person mike1 = new Person() { Age = 21, Name = "Mike" };
            string str = BinarySerializer.Serialize<Person>(mike1);
            Console.WriteLine($"序列化到字符串  1 ====== {str}");

            Person mike2 = BinarySerializer.Deserialize<Person>(str); //反序列化接口报错
            //Console.WriteLine($"反序列化字符串  1 ====== {mike2.Age}  {mike2.Name}");
            Console.ReadKey();
        }

        static void mike_NameChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Name Changed");
        }
    }

    [Serializable]
    class Person
    {
        private string name;
        public int Age { get; set; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (NameChanged != null)
                {
                    NameChanged(this, null);
                }
                name = value;
            }
        }

        public event EventHandler NameChanged;
    }

    //[Serializable]
    //class Person
    //{
    //    private string name;
    //    public string Name
    //    {
    //        get
    //        {
    //            return name;
    //        }
    //        set
    //        {
    //            if (NameChanged != null)
    //            {
    //                NameChanged(this, null);
    //            }
    //            name = value;
    //        }
    //    }

    //    public int Age { get; set; }

    //    [NonSerialized]
    //    private Department department;
    //    public Department Department
    //    {
    //        get
    //        {
    //            return department;
    //        }
    //        set
    //        {
    //            department = value;
    //        }
    //    }

    //    [field: NonSerialized]
    //    public event EventHandler NameChanged;
    //}


    public class BinarySerializer
    {
        //将类型序列化为字符串
        public static string Serialize<T>(T t)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, t);
                return System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        //将类型序列化为文件
        public static void SerializeToFile<T>(T t, string path, string fullName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fullPath = string.Format(@"{0}\{1}", path, fullName);
            using (FileStream stream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, t);
                stream.Flush();
            }
        }

        //将字符串反序列化为类型
        public static TResult Deserialize<TResult>(string s) where TResult : class
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(s);
            using (MemoryStream stream = new MemoryStream(bs))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as TResult;
            }
        }

        //将文件反序列化为类型
        public static TResult DeserializeFromFile<TResult>(string path) where TResult : class
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as TResult;
            }
        }
    }

}
