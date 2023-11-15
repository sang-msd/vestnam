using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGClothes.Common
{
    public enum OrderStatus
    {
        PENDING,
        PROCESSING,
        TRANSPORTING,
        SUCCESSFUL,
        CANCELLED
    }
}