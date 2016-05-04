using System;
using uFrame.Kernel;
using UniRx;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

namespace uFrame2
{
    public class Service1 : MonoBehaviour, IKernelService
    {
        public IEventAggregator EventAggregator { get; set; }
        public void Dispose()
        {
        }

        public void KernelReady()
        {
            Debug.Log(GetType().Name + " SERVICE LOADED");

        }

        public Scene Scene { get; private set; }


        public void Setup()
        {
            Debug.Log(GetType().Name +" SERVICE SETUP");
        }

        public IObservable<Unit> SetupAsync()
        {
            return Observable.Timer(TimeSpan.FromSeconds(4)).Select(_ => Unit.Default).Do(_ =>
            {
                Debug.Log(GetType().Name + " SERVICE SETUP ASYNC");
            });
        }
    }
}