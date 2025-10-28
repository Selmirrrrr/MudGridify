namespace Selmir.MudGridify.Playground.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public double Salary { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public int Age { get; set; }
}
