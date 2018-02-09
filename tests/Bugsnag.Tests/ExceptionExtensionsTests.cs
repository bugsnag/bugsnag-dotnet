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
      Assert.Equal(expected, new Bugsnag.Payload.Method(method).DisplayName());
    }

    public static IEnumerable<object[]> TestData()
    {
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestGenericMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestGenericMethod<T>(T p1)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestMethod(int val)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestMethod2)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestMethod2(int val1, float val2, string val3)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestPartialGenericMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestPartialGenericMethod<T>(int val, T p1)" };
      yield return new object[] { typeof(NonGenericClass).GetMethod(nameof(NonGenericClass.TestGenericReturnTypeMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+NonGenericClass.TestGenericReturnTypeMethod<T>()" };

      // can't get the method parameter to be T due to the fact that we get the
      // methodinfo from the closed generic type and it's tricky/impossible to
      // resolve back to the open generic type and retrieve the same method info
      yield return new object[] { typeof(GenericClass<int>).GetMethod(nameof(GenericClass<int>.TestMethod)), "Bugsnag.Tests.ExceptionExtensionsTests+GenericClass<T>.TestMethod(int p1)" };
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
