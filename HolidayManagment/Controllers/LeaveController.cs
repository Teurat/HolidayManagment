using HolidayManagment.Data;
using HolidayManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class LeaveController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var leaves = _context.Leaves
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .ToListAsync();
        return View(await leaves);
    }

    public IActionResult Create()
    {
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
        ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EmployeeId, LeaveTypeId, DateFrom, DateTo, IsApproved, LeaveDays")] Leave leave)
    {
        if (ModelState.IsValid)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(leave.LeaveTypeId);
            if (leaveType != null)
            {
                var employee = await _context.Employees.FindAsync(leave.EmployeeId);
                if (employee != null)
                {
                    if (leaveType.Id ==1)
                    {
                        employee.AnnualLeaveDays = 28 + employee.ExperienceInCompany;
                        employee.LeaveDaysLeft = employee.AnnualLeaveDays - leave.LeaveDays;
                    }

                    else
                    {
                        employee.AnnualLeaveDays = 0;
                        employee.LeaveDaysLeft = leave.LeaveDays;
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                
            }

            _context.Add(leave);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leave.EmployeeId);
        ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leave.LeaveTypeId);
        return View(leave);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var leave = await _context.Leaves
            .Include(l => l.Employee) 
            .Include(l => l.LeaveType)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
        {
            return NotFound();
        }

        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leave.EmployeeId);
        ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leave.LeaveTypeId);

        return View(leave);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, EmployeeId, LeaveTypeId, DateFrom, DateTo, IsApproved, LeaveDays")] Leave leave)
    {
        if (id != leave.Id)
        {
            return NotFound();
        }

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == leave.EmployeeId);
        var leaveTypeExists = await _context.LeaveTypes.AnyAsync(l => l.Id == leave.LeaveTypeId);

        if (!employeeExists)
        {
            ModelState.AddModelError("EmployeeId", "The selected employee does not exist.");
        }

        if (!leaveTypeExists)
        {
            ModelState.AddModelError("LeaveTypeId", "The selected leave type does not exist.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leave.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leave.LeaveTypeId);
            return View(leave);
        }

        var originalLeave = await _context.Leaves.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

        if (originalLeave == null)
        {
            return NotFound();
        }

        bool hasLeaveTypeChanged = originalLeave.LeaveTypeId != leave.LeaveTypeId;

        var employee = await _context.Employees.FindAsync(leave.EmployeeId);
        var leaveType = await _context.LeaveTypes.FindAsync(leave.LeaveTypeId);

        if (employee != null && leaveType != null)
        {
            if (hasLeaveTypeChanged)
            {
                if (leaveType.Id == 1)
                {
                    employee.AnnualLeaveDays = 28 + employee.ExperienceInCompany;
                    employee.LeaveDaysLeft = employee.AnnualLeaveDays - leave.LeaveDays;  
                }
                else
                {
                    employee.AnnualLeaveDays = 0;  
                    employee.LeaveDaysLeft = leave.LeaveDays;  
                }

                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
        }

        try
        {
            _context.Update(leave);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LeaveExists(leave.Id))
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


    public async Task<IActionResult> Delete(int id)
    {
        var leave = await _context.Leaves
                                  .Include(l => l.Employee)
                                  .Include(l => l.LeaveType)
                                  .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
        {
            return NotFound();
        }

        return View(leave);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var leave = await _context.Leaves.FindAsync(id);

        if (leave == null)
        {
            return NotFound();
        }

        _context.Leaves.Remove(leave);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool LeaveExists(int id)
    {
        return _context.Leaves.Any(e => e.Id == id);
    }
}
