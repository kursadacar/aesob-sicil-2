﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLNACE")]
    public class Nace : BaseEntity
    {
        public string NACE { get; set; }
        public string TANIMI { get; set; }
        public int? MESLEKID { get; set; }
    }
}