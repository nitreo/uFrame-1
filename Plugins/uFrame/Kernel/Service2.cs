using System;
using uFrame.Kernel;
using UniRx;
using UnityEngine;
using Zenject;
using Scene = UnityEngine.SceneManagement.Scene;

namespace uFrame2
{
    public class Service2 : MonoBehaviour, IKernelService
    {
        public IEventAggregator EventAggregator { get; set; }

        public void Dispose()
        {
        }

        public void KernelReady()
        {
            Debug.Log(GetType().Name + " SERVICE LOADED");
            Debug.Log(Service.GetType().Name);
        }

        public Scene Scene { get; private set; }

        public void Setup()
        {
            Debug.Log(GetType().Name +" SERVICE SETUP");
        }

        [Inject] public Service1 Service { get; set; }

        [PostInject]
        public void PostInj()
        {
            Debug.Log("POST INJECT!!!");
        }

        public IObservable<Unit> SetupAsync()
        {
            return Observable.Timer(TimeSpan.FromSeconds(2)).Select(_ => Unit.Default).Do(_ =>
            {
                Debug.Log(GetType().Name + " SERVICE SETUP ASYNC");
            });
        }
    }
}