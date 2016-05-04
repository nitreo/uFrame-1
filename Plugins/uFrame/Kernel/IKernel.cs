using System;
using System.Collections;
using System.Collections.Generic;
using uFrame.Kernel;
using UniRx;
using UnityEngine;
using UnityEngineInternal;
using Zenject;

namespace uFrame2
{
    public interface IKernel : IEventsAware
    {

        DiContainer Container { get; set; }

        /// <summary>
        /// State of the kernel
        /// </summary>
        KernelState State { get; set; }

        /// <summary>
        /// Any component will be initialized with a kernel on start
        /// </summary>
        /// <param name="kernelComponent"></param>
        void InitializeComponent(IKernelComponent kernelComponent);
        
        /// <summary>
        /// Any component will be initialized with a kernel on start
        /// </summary>
        /// <param name="kernelComponent"></param>
        IObservable<IKernel> LoadKernel();

        /// <summary>
        /// Use this method to wait for kernel to load
        /// Resolved immediately if kernel is already loaded
        /// </summary>
        /// <returns></returns>
        IObservable<IKernel> WaitToLoad();

        /// <summary>
        /// Detaches kernel from any scene and makes it persist between scene loads
        /// </summary>
        void MakePersistent();

        /// <summary>
        /// Name of the kernel for debugging purposes
        /// </summary>
        string Name { get; }
    }

    public interface IEventsAware
    {
        IEventAggregator EventAggregator { get; set; }
    }

    public interface IKernelService : IKernelReadyHook, IEventsAware
    {
        /// <summary>
        /// </summary>
        void Setup();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        IObservable<Unit> SetupAsync();

    }

    public interface LoadHook : IDisposable
    {
        void BeforeLoad();
        void AfterLoad();
    }

    public interface IIoCHook : IDisposable
    {
        void BeforeIoCContainerSet(DiContainer container);
        void AfterIoCContainerSet(DiContainer container);
    }

    public interface IKernelReadyHook : IDisposable
    {
        void KernelReady();
    }

    public interface IGetServicesHook : IDisposable
    {
        void GetServices(IList<IKernelService> services);
    }

    public interface IInitializeComponentHook : IDisposable
    {
        void InitializeComponent();
        IObservable<IKernelComponent> InitializeComponentAsync();
    }

}