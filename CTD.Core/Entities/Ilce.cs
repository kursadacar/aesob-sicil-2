﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLILCE")]
    public class Ilce : BaseEntity
    {
        public int ID_ESKI { get; set; }
        public int ILID { get; set; }
        public string ILCE { get; set; }
    }
}