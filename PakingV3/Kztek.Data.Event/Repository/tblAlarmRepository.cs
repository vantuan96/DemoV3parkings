﻿using Kztek.Data.Event.Infrastructure;
using Kztek.Model.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Data.Event.Repository
{
    public class tblAlarmRepository : RepositoryBase<tblAlarm>, ItblAlarmRepository
    {
        public tblAlarmRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface ItblAlarmRepository : IRepository<tblAlarm>
    {

    }
}
