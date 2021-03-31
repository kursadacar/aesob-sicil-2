using System.ComponentModel.DataAnnotations;

namespace CTD.Core.Entities
{
    public class BaseEntity
    {
        [Key] public int Id { get; set; }
    }
}