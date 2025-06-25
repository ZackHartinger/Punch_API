using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBD_API.Models;
using Punch_API.Models;

namespace Punch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeWorkDaysController : ControllerBase
    {
        private readonly PunchDbContext _context;

        public EmployeeWorkDaysController(PunchDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeWorkDays
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeWorkDay>>> GetEmployeeWorkDays()
        {
            return await _context.EmployeeWorkDays
                .Include(e => e.User)
                .Include(e => e.WorkDayTasks)
                .ThenInclude(e => e.WorkTask)
                .ToListAsync();
        }

        [HttpGet]
        [Route("by-company/{companyId}")]
        public async Task<ActionResult<IEnumerable<EmployeeWorkDay>>> GetEmployeeWorkDaysByCompany(int companyId)
        {
            return await _context.EmployeeWorkDays
                .Include(e => e.User)
                .Include(e => e.WorkDayTasks)
                .ThenInclude(e => e.WorkTask)
                .Where(e => e.CompanyId == companyId)
                .ToListAsync();
        }

        [HttpGet("top-5/{id}")]
        [Authorize]
        //[Route("top-5")]
        public async Task<ActionResult<IEnumerable<EmployeeWorkDay>>> Get5MostRecentEmployeeWorkDays(int id)
        {
            return await _context.EmployeeWorkDays
                .Include(e => e.User)
                .Include(e => e.WorkDayTasks)
                .ThenInclude(e => e.WorkTask)
                .Where(e => e.UserId == id)
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeWorkDayDTO>> GetEmployeeWorkDay(int id)
        {
            var employeeWorkDay = await _context.EmployeeWorkDays
                .Include(e => e.User)
                .Include(e => e.WorkDayTasks)
                .ThenInclude(e => e.WorkTask)
                .FirstAsync(e => e.EmployeeWorkDayId == id);

            var employeeWorkDayDTO = new EmployeeWorkDayDTO
            {
                EmployeeWorkDayId = employeeWorkDay.EmployeeWorkDayId,
                CustomerName = employeeWorkDay.CustomerName,
                Date = employeeWorkDay.Date,
                StartTime = employeeWorkDay.StartTime,
                EndTime = employeeWorkDay.EndTime,
                LunchTime = employeeWorkDay.LunchTime,
                LunchDuration = employeeWorkDay.LunchDuration,
                Mileage = employeeWorkDay.Mileage,
                TruckName = employeeWorkDay.TruckName,
                UserId = employeeWorkDay.UserId
            };

            foreach (WorkDayTask workDayTask in employeeWorkDay.WorkDayTasks)
            {
                employeeWorkDayDTO.WorkDayTasks.Add(new WorkTaskDTO
                {
                    WorkTaskId = workDayTask.WorkTask.WorkTaskId,
                    Category = workDayTask.WorkTask.Category,
                    Description = workDayTask.WorkTask.Description,
                    CompanyId = workDayTask.WorkTask.CompanyId,
                    IsDeprecated = workDayTask.WorkTask.IsDeprecated
                });
            }

            if (employeeWorkDay == null)
            {
                return NotFound();
            }

            return employeeWorkDayDTO;
        }

        // GET: api/EmployeeWorkDays/5
        // Gets list of employee workdays for a specific user
        [HttpGet]
        [Route("by-employee/{id}")]
        public async Task<ActionResult<IEnumerable<EmployeeWorkDay>>> GetEmployeeWorkDaysByEmployeeId(int id)
        {
            if (id == null)
            {
                id = 0;
            }
            var employeeWorkDays = await _context.EmployeeWorkDays
                .Include(e => e.User)
                .Include(e => e.WorkDayTasks)
                .ThenInclude(e => e.WorkTask)
                .Where(e => e.UserId == id)
                .ToListAsync();

            if (employeeWorkDays == null)
            {
                return NotFound();
            }

            return employeeWorkDays;
        }

        [HttpGet]
        [Route("filter/{id?}/{date?}/{toDate?}")]
        [Route("filter/{id?}/{date?}")]
        [Route("filter/{id?}")]
        public async Task<ActionResult<IEnumerable<EmployeeWorkDay>>> FilterEmployeeWorkDays(int? id = null, string? date = null, string? toDate = null)
        {
            // ----------- NEEDS ATTENTION -----------------
            // the parsing of dates breaks DRY practices and should be extracted into a utility/helper method so that any future changes will only need to be committed in one place

            // if filtering by id and date range
            if (id != 0 && date != null && toDate != null)
            {
                // Format dates
                string[] dateSplit = date.Split('-');
                string[] toDateSplit = toDate.Split('-');

                // convert and assign string values to ints
                int fromYear = Convert.ToInt32(dateSplit[0]);
                int fromMonth = Convert.ToInt32(dateSplit[1]);
                int fromDay = Convert.ToInt32(dateSplit[2]);

                int toYear = Convert.ToInt32(toDateSplit[0]);
                int toMonth = Convert.ToInt32(toDateSplit[1]);
                int toDay = Convert.ToInt32(toDateSplit[2]);


                var employeeWorkDays = await _context.EmployeeWorkDays
                .Include(e => e.User)
                .Include(e => e.WorkDayTasks)
                .ThenInclude(e => e.WorkTask)
                .Where(e => e.UserId == id)
                .Where(e => e.Date >= new DateOnly(fromYear, fromMonth, fromDay))
                .Where(e => e.Date <= new DateOnly(toYear, toMonth, toDay))
                .OrderBy(e => e.Date)
                .ToListAsync();

                if (employeeWorkDays == null)
                {
                    return NotFound();
                }

                return employeeWorkDays;
            }
            // if filtering by date range only
            else if (date != null && toDate != null)
            {
                // Format dates
                string[] dateSplit = date.Split('-');
                string[] toDateSplit = toDate.Split('-');

                // convert and assign string values to ints
                int fromYear = Convert.ToInt32(dateSplit[0]);
                int fromMonth = Convert.ToInt32(dateSplit[1]);
                int fromDay = Convert.ToInt32(dateSplit[2]);

                int toYear = Convert.ToInt32(toDateSplit[0]);
                int toMonth = Convert.ToInt32(toDateSplit[1]);
                int toDay = Convert.ToInt32(toDateSplit[2]);

                var employeeWorkDays = await _context.EmployeeWorkDays
                .Include(e => e.User)
                .Include(e => e.WorkDayTasks)
                .ThenInclude(e => e.WorkTask)
                .Where(e => e.Date >= new DateOnly(fromYear, fromMonth, fromDay))
                .Where(e => e.Date <= new DateOnly(toYear, toMonth, toDay))
                .OrderBy(e => e.Date)
                .ToListAsync();

                if (employeeWorkDays == null)
                {
                    return NotFound();
                }

                return employeeWorkDays;
            }
            // if filtering by id and date
            else if (id != 0 && date != null)
            {
                // Format dates
                string[] dateSplit = date.Split('-');

                // convert and assign string values to ints
                int fromYear = Convert.ToInt32(dateSplit[0]);
                int fromMonth = Convert.ToInt32(dateSplit[1]);
                int fromDay = Convert.ToInt32(dateSplit[2]);

                var employeeWorkDays = await _context.EmployeeWorkDays
                    .Include(e => e.User)
                    .Include(e => e.WorkDayTasks)
                    .ThenInclude(e => e.WorkTask)
                    .Where(e => e.UserId == id)
                    .Where(e => e.Date == new DateOnly(fromYear, fromMonth, fromDay))
                       .OrderBy(e => e.Date)
                    .ToListAsync();

                if (employeeWorkDays == null)
                {
                    return NotFound();
                }

                return employeeWorkDays;
            }
            // if filtering by id only
            else if (id != 0 && date == null && toDate == null)
            {
                var employeeWorkDays = await _context.EmployeeWorkDays
                   .Include(e => e.User)
                   .Include(e => e.WorkDayTasks)
                   .ThenInclude(e => e.WorkTask)
                   .Where(e => e.UserId == id)
                      .OrderBy(e => e.Date)
                   .ToListAsync();

                if (employeeWorkDays == null)
                {
                    return NotFound();
                }

                return employeeWorkDays;
            }
            // if filtering by date only
            else if (date != null && id == 0 && toDate == null)
            {
                // Format dates
                string[] dateSplit = date.Split('-');

                // convert and assign string values to ints
                int fromYear = Convert.ToInt32(dateSplit[0]);
                int fromMonth = Convert.ToInt32(dateSplit[1]);
                int fromDay = Convert.ToInt32(dateSplit[2]);

                var employeeWorkDays = await _context.EmployeeWorkDays
                   .Include(e => e.User)
                   .Include(e => e.WorkDayTasks)
                   .ThenInclude(e => e.WorkTask)
                   .Where(e => e.Date == new DateOnly(fromYear, fromMonth, fromDay))
                   .OrderBy(e => e.Date)
                   .ToListAsync();

                if (employeeWorkDays == null)
                {
                    return NotFound();
                }

                return employeeWorkDays;
            }
            // no filters selected return all
            else
            {
                return await _context.EmployeeWorkDays
                    .Include(e => e.User)
                    .Include(e => e.WorkDayTasks)
                    .ThenInclude(e => e.WorkTask)
                    .OrderBy(e => e.Date)
                    .ToListAsync();
            }
        }

        [HttpPost]
        [Route("edit")]
        public async Task<ActionResult<EmployeeWorkDayDTO>> EditEmployeeWorkDay(EmployeeWorkDayDTO employeeWorkDay)
        {
            if (ModelState.IsValid)
            {
                if (employeeWorkDay.EmployeeWorkDayId == 0)
                {
                    await PostEmployeeWorkDay(employeeWorkDay);
                }
                else
                {
                    var employeeWorkDayToEdit = await _context.EmployeeWorkDays
                        .Include(e => e.User)
                        .Include(e => e.WorkDayTasks)
                        .ThenInclude(e => e.WorkTask)
                        .FirstAsync(e => e.EmployeeWorkDayId == employeeWorkDay.EmployeeWorkDayId);

                    employeeWorkDayToEdit.CustomerName = employeeWorkDay.CustomerName;
                    employeeWorkDayToEdit.Date = employeeWorkDay.Date;
                    employeeWorkDayToEdit.StartTime = employeeWorkDay.StartTime;
                    employeeWorkDayToEdit.EndTime = employeeWorkDay.EndTime;
                    employeeWorkDayToEdit.LunchDuration = employeeWorkDay.LunchDuration;
                    employeeWorkDayToEdit.LunchTime = employeeWorkDay.LunchTime;
                    employeeWorkDayToEdit.TruckName = employeeWorkDay.TruckName;
                    employeeWorkDayToEdit.Mileage = employeeWorkDay.Mileage;


                    // Create list of currently selected ids to easily compare against the existing object
                    List<int?> selectedWorkTaskIds = new List<int?>();
                    // Fill list with selected ids from the request object
                    foreach (WorkTaskDTO task in employeeWorkDay.WorkDayTasks)
                    {
                        selectedWorkTaskIds.Add(task.WorkTaskId);
                    }

                    // Remove unchecked tasks from existing workday object
                    foreach (WorkDayTask task in employeeWorkDayToEdit.WorkDayTasks)
                    {
                        // cannot compare different object types trying to create a temporary collection to check for changes made to the list to remove tasks that have been deselected
                        if (!selectedWorkTaskIds.Contains(task.WorkTaskId))
                        {
                            employeeWorkDayToEdit.WorkDayTasks.Remove(task);
                            _context.WorkDayTasks.Remove(task);
                        }
                    }

                    // Check for duplicate tasks and update accordingly
                    foreach (WorkTaskDTO task in employeeWorkDay.WorkDayTasks)
                    {
                        var duplicateWorkTask = employeeWorkDayToEdit.WorkDayTasks.Where(t => t.WorkTaskId == task.WorkTaskId).FirstOrDefault();
                        if (duplicateWorkTask == null)
                        {
                            // needs if statements to check for duplicate tasks before adding new workdayTasks
                            employeeWorkDayToEdit.WorkDayTasks.Add(new WorkDayTask { EmployeeWorkDayId = employeeWorkDayToEdit.EmployeeWorkDayId, WorkTaskId = task.WorkTaskId });
                        }
                    }
                    _context.EmployeeWorkDays.Update(employeeWorkDayToEdit);
                }
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/EmployeeWorkDays
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeWorkDay>> PostEmployeeWorkDay([FromBody] EmployeeWorkDayDTO employeeWorkDay)
        {
            var user = await _context.AppUsers.FirstAsync(e => e.Id == employeeWorkDay.UserId);

            var newWorkDay = new EmployeeWorkDay
            {
                CustomerName = employeeWorkDay.CustomerName,
                Date = employeeWorkDay.Date,
                StartTime = employeeWorkDay.StartTime,
                EndTime = employeeWorkDay.EndTime,
                LunchTime = employeeWorkDay.LunchTime,
                LunchDuration = employeeWorkDay.LunchDuration,
                Mileage = employeeWorkDay.Mileage,
                TruckName = employeeWorkDay.TruckName,
                UserId = employeeWorkDay.UserId,
                User = user,
                CompanyId = 1
            };

            _context.EmployeeWorkDays.Add(newWorkDay);
            _context.SaveChanges();

            foreach (WorkTaskDTO task in employeeWorkDay.WorkDayTasks)
            {
                _context.WorkDayTasks.Add(new WorkDayTask { EmployeeWorkDayId = newWorkDay.EmployeeWorkDayId, WorkTaskId = task.WorkTaskId });
            }


            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetEmployeeWorkDay", new { id = employeeWorkDay.EmployeeWorkDayId }, employeeWorkDay);
            return NoContent();
        }

        // DELETE: api/EmployeeWorkDays/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeWorkDay(int id)
        {
            var employeeWorkDay = await _context.EmployeeWorkDays.Include(e => e.WorkDayTasks).FirstAsync(e => e.EmployeeWorkDayId == id);
            if (employeeWorkDay == null)
            {
                return NotFound();
            }

            _context.EmployeeWorkDays.Remove(employeeWorkDay);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeWorkDayExists(int id)
        {
            return _context.EmployeeWorkDays.Any(e => e.EmployeeWorkDayId == id);
        }
    }
}
