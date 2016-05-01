using System;
using System.Collections;
using uFrame.Kernel;
using UniRx;
using UnityEngine;
using Zenject;

namespace uFrame2
{
    public class Kernel : MonoBehaviour, IKernel
    {
        private IEventAggregator _eventAggregator;
        private DiContainer _container;
        private IObservable<IKernel> _kernelReadyObservable;
        private IObservable<IKernel> _thisObservable;

        public virtual DiContainer Container { get; set; }

        public virtual IEventAggregator EventAggregator { get; set; }

        public KernelState State { get; set; }

        public string Name
        {
            get { return gameObject.name; }
        }

        public UnityEngine.SceneManagement.Scene UnityScene
        {
            get { return gameObject.scene; }
        }

        private IObservable<IKernel> ThisObservable
        {
            get
            {
                return _thisObservable ?? (_thisObservable = Observable.Return(this as IKernel)); 
            }
        }

        private IObservable<IKernel> KernelReadyObservable
        {
            get
            {
                return _kernelReadyObservable ?? (_kernelReadyObservable = Observable.FromCoroutineValue<IKernel>(WaitForKernelReadyCoroutine)); 
            }
        }

        public void MakePersistent()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IObservable<IKernel> WaitToLoad()
        {
            //If kernel is ready, return immediate observable with this value
            if (State == KernelState.Ready)
                return ThisObservable;

            //Wait for kernel to fully load
            return KernelReadyObservable; //TODO Same here
        }

        /// <summary>
        /// Coroutine which is used to wait for kernel to load
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForKernelReadyCoroutine()
        {
            while (State != KernelState.Ready)
                yield return new WaitForSeconds(0.1f);
            yield return this;
        }

        public virtual void Awake()
        {
            KernelRegister.RegisterKernelForScene(UnityScene, this);
        }

        public virtual void InitializeComponent(IKernelBehaviour kernelComponent)
        {
            KernelRegister.Logger.Log(string.Format("KernelReady @ {0}.{1}", Name, kernelComponent.GetName()));

            kernelComponent.EventAggregator = EventAggregator;
            kernelComponent.KernelLoaded();
        }

        public virtual IObservable<IKernel> LoadKernel()
        {
            KernelRegister.Logger.Log(string.Format("{0} Kernel Loaded", Name ?? gameObject.scene.name));

            State = KernelState.Ready; //TODO Stub
            return ThisObservable;
        }


    }
}