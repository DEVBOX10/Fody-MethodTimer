﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class WithoutInterceptorTests
{
    AssemblyWeaver assemblyWeaver;

    public WithoutInterceptorTests()
    {
        var assemblyPath = Path.GetFullPath(@"..\..\..\AssemblyWithoutInterceptor\bin\Debug\AssemblyWithoutInterceptor.dll");
        assemblyWeaver = new AssemblyWeaver(assemblyPath);
    }

    [Test]
    public void AssertAttributeIsRemoved()
    {
        var type = assemblyWeaver.Assembly.GetType("TimeAttribute");
        Assert.IsNull(type);
    }

    [Test]
    public void CheckErrors()
    {
        Assert.Contains("Method 'System.Void AbstractClassWithAttributeOnMethod::Method()' is abstract but has a [TimeAttribute]. Remove this attribute.", assemblyWeaver.Errors);
        Assert.Contains("Method 'System.Void MyInterface::MyMethod()' is abstract but has a [TimeAttribute]. Remove this attribute.", assemblyWeaver.Errors);
    }

    [Test]
    public void ClassWithAsyncMethod()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithAsyncMethod");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() =>
        {
            var task = (Task) instance.MethodWithAwait();
            task.Wait();
        });

        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("ClassWithAsyncMethod.MethodWithAwait "));
    }



    [Test]
    public void ClassWithExceptionAsyncMethod()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithAsyncMethod");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() =>
        {
            var task = (Task) instance.ComplexMethodWithAwait(-1);
            task.Wait();
        });

        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("ClassWithAsyncMethod.ComplexMethodWithAwait "));
    }

    [Test]
    public void ClassWithFastComplexAsyncMethod()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithAsyncMethod");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() =>
        {
            var task = (Task) instance.ComplexMethodWithAwait(0);
            task.Wait();
        });

        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("ClassWithAsyncMethod.ComplexMethodWithAwait "));
    }

    [Test]
    public void ClassWithMediumComplexAsyncMethod()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithAsyncMethod");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() =>
        {
            var task = (Task) instance.ComplexMethodWithAwait(2);
            task.Wait();
        });

        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("ClassWithAsyncMethod.ComplexMethodWithAwait "));
    }

    [Test]
    public void ClassWithSlowComplexAsyncMethod()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithAsyncMethod");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() =>
        {
            var task = (Task) instance.ComplexMethodWithAwait(100);
            task.Wait();
        });

        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("ClassWithAsyncMethod.ComplexMethodWithAwait "));
    }

    [Test]
    public void ClassWithConstructor()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithConstructor");
        var message = DebugRunner.CaptureDebug(() => Activator.CreateInstance(type));
        Assert.AreEqual(2, message.Count);
        Assert.IsTrue(message[0].StartsWith("ClassWithConstructor.cctor "));
        Assert.IsTrue(message[1].StartsWith("ClassWithConstructor.ctor "));
    }

    [Test]
    public void ClassWithAttribute()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithAttribute");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() => instance.Method());
        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("ClassWithAttribute.Method "));
    }

    [Test]
    public void MethodWithReturnAndCatchReThrow()
    {
        var type = assemblyWeaver.Assembly.GetType("MiscMethods");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() => instance.MethodWithReturnAndCatchReThrow());
        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("MiscMethods.MethodWithReturnAndCatchReThrow "));
    }

    [Test]
    public void ClassWithMethod()
    {
        var type = assemblyWeaver.Assembly.GetType("ClassWithMethod");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() => instance.Method());
        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("ClassWithMethod.Method "));
    }

    [Test]
    public void GenericClassWithMethod()
    {
        var type = assemblyWeaver.Assembly.GetType("GenericClassWithMethod`1[[System.String, mscorlib]]");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() => instance.Method());
        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("GenericClassWithMethod`1.Method "));

    }


    //}
    //[Test]
    //public void MethodWithAwait()
    //{
    //    var message = DebugRunner.CaptureDebug(() =>
    //        {
    //            var type = assemblyWeaver.Assembly.GetType("ClassWithAsyncMethod");
    //            var instance = (dynamic) Activator.CreateInstance(type);
    //            instance.MethodWithAwait();
    //        });
    //    Assert.AreEqual(1, message.Count);
    //    var first = message.First();
    //    Assert.IsTrue(first.StartsWith("ClassWithAsyncMethod.MethodWithAwait "));

    //}

    [Test]
    public void MethodWithReturn()
    {
        var type = assemblyWeaver.Assembly.GetType("MiscMethods");
        var instance = (dynamic) Activator.CreateInstance(type);
        var message = DebugRunner.CaptureDebug(() => instance.MethodWithReturn());
        Assert.AreEqual(1, message.Count);
        Assert.IsTrue(message.First().StartsWith("MiscMethods.MethodWithReturn "));
    }

    [Test]
    public void InheritedClass()
    {
        var type = assemblyWeaver.Assembly.GetType("InheritedClass");
        var message = DebugRunner.CaptureDebug(() => Activator.CreateInstance(type));
        Assert.AreEqual(2, message.Count);
        var second = message.Skip(1).First();
        Assert.IsTrue(second.StartsWith("InheritedClass.ctor "), second);
        var first = message.First();
        Assert.IsTrue(first.StartsWith("BaseClass.ctor "), first);
    }

    [Test]
    public void InheritedClassDoingCall()
    {
        var type = assemblyWeaver.Assembly.GetType("InheritedClassDoingCall");
        var message = DebugRunner.CaptureDebug(() => Activator.CreateInstance(type));
        Assert.AreEqual(3, message.Count);
        Assert.IsTrue(message[0].StartsWith("InheritedClassDoingCall.get_Property "), message[0]);
        Assert.IsTrue(message[1].StartsWith("BaseClass.ctor "), message[1]);
        Assert.IsTrue(message[2].StartsWith("InheritedClassDoingCall.ctor "), message[2]);
    }


    //[Test]
    //public void MethodWithAsyncReturn()
    //{
    //    var message = DebugRunner.CaptureDebug(() =>
    //        {
    //            var type = assemblyWeaver.Assembly.GetType("ClassWithAsyncMethod");
    //            var instance = (dynamic) Activator.CreateInstance(type);
    //            instance.MethodWithReturn();
    //        });
    //    Assert.AreEqual(1, message.Count);
    //    Assert.IsTrue(message.First().StartsWith("ClassWithAsyncMethod.MethodWithReturn "));

    //}

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assemblyWeaver.Assembly.CodeBase.Remove(0, 8));
    }

}