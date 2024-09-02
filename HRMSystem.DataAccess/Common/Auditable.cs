using HRMSystem.DataAccess.Entities;

namespace HRMSystem.DataAccess.Common;

public class Auditable
{
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public User? Creator { get; set; }
    public User? Modifier { get; set; }
    public bool IsDeleted { get; set; }

    // Method to set credentials for audit
    public void SetCredentials(User creator)
    {
        if (CreationDate == default)
        {
            CreationDate = DateTime.UtcNow.AddHours(4);
            Creator = creator;
        }
        else
        {
            ModificationDate = DateTime.UtcNow.AddHours(4);
            Modifier = creator;
        }
    }
}
