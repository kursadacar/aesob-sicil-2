﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLMAKBUZ")]
    public class Makbuz : BaseEntity
    {
        public string ONTAKI { get; set; }
        public int MAKBUZNO { get; set; }
        public int KULLANICI { get; set; }
    }
}