using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stargripcorp.dataplatform.infra.azure.Exceptions
{
    internal class ApiResponseExceptions(string message) : Exception(message)
    {
    }
}
