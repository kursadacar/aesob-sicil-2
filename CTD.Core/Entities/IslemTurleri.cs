﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLISLEMTURLERI")]
    public class IslemTurleri : BaseEntity
    {
        public string ISLEMTURU { get; set; }
    }
}