using Microsoft.AspNetCore.Mvc.Rendering;

namespace HolidayManagment.Models
{
    public class Employee
    {
        
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public int CompanyId { get; set; }
        public DateTime FirstEmployment { get; set; }
        public DateTime EmployedInCompany { get; set; }

        public Company? Company { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public int? AnnualLeaveDays { get; set; }
        public int? LeaveDaysLeft { get; set; }
        public bool IsActive { get; set; }
        public int? ExperienceInCompany {  get; set; }

        //public ICollection<Leave>? Leaves { get; set; }
    }
}
