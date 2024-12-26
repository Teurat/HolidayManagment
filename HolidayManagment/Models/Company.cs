namespace HolidayManagment.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int NrEmployees { get; set; }
        public int FoundationYear { get; set; }
        //public ICollection<Employee> Employees { get; set; }
    }
}
