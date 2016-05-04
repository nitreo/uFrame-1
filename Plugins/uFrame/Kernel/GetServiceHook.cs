using System.Collections.Generic;
using UnityEngine;

namespace uFrame2
{
    public class GetServiceHook : MonoBehaviour, IGetServicesHook
    {
        public void Dispose()
        {
            
        }

        public void GetServices(IList<IKernelService> services)
        {
        }
    }
}