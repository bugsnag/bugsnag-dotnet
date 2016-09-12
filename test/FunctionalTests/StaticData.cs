using System;

namespace Bugsnag.Test.FunctionalTests
{
    /// <summary>
    /// Contains static data used in functional tests
    /// </summary>
    public static class StaticData
    {
        public const string TestApiKey = "ABCDEF1234567890ABCDEF1234567890";

        // Custom created exceptions used for testing
        public static readonly SystemException TestCreatedException;
        public static readonly RankException TestThrowException;
        public static readonly ArithmeticException TestInnerException;
        public static readonly TimeoutException TestCallStackException;
        public static readonly DivideByZeroException TestInnerNoStackException;

        static StaticData()
        {
            // Exception created but is not thrown
            TestCreatedException = new SystemException("System Test Exception");

            // Traditional created and throw exception
            try
            {
                throw new RankException("Rank Test");
            }
            catch (RankException e)
            {
                TestThrowException = e;
            }

            // Exception containing inner exceptions
            try
            {
                try
                {
                    try
                    {
                        throw new TypeAccessException("Test Type Exception");
                    }
                    catch (TypeAccessException exp1)
                    {
                        throw new DivideByZeroException("Divide By Zero Test", exp1);
                    }
                }
                catch (DivideByZeroException exp2)
                {
                    throw new ArithmeticException("Inner Exception Test", exp2);
                }
            }
            catch (ArithmeticException exp3)
            {
                TestInnerException = exp3;
            }

            // Exception with a defined stack trace
            var callClass = new TestNamespace.ClassAlpha();
            try
            {
                callClass.ThrowException();
            }
            catch (TimeoutException exp)
            {
                TestCallStackException = exp;
            }

            // Exception containing an inner exception with no stack trace
            try
            {
                try
                {
                    var rootExp = new OperationCanceledException("Operation Cancelled Test");
                    throw new TimeoutException("Timeout test exception", rootExp);
                }
                catch (TimeoutException exp1)
                {
                    throw new DivideByZeroException("Divide By Zero Test", exp1);
                }
            }
            catch (DivideByZeroException exp2)
            {
                TestInnerNoStackException = exp2;
            }
        }
    }
}

// Create a custom namespace so that the associated call stack is not identified
// as a Bugsnag related frame
namespace TestNamespace
{
    public class ClassAlpha
    {
        public void ThrowException()
        {
            var beta = new ClassBeta();
            beta.ThrowException();
        }
    }

    public class ClassBeta
    {
        public void ThrowException()
        {
            var gamma = new ClassGamma();
            gamma.ThrowException();
        }
    }

    public class ClassGamma
    {
        public void ThrowException()
        {
            throw new TimeoutException("Test Timeout Exception");
        }
    }
}
