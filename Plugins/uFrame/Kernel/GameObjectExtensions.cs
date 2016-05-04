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
            return KernelRegister.GetLoadedKernelForScene(script.gameObject.scene);
        }

        /// <summary>
        /// Ugly method to get name out of the interface for debugging/logging purposes
        /// </summary>
        /// <param name="kernelBeh"></param>
        /// <returns></returns>
        public static string GetName(this IKernelComponent target)
        {
            return (target as MonoBehaviour).gameObject.name;
        }
    }
}