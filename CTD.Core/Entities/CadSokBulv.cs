﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLCADSOKBULV")]
    public class CadSokBulv : BaseEntity
    {
        public int? ILCEID { get; set; }
        public int? MAHALLEID { get; set; }
        public string CADSOKBULV { get; set; }
    }
}