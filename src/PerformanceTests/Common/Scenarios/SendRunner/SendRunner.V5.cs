#if Version5
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

partial class SendRunner
{
    async Task SendLocal(Command msg)
    {
        EndpointInstance.SendLocal(msg);
    }
}
#endif