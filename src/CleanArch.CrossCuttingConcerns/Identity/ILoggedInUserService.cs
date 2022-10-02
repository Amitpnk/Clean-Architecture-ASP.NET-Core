using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.CrossCuttingConcerns.Identity
{
    public interface ILoggedInUserService
    {
        bool IsAuthenticated { get; }
        string UserId { get;  }
    }
}
