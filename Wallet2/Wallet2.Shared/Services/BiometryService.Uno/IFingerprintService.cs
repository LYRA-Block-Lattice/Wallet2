using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BiometryService
{
    public interface IFingerprintService
    {
        Task<bool> Authenticate(CancellationToken ct);
    }
}
