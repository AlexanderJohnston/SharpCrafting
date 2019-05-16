using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HouseofCat.Models;

using System.Runtime.CompilerServices;

namespace SharpCrafting.Win32NT.Utilities
{
  static class ExceptionThrower
  {
    private static ThreadSafeRandomNumberGenerator _random = new ThreadSafeRandomNumberGenerator();

    public static Task ThrowsRandomSystemExceptionAsync()
    {
      switch (_random.Next(0, 3))
      {
        case 0: break; // Fall through for a random exception.
        case 1: break;
        case 2: throw new ArgumentOutOfRangeException(); // Force one exception to show up more often.
      }
      switch (_random.Next(0, 21))
      {
        case 0: throw new Exception();
        case 1: throw new SystemException();
        case 2: throw new ArgumentException();
        case 3: throw new NullReferenceException();
        case 4: throw new AccessViolationException();
        case 5: throw new IndexOutOfRangeException();
        case 6: throw new InvalidCastException();
        case 7: throw new ArgumentNullException();
        case 8: throw new InvalidOperationException();
        case 9: throw new ArgumentOutOfRangeException();
        case 10: throw new InvalidCastException();
        case 11: throw new ArgumentNullException();
        case 12: throw new InvalidOperationException();
        case 13: throw new ArgumentOutOfRangeException();
        case 14: throw new NullReferenceException();
        case 15: throw new NullReferenceException();
        case 16: throw new InvalidCastException();
        case 17: throw new InvalidCastException();
        case 18: throw new NullReferenceException();
        case 19: throw new NullReferenceException();
        case 20: throw new InvalidOperationException();
      }

      return Task.CompletedTask;
    }

    public static async Task ThrowUnsafeExceptionAsync ()
    {
        int    a = 0;
        string b = null;

        var ra = UnsafeLocalRef.Create(ref a);
        var rb = UnsafeLocalRef.Create(ref b);

        a = 1;
        b = "abc";

        Console.WriteLine(ra.CurrentValue);
        Console.WriteLine(rb.CurrentValue);
    }

    public unsafe struct UnsafeLocalRef
    {
        private readonly void*             _ptr;
        private readonly ReadValueDelegate _delegate;

        public object CurrentValue => _delegate(_ptr);

        private UnsafeLocalRef(void* ptr, ReadValueDelegate readDelegate)
        {
            _ptr      = ptr;
            _delegate = readDelegate;
        }

        public static UnsafeLocalRef Create<T>(ref T local)
        {
            return new UnsafeLocalRef(Unsafe.AsPointer(ref local), ptr => Unsafe.Read<T>(ptr));
        }

        private delegate object ReadValueDelegate(void* ptr);
    }
    }
}
