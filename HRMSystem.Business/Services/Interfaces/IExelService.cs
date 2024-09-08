using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMSystem.Business.Services.Interfaces
{
    public interface IExelService
    {
        public Task<byte[]> ExportWorkers();
    }
}
