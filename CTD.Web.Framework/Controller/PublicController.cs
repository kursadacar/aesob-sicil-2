﻿using CTD.Data.UnitofWork;

namespace CTD.Web.Framework.Controller
{
    public class PublicController : BaseController
    {
        public PublicController(IUnitofWork uow) : base(uow)
        {
        }
    }
}