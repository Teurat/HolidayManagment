using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HolidayManagment.Data;
using HolidayManagment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees.Include(e => e.Company).ToListAsync();
        return View(employees);
    }

    public IActionResult Create()
    {
        ViewBag.Companies = new SelectList(_context.Companies, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName, LastName, CompanyId, FirstEmployment, EmployedInCompany, AnnualLeaveDays, LeaveDaysLeft, IsActive, ExperienceInCompany")] Employee employee)
    {
        if (ModelState.IsValid)
        {
            if (employee.LeaveDaysLeft == null)
            {
                employee.LeaveDaysLeft = 0; 
            }

            if (employee.AnnualLeaveDays == 0)  
            {
                employee.AnnualLeaveDays = 0;  
            }

            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Companies = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
        return View(employee);
    }

    public IActionResult Edit(int id)
    {
        var employee = _context.Employees
                               .Include(e => e.Company)
                               .FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return NotFound();
        }

        ViewBag.Companies = _context.Companies.ToList();
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, FirstName, LastName, CompanyId, FirstEmployment, EmployedInCompany, AnnualLeaveDays, LeaveDaysLeft, IsActive, ExperienceInCompany")] Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }
       
        var companyExists = await _context.Companies.AnyAsync(c => c.Id == employee.CompanyId);
        if (!companyExists)
        {
            ModelState.AddModelError("CompanyId", "The selected company does not exist.");
            ViewBag.Companies = await _context.Companies.ToListAsync();  
            return View(employee);
        }
        if (ModelState.IsValid)
        {
            if (employee.AnnualLeaveDays == 0)
            {
                employee.AnnualLeaveDays = 0;  
            }

            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Companies = await _context.Companies.ToListAsync();  
        return View(employee);
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
                                     .Include(e => e.Company)  
                                     .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);  // Return the employee view to confirm deletion
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Find the employee by Id
        var employee = await _context.Employees
                                     .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        // Optionally, handle leaves if you want to delete them or ensure there are no orphaned records
        // _context.Leaves.RemoveRange(employee.Leaves);  // If leaves should be deleted

        _context.Employees.Remove(employee);  
        await _context.SaveChangesAsync();  

        return RedirectToAction(nameof(Index));  
    }

    // Helper method to check if the employee exists
    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.Id == id);
    }
}
