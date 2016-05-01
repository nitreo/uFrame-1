using uFrame.Kernel;
using UniRx;
using Zenject;

namespace uFrame2
{
    public interface IKernel
    {

        /// <summary>
        /// Lazy-instantiated Zenject container
        /// </summary>
        DiContainer Container { get; set; }

        /// <summary>
        /// Lazy-instantiated Event Aggregator
        /// </summary>
        IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// State of the kernel
        /// </summary>
        KernelState State { get; set; }

        /// <summary>
        /// Any component will be initialized with a kernel on start
        /// </summary>
        /// <param name="kernelComponent"></param>
        void InitializeComponent(IKernelBehaviour kernelComponent);
        
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
}