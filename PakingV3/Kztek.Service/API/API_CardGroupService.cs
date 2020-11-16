using Kztek.Data.Repository;
using Kztek.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Service.API
{
    public interface IAPI_CardGroupService
    {
        IEnumerable<tblCardGroup> GetAll();
    }

    public class API_CardGroupService : IAPI_CardGroupService
    {
        private ItblCardGroupRepository _tblCardGroupRepository;

        public API_CardGroupService(ItblCardGroupRepository _tblCardGroupRepository)
        {
            this._tblCardGroupRepository = _tblCardGroupRepository;
        }

        /// <summary>
        /// get all card group
        /// </summary>
        /// <returns></returns>
        public IEnumerable<tblCardGroup> GetAll()
        {
            var query = from n in _tblCardGroupRepository.Table
                        select n;

            //if (!string.IsNullOrEmpty(AuthCardGroupIds))
            //{
            //    var list = AuthCardGroupIds.Split(';');
            //    query = query.Where(n => list.Contains(n.CardGroupID.ToString()));
            //}

            return query;
        }

    }
}
