using Jet3UpHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Jobs
{
    /// <summary>
    /// Specific implementation fo a job for Aerotec.
    /// </summary>
    public class AerotecJob : Job
    {
        public MachineTypeEnum MachineType;

        public string HTZ;

        public string Signature;

        public string ANR;

        public string BTIDX;

        public string ControllerId;

        public string? anzahl;
    }
}
