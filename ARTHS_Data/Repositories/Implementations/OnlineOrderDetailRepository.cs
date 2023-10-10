﻿using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class OnlineOrderDetailRepository : Repository<OnlineOrderDetail>, IOnlineOrderDetailRepository
    {
        public OnlineOrderDetailRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
