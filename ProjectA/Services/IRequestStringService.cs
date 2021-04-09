using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectA.Services
{
    public interface IRequestStringService
    {
        Task<string> GetStringsFromProjectB();
    }
}
