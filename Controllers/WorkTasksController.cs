using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: api/WorkTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkTask>>> GetWorkTasks()
        {
            return await _context.WorkTasks.ToListAsync();
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
        public async Task<ActionResult<IEnumerable<WorkTask>>> GetWorkTaskByCategory(string category)
        {
            var workTasks = await _context.WorkTasks
                .Where(t => t.Category == category)
                .ToListAsync();

            return workTasks;
        }

        // PUT: api/WorkTasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkTask(int id, WorkTask workTask)
        {
            if (id != workTask.WorkTaskId)
            {
                return BadRequest();
            }

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
        public async Task<ActionResult<WorkTask>> PostWorkTask(WorkTask workTask)
        {
            _context.WorkTasks.Add(workTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkTask", new { id = workTask.WorkTaskId }, workTask);
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
