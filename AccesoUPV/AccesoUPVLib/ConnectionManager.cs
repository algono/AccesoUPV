using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib
{
    public interface ConnectionManager<T>
    {
        bool Connected { get; }

        Task<T> connect();
        Task<T> disconnect();
    }
}
