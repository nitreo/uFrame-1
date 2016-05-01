using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using uFrame.Kernel;
using uFrame2;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Scene = UnityEngine.SceneManagement.Scene;

namespace uFrame2
{

    /// <summary>
    /// Use this code to turn your own monobeh classes into IKernelBehaviour.
    /// Literally, implement the interface on your base class and copy-paste the code
    /// </summary>
    public class KernelComponent : MonoBehaviour, IKernelBehaviour
    {

        public IEventAggregator EventAggregator { get; set; }

        public virtual void Start()
        {
            this.LocateLoadedKernel().Subscribe(loadedKernel => loadedKernel.InitializeComponent(this));
        }

        public Scene Scene { get { return gameObject.scene; } } //dreaming of new C# 5.0 => ,right?

        public virtual void KernelLoaded() {  }
    }

}
