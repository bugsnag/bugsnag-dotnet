using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Bugsnag.Tests
{
  public class ExceptionExtensionsTests
  {
    [Theory]
    [MemberData(nameof(TestData))]
    public void FriendlyMethodNameTest(MethodBase method, string expected)
    {
      Assert.Equal(expected, method.FriendlyMethodName());
    }

    public static IEnumerable<object[]> TestData()
    {
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestGenericMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestGenericMethod<T>(T p1)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestMethod(System.Int32 val)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestMethod2)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestMethod2(System.Int32 val1, System.Single val2, System.String val3)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestPartialGenericMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestPartialGenericMethod<T>(System.Int32 val, T p1)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestGenericReturnTypeMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestGenericReturnTypeMethod<T>()" };

      // unfortunately cannot get this generic argument name correctly. Should be T but will have to make do with System.Int32
      yield return new object[] { typeof(GenericClass<int>).GetMethod(nameof(GenericClass<int>.TestMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+GenericClass<T>.TestMethod(System.Int32 p1)" };
      yield return new object[] { typeof(GenericClass<int>).GetMethod(nameof(GenericClass<int>.TestMultipleGenericMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+GenericClass<T>.TestMultipleGenericMethod<U>(U p2)" };
      yield return new object[] { typeof(GenericClass<int>).GetMethod(nameof(GenericClass<int>.TestVoidMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+GenericClass<T>.TestVoidMethod()" };
    }

    public class NonGenericClass
    {
      public NonGenericClass() { }
      public NonGenericClass(int val) { }

      public void TestGenericMethod<T>(T p1) { }

      public void TestMethod(int val) { }

      public void TestMethod2(int val1, float val2, string val3) { }

      public void TestPartialGenericMethod<T>(int val, T p1) { }

      public T TestGenericReturnTypeMethod<T>() => default(T);
    }

    public class GenericClass<T>
    {
      public GenericClass() { }
      public GenericClass(T val) { }
      public GenericClass(T p, int val) { }

      public void TestMethod(T p1) { }
      public void TestMultipleGenericMethod<U>(U p2) { }
      public void TestVoidMethod() { }
    }
  }
}
