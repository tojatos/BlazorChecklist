using BlazorChecklist.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlazorChecklist.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ChecklistController : ControllerBase
    {
        private readonly ChecklistContext _context;

        public ChecklistController(ChecklistContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Returns list of checklists' names.
        /// </summary>
        /// <response code="200">New checklist inserted.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public List<string> GetListNames() => _context.Checklists.Select(c => c.Name).ToList();

        /// <summary>
        /// Inserts new checklist with a unique name.
        /// </summary>
        /// <response code="201">New checklist inserted.</response>
        /// <response code="400">Checklist name is invalid</response>
        /// <response code="409">Checklist of given name already exists.</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public IActionResult CreateChecklist([FromBody] string checklistName)
        {
            if (string.IsNullOrWhiteSpace(checklistName)) return StatusCode(400);
            if(_context.Checklists.Any(c => c.Name == checklistName)) return StatusCode(409);
            _context.Checklists.Add(new Checklist
            {
                Name = checklistName,
            });
            _context.SaveChanges();

            return StatusCode(201);
        }

        /// <summary>
        /// Remove checklist of given name.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="404">Checklist of given ID does not exist.</response>
        [HttpDelete("{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteChecklist(string name)
        {
            Checklist checklist = _context.Checklists.FirstOrDefault(c => c.Name == name);
            if (checklist == null) return NotFound();

            _context.Checklists.Remove(checklist);
            _context.SaveChanges();
            return Ok();

        }

        /// <summary>
        /// Returns list of checklist items.
        /// </summary>
        /// <response code="200">JSON array of checklist's items.</response>
        [HttpGet("{name}/items")]
        [ProducesResponseType(200)]
        public List<CItem> GetChecklistItems(string name) => _context.Items.ToList().Where(i => i.ChecklistName == name)
            .Select(i => new CItem{Name = i.Name, Checked = i.Checked}).ToList();

        public struct CItem
        {
            public string Name;
            public bool Checked;
        }

        /// <summary>
        /// Inserts new unchecked item to checklist and gives it unique ID.
        /// </summary>
        /// <response code="201">Returns ID of newly added item.</response>
        [HttpPost("{name}/items")]
        [ProducesResponseType(201)]
        public IActionResult CreateChecklistItem([FromBody] string itemName, string name)
        {
            var item = new Item
            {
                ChecklistName = name,
                Name = itemName,
                Checked = false,
            };
            _context.Items.Add(item);
            _context.SaveChanges();

            return StatusCode(201, item.Id);

        }

        /// <summary>
        /// Check or uncheck checklist's item.
        /// </summary>
        /// <response code="202">OK.</response>
        /// <response code="404">Item of given ID does not exist.</response>
        [HttpPatch("{name}/items/{id}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public IActionResult SetItemCheck([FromBody] bool Checked, int id, string name)
        {
            if (!_context.Items.Any(i => i.Id == id && i.ChecklistName == name)) return NotFound();

            _context.Items.First(i => i.Id == id && i.ChecklistName == name).Checked = Checked;
            _context.SaveChanges();

            return StatusCode(202);
        }

        /// <summary>
        /// Remove checklist's item.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="404">Item of given ID does not exist in checklist.</response>
        [HttpDelete("{name}/items/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteItem(int id, string name)
        {
            Item itemToRemove = _context.Items.FirstOrDefault(i => i.Id == id && i.ChecklistName == name);
            if (itemToRemove == null) return NotFound();

            _context.Items.Remove(itemToRemove);
            _context.SaveChanges();

            return StatusCode(202);
        }
    }
}
