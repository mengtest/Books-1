using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

/*
  在.Net中每次使用new操作符创建对象时，CLR都会为改对象在对上分配内存，一旦这些对象不再被引用，就会回收它们的内存

  对于没有继承IDisposable接口的类型对象，垃圾回收器则会直接释放对象所占用的内存，

  对于实现了Dispose模式的类型，在每次创建的时候，CLR都会将该对象的做一个指针放到终结列表中，垃圾回收器在回收内存之前，会首先将终结列表中的指针
  放到一个frechable队列中，同时CLR还会分配专门的线程读取frechable队列，并调用对象的终结器（析构方法）,只有这个时候对象才会真正被标识为垃圾，
  并且在下一次进行垃圾回收时释放对象占用的内存 

  总结: 实现Dispose模式的类型对象，起码要经过2次垃圾回收才能真正被回收掉，基于这个特点，如果我们的类型提供了显示是否的方法来减少一次垃圾回收，同时也可以
        终结器中提示隐式清理，以避免调用者忘记调用该方法带来的资源泄漏

    if(对象不存在引用)
    {
        if(进行垃圾回收)
       {
            if(未实现Dispose)
            {
                直接释放对象所占用的内存
            }
            else
            {
                1 把终结列表中的指针放入队列里
                2 在子线程中去读该队列
                3 调用对象的析构方法，并且标记为该对象可回收
                4 下一次垃圾回收时进行对象占用内存的释放
            }
        }
    }



   
 */


/*
    垃圾回收执行满足条件，以下其一就能触发
    1 系统具有低的物理内存
    2 由托管对上已分配的对象使用内存超过了可接受范围
    3 主动调用GC.Collect时

    垃圾回收机制中"代"的概念
    {
        一共分为3代（0代,1代,2代）,第0代包含一些短期生成的对象，
    }
    
 */


/*
    在CLR托管的应用程序中，存在一个“根”的概念，类型的静态字段，方法参数以及局部变量都可以作为“根”的存在(值类型不能作为“根”，只要引用类型的指针才能作为"根")
    在一次垃圾回收中，垃圾回收器会沿着线程栈上行检查"根"的存在（例如 有两个局部变量a，b，b后被定义，b会优先被检查），当检查到方法内的"根"时，付过发现没有任何一个地方
    引用了局部变量，则不管是否已经显式将其赋值为null，都意味着该"根"已经被停止，然后垃圾回收器发现该根的引用为空，同时标记该根可以被释放，所以方法内的局部变量以及方法参数
    显式置为null是没有意义的，更进一步在release模式下，jit编译器会忽略相关语句（s = null）

    类型的静态字段必须置为null，因为静态字段一旦被创建，该“根”就一直存在，所以垃圾回收器始终不会认为它是一个垃圾
    可以在类的析构方法里显式置为null，
    在实际工作中，一段感觉到自己的静态引用类型参数占用的内存空间比较大，并且用完后就不会再使用了，遍可立即将其赋值为null

    建议：少用静态字段


     
*/
namespace Tip46
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (SampleClass c1 = new SampleClass())
            //{
            //    //SampleClass继承IDisposable 当脱离using范围内，会自动执行c1的Dispose方法
            //}
        }
    }

    public class SampleClass : IDisposable
    {
        //演示创建一个非托管资源
        //Marshal.AllocHGlobal ==>通过使用指定的字节数，从进程的非托管内存中分配内存
        /*
            100就是100个字节
            hglobal是这100个字节的第一个字节的地址
            两个方法相当于c里面的malloc, free
            10

            H = heap 堆
            Global = 全局
            Alloc = Allocate 分配 
        */
       
        private IntPtr nativeResource = Marshal.AllocHGlobal(100);
        //演示创建一个托管资源
        private AnotherResource managedResource = new AnotherResource();
        private bool disposed = false;

        /// <summary>
        /// 实现IDisposable中的Dispose方法
        /// </summary>
        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器） 
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 不是必要的，提供一个Close方法仅仅是为了更符合其他语言（如
        /// C++）的规范
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 必须，防止程序员忘记了显式调用Dispose方法  析构的时候会被调用，而析构又是被GC触发的
        /// </summary>
        ~SampleClass()
        {
            //必须为false
            Dispose(false);
        }

        /// <summary>
        /// 非密封类修饰用protected virtual
        /// 密封类修饰用private
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                // 清理托管资源
                if (managedResource != null)
                {
                    managedResource.Dispose();
                    managedResource = null;
                }
            }
            // 清理非托管资源
            if (nativeResource != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(nativeResource);
                nativeResource = IntPtr.Zero;
            }
            //让类型知道自己已经被释放
            disposed = true;
        }

        public void SamplePublicMethod()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("SampleClass", "SampleClass is disposed");
            }
            //省略
        }
    }

    class AnotherResource : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
