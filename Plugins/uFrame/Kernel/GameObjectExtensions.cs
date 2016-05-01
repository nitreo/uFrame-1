using UniRx;
using UnityEngine;

namespace uFrame2
{
    public static class GameObjectExtensions
    {
      
        /// <summary>
        /// Returns observable which is resolved with loaded subkernel or game kernel associated with given monobeh.
        /// Do not invoke on awake, as you are not guaranteed to get a correct/any kernel. 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static IObservable<IKernel> LocateLoadedKernel(this MonoBehaviour script)
        {
            //Ignore this logging crappile
            var args = !string.IsNullOrEmpty(script.gameObject.scene.name) ? script.gameObject.scene.name : "Scene Is Not Saved";
            KernelRegister.Logger.Log(string.Format("Locating kernel for {2} at {1} (0x{0:X8})", script.gameObject.scene.GetHashCode(), args,script.gameObject.name));

            return KernelRegister.GetLoadedKernelForScene(script.gameObject.scene);
        }

        /// <summary>
        /// Ugly method to get name out of the interface for debugging/logging purposes
        /// </summary>
        /// <param name="kernelBeh"></param>
        /// <returns></returns>
        public static string GetName(this IKernelBehaviour kernelBeh)
        {
            var monobeh = kernelBeh as MonoBehaviour;
            return monobeh.gameObject.name;
        }
    }
}