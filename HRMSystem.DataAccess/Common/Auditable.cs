using HRMSystem.DataAccess.Entities;
using System.Runtime.InteropServices;

namespace HRMSystem.DataAccess.Common;

public class Auditable
{
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public User Creator { get; set; }  // Navigation for creator
    public User Modifier { get; set; } // Navigation for modifier
    public int? CreatorId { get; set; }  // Foreign key for creator
    public int? ModifierId { get; set; } // Foreign key for modifier
    public bool IsDeleted { get; set; }

    // Method to set credentials for audit
    protected void SetCredentials(int? adminId)
    {
        if (CreationDate == default)
        {
            CreationDate = DateTime.UtcNow.AddHours(4);
            CreatorId = adminId;
        }
        else
        {
            ModificationDate = DateTime.UtcNow.AddHours(4);
            ModifierId = adminId;
        }
    }
}
