using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore;
using OBD_API.Models;
using Punch_API.Models;

namespace Punch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTasksController : ControllerBase
    {
        private readonly PunchDbContext _context;

        public WorkTasksController(PunchDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("test")]
        public string Test()
        {
            string test = "test";
            return test;
        }

        // GET: api/WorkTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkTask>>> GetWorkTasks()
        {
            var workTasks = await _context.WorkTasks
                .Include(wt => wt.Company)
                .Where(wt => wt.IsDeprecated == false)
                .GroupBy(wt => wt.Category)
                .ToListAsync();

            var workTaskDTOs = new List<WorkTaskDTO>();


            for (int i = 0; i < workTasks.Count(); i++)
            {
                var workTaskGroup = workTasks[i];
                foreach (WorkTask workTask in workTaskGroup)
                {
                    workTaskDTOs.Add(new WorkTaskDTO
                    {
                        WorkTaskId = workTask.WorkTaskId,
                        Category = workTask.Category,
                        Description = workTask.Description,
                        IsDeprecated = workTask.IsDeprecated,
                        CompanyId = workTask.CompanyId
                    });
                }
            }

            return Ok(workTaskDTOs);
        }

        [HttpGet]
        [Route("by-company/{companyId}")]
        public async Task<ActionResult<IEnumerable<WorkTask>>> GetWorkTasksByCompany(int companyId)
        {
            var workTasks = await _context.WorkTasks
                .Include(wt => wt.Company)
                .Where(wt => wt.IsDeprecated == false && wt.CompanyId == companyId)
                .GroupBy(wt => wt.Category)
                .ToListAsync();

            var workTaskDTOs = new List<WorkTaskDTO>();


            for (int i = 0; i < workTasks.Count(); i++)
            {
                var workTaskGroup = workTasks[i];
                foreach (WorkTask workTask in workTaskGroup)
                {
                    workTaskDTOs.Add(new WorkTaskDTO
                    {
                        WorkTaskId = workTask.WorkTaskId,
                        Category = workTask.Category,
                        Description = workTask.Description,
                        IsDeprecated = workTask.IsDeprecated,
                        CompanyId = workTask.CompanyId
                    });
                }
            }

            return Ok(workTaskDTOs);
        }

        // GET: api/WorkTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkTask>> GetWorkTask(int id)
        {
            var workTask = await _context.WorkTasks.FindAsync(id);

            if (workTask == null)
            {
                return NotFound();
            }

            return workTask;
        }

        [HttpGet]
        [Route("category/{category}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetWorkTaskByCategory(string category)
        {
            var workTasks = await _context.WorkTasks
                .Where(t => t.Category == category)
                .Where(t => t.IsDeprecated == false)
                .ToListAsync();

            var workTaskDTOs = new List<WorkTaskDTO>();

            foreach (WorkTask workTask in workTasks)
            {
                workTaskDTOs.Add(new WorkTaskDTO
                {
                    WorkTaskId = workTask.WorkTaskId,
                    Category = workTask.Category,
                    Description = workTask.Description,
                    CompanyId = workTask.CompanyId,
                    IsDeprecated = workTask.IsDeprecated
                });
            }

            return workTaskDTOs;
        }

        // PUT: api/WorkTasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkTask(int id, WorkTaskDTO workTaskDTO)
        {
            if (id != workTaskDTO.WorkTaskId)
            {
                return BadRequest();
            }
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == workTaskDTO.CompanyId);
            var workTask = new WorkTask
            {
                WorkTaskId = workTaskDTO.WorkTaskId,
                Category = workTaskDTO.Category,
                Description = workTaskDTO.Description,
                IsDeprecated = workTaskDTO.IsDeprecated,
                CompanyId = workTaskDTO.CompanyId,
                Company = company
            };

            _context.Entry(workTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkTaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WorkTasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WorkTask>> PostWorkTask(WorkTaskDTO workTask)
        {
            if (ModelState.IsValid)
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == workTask.CompanyId);
                var newWorkTask = new WorkTask
                {
                    WorkTaskId = workTask.WorkTaskId,
                    Category = workTask.Category,
                    Description = workTask.Description,
                    IsDeprecated = workTask.IsDeprecated,
                    CompanyId = workTask.CompanyId,
                    Company = company
                };
                _context.WorkTasks.Add(newWorkTask);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetWorkTask", new { id = workTask.WorkTaskId }, newWorkTask);
            }
            else
            {
                return BadRequest("A work task object is required");
            }
        }

        // DELETE: api/WorkTasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkTask(int id)
        {
            var workTask = await _context.WorkTasks.FindAsync(id);
            if (workTask == null)
            {
                return NotFound();
            }

            _context.WorkTasks.Remove(workTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkTaskExists(int id)
        {
            return _context.WorkTasks.Any(e => e.WorkTaskId == id);
        }
    }
}
