using System.ComponentModel.DataAnnotations;

namespace HRMSystem.DataAccess.Common;

public class EntityBase : Auditable
{
    [Key]
    public int Id { get; set; }
}