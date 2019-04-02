using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.Api.Models;

namespace Ncs.Prototype.Web.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
 //   [FormatFilter]
    public class PartsApiController : ControllerBase
    {
        private readonly IList<PartsData> PartsDataList;

        public PartsApiController()
        {
            PartsDataList = new List<PartsData>()
            {
                new PartsData()
                {
                    Id=1,
                    Name="Part item 1",
                    Description="This is part description #1"
                },
                new PartsData()
                {
                    Id=2,
                    Name="Part item 2",
                    Description="This is part description #2"
                },
                new PartsData()
                {
                    Id=3,
                    Name="Part item 3",
                    Description="This is part description #3"
                },
                new PartsData()
                {
                    Id=4,
                    Name="Part item 4",
                    Description="This is part description #4"
                },
                new PartsData()
                {
                    Id=5,
                    Name="Part item 5",
                    Description="This is part description #5"
                }
            };
        }

        // GET: api/PartsApi
        [HttpGet]
        [Route("Get/{id}.{format?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(406)]
        [Produces("application/json", "application/xml", "application/html")]
        public IActionResult Get(int id, string format)
        {
            var results = GetResults($"ANONYMOUS (CN/{format})");

            if (!results.Any())
            {
                return NotFound();
            }

            return Ok(results);
        }

        // GET: api/PartsApi/GetAuthorized
        [HttpGet]
        [Authorize]
        [Route("GetAuthorized/{id}.{format?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(406)]
        [Produces("application/json", "application/xml", "application/html")]
        public IActionResult GetAuthorized(int id, string format)
        {
            var results = GetResults($"AUTHORIZED (CN/{format})");

            if (!results.Any())
            {
                return NotFound();
            }

            return Ok(results);
        }

        private IEnumerable<PartsData> GetResults(string stub)
        {
            var results = (from a in PartsDataList select a).ToList();

            results.ForEach(f => f.Description = f.Description.Replace("description", stub, StringComparison.InvariantCultureIgnoreCase));

            return results;
        }
    }
}
