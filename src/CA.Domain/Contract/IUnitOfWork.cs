using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Domain.Contract
{
    public interface IUnitOfWork
    {
        int SaveChanges();

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void CommitTransaction();
    }
}
