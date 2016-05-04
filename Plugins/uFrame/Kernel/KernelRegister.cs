using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using uFrame.Kernel;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace uFrame2
{
    public static class KernelRegister
    {

        public const string ZENJECT_VERSION = "v3.10"; //TODO find a way to locate the versions automatically ?
        public const string UNIRX_VERSION = "v5.2.0";
        public const string KERNEL_SCENE_KEY = "KERNEL_SCENE";

        private static Dictionary<UnityEngine.SceneManagement.Scene, IKernel> _sceneKernels;
        private static IEventAggregator SharedEventAggregator = new EventAggregator();
        private static string _rootKernelScene;
        private static Logger _logger;
        private static string _editorKernelScene;
        private static IObservable<IKernel> _readyKernelObservable;
        private static IObservable<IKernel> _nullRootKernelObservable;
        private static IObservable<IKernel> _waitForRootKernelLoadCoroutine;


        static KernelRegister()
        {
            Logger.Log("Using Zenject " + ZENJECT_VERSION);
            Logger.Log("Using UniRX " + UNIRX_VERSION);
            Logger.Log(string.Format("Selected Kernel: \"{0}\"", RootKernelScene));

            if (!Application.CanStreamedLevelBeLoaded(RootKernelScene))
            {
                Logger.LogWarning("Kernel Registry", string.Format("Kernel Scene {0} cannot be loaded and will be ignored. Is it added to build settings?", RootKernelScene));
                ForceIgnoreRootKernel = true;
            }

        }

        public static bool IsRootKernelLoadingOrReady { get; set; }

        /// <summary>
        /// This flag will be used to force ignore root kernel if such kernel could not be loaded
        /// </summary>
        public static bool ForceIgnoreRootKernel { get; set; }

        public static Logger Logger
        {
            get { return _logger ?? (_logger = new Logger(new TemproraryLoggerHandler())); }
            set { _logger = value; }
        }

        /// <summary>
        /// PlayerPrefs-bound property which sets the Root Kernel Scene
        /// </summary>
        public static string RootKernelScene
        {
            get { return PlayerPrefs.GetString(KERNEL_SCENE_KEY, null); }
            set { PlayerPrefs.SetString(KERNEL_SCENE_KEY, value); }
        }

        /// <summary>
        /// Scene -> Kernel map
        /// </summary>
        public static Dictionary<UnityEngine.SceneManagement.Scene, IKernel> SceneKernels
        {
            get { return _sceneKernels ?? (_sceneKernels = new Dictionary<UnityEngine.SceneManagement.Scene, IKernel>()); }
            set { _sceneKernels = value; }
        }

        public static bool ShouldUseRootKernel {
            get
            {
                return !string.IsNullOrEmpty(RootKernelScene) && Application.CanStreamedLevelBeLoaded(RootKernelScene) && !ForceIgnoreRootKernel;
            }
        }

        public static bool IsRootKernelLoaded
        {
            get { return ShouldUseRootKernel && RootKernel != null && RootKernel.State == KernelState.Ready; }
        }

        /// <summary>
        /// This method will load root kernel IF NEEDED and IF POSSIBLE
        /// </summary>
        public static void LoadRootKernelIfNeeded()
        {
            if(!ShouldUseRootKernel || IsRootKernelLoadingOrReady) return;

            IsRootKernelLoadingOrReady = true;
            SceneManager.LoadSceneAsync(RootKernelScene, LoadSceneMode.Additive);
        }

        /// <summary>
        /// This method tells kernel framework to use specific kernel for a given scene.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="kernel"></param>
        public static void RegisterKernelForScene(UnityEngine.SceneManagement.Scene scene, IKernel kernel)
        {
            // Touch the root kernel, in case it has not been done yet.
            // Avoid if invoked from root kernel
            if(scene.name != RootKernelScene) LoadRootKernelIfNeeded();

            // Map/Remap scene to kernel
            SceneKernels[scene] = kernel;

            // Retreat if kernel is already loaded
            if (kernel.State == KernelState.Ready) return;

            // All kernels share the only one instance of EventAggregator
            kernel.EventAggregator = SharedEventAggregator; 

            // if scene matches
            if (scene.name == RootKernelScene)
            {
                IsRootKernelLoadingOrReady = true;
                RootKernel = kernel;
                //Use shared DIContainer  as Root DiContianer
                kernel.Container = new DiContainer();
                kernel.MakePersistent();
                kernel.LoadKernel();
            }
            else //Delay loading of any other kernel
            {
                WaitForRootKernelLoad().Subscribe(rootKernel => //Delay loading of scene kernel
                {
                    kernel.Container = rootKernel != null ? new DiContainer(rootKernel.Container) : new DiContainer(); //Since RootKernel is used, allocate child container
                    kernel.LoadKernel();
                });
            }

        }

        private static IObservable<IKernel> WaitForRootKernelLoad()
        {
            return ReadyKernelObservable;
        }

        private static IObservable<IKernel> ReadyKernelObservable
        {
            get
            {
                if (IsRootKernelLoaded)
                    return _readyKernelObservable ?? (_readyKernelObservable = Observable.Return(RootKernel));

                if (!ShouldUseRootKernel)
                    return _nullRootKernelObservable ?? ( _nullRootKernelObservable = Observable.Return<IKernel>(null));

//                if (!IsRootKernelLoaded)
                    return _waitForRootKernelLoadCoroutine ??
                    (_waitForRootKernelLoadCoroutine = Observable.FromCoroutineValue<IKernel>(WaitForRootKernelLoadCoroutine));

            }
            set { _readyKernelObservable = value; }
        }

        private static IEnumerator WaitForRootKernelLoadCoroutine()
        {
            while (!IsRootKernelLoaded)
                yield return new WaitForSeconds(0.1f);

            yield return RootKernel;
        }

        private static IKernel RootKernel { get; set; }

        public static void UnRegisterKernelForScene(UnityEngine.SceneManagement.Scene scene, IKernel kernel)
        {
            SceneKernels.Remove(scene);
        }

        public static IObservable<IKernel> GetLoadedKernelForScene(UnityEngine.SceneManagement.Scene scene)
        {
            LoadRootKernelIfNeeded();

            return WaitForRootKernelLoad().ContinueWith(rootKernel =>
            {
                IKernel kernel = rootKernel; //Default kernel
                if (SceneKernels.ContainsKey(scene)) kernel = SceneKernels[scene]; //Remap if scene kernel is available
                Assert.IsNotNull(kernel, "No root kernel was found and no matching scene kernel was detected.");
                return kernel.WaitToLoad();
            });

        }
    }
}