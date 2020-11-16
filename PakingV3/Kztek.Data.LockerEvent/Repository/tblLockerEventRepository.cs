﻿using Kztek.Data.LockerEvent.Infrastructure;
using Kztek.Model.Models.LockerEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Data.LockerEvent.Repository
{
    public class tblLockerEventRepository : RepositoryBase<tblLockerEvent>, ItblLockerEventRepository
    {
        public tblLockerEventRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }

    public interface ItblLockerEventRepository : IRepository<tblLockerEvent>
    {
    }
}
