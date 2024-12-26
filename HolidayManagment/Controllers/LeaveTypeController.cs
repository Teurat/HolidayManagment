using HolidayManagment.Data;
using HolidayManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class LeaveTypeController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveTypeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.LeaveTypes.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] LeaveType leaveType)
    {
        if (ModelState.IsValid)
        {
            _context.Add(leaveType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(leaveType);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType == null)
        {
            return NotFound();
        }
        return View(leaveType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] LeaveType leaveType)
    {
        if (id != leaveType.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(leaveType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveTypeExists(leaveType.Id))
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
        return View(leaveType);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var leaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (leaveType == null)
        {
            return NotFound();
        }

        return View(leaveType);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(id);
        _context.LeaveTypes.Remove(leaveType);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LeaveTypeExists(int id)
    {
        return _context.LeaveTypes.Any(e => e.Id == id);
    }
}
    