﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLIL")]
    public class Il : BaseEntity
    {
        public string SEHIR { get; set; }
    }
}