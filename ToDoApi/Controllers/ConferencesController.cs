﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using NETConfAPI.Models;

namespace NETConfAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ConferencesController : Controller
    {
        private readonly NETConfContext context;

        public ConferencesController(NETConfContext context)
        {
            this.context = context;

            if (this.context.Conferences.Count() == 0)
            {
                Utils.InitContext(context);
            }
        }

        /// <summary>
        /// Get Conferences
        /// </summary>
        [HttpGet]
        public IEnumerable<Conference> Get()
        {
            var items = this.context.Conferences
                            .Include(c => c.Talks)
                            .ThenInclude(d => d.Speaker);

            return items.ToList();
        }

        /// <summary>
        /// Get a Conference
        /// </summary>
        /// <param name="id">The ID of the conference</param>
        [HttpGet("{id}", Name = "GetConferencesById")]
        public IActionResult GetById(int id)
        {
            var item = this.context.Conferences
                            .Include(c => c.Talks)
                            .ThenInclude(d => d.Speaker)
                            .FirstOrDefault(t => t.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        /// <summary>
        /// Create new Conference
        /// </summary>
        /// <param name="item"></param>
        /// <returns>A newly-created Conference</returns>
        /// <response code="201">Returns the newly-created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        public IActionResult Post([FromBody]Conference item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            this.context.Conferences.Add(item);
            this.context.SaveChanges();

            return CreatedAtRoute("GetConferencesById", new { id = item.Id }, item);
        }

        /// <summary>
        /// Update Conference
        /// </summary>
        /// <param name="id">The Conference ID</param>
        /// <param name="updatedItem"></param>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Conference updatedItem)
        {
            if (updatedItem == null || updatedItem.Id != id)
            {
                return BadRequest();
            }

            var item = this.context.Conferences.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = updatedItem.Name;
            item.Year = updatedItem.Year;

            this.context.Conferences.Update(item);
            this.context.SaveChanges();
            return new NoContentResult();
        }

        /// <summary>
        /// Delete Conference
        /// </summary>
        /// <param name="id">The Conference ID</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = this.context.Conferences
                            .Include(c => c.Talks)
                            .FirstOrDefault(t => t.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            // Remove talks
            foreach (var talk in item.Talks)
            {
                this.context.Talks.Remove(talk);
            }

            this.context.Conferences.Remove(item);
            this.context.SaveChanges();
            return new NoContentResult();
        }
    }
}