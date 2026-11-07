using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ToyosiAndGIG.Domain.Entities
{
    public class User:IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; } 
        public string LastUpdatedBy { get; set; }
    }
}
