using System;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using VillaAPI.Logging;

namespace VillaAPI.Controllers
{
	[ApiController]
    //[Route("api/[controller]")]
    [Route("api/VillaApi")]
	public class VillaAPIController : ControllerBase
	{
        //private readonly ILogger<VillaAPIController> _logger;
        //private readonly ILogging _logger;
        private readonly ApplicationDbContext _db;

        //public VillaAPIController(ILogger<VillaAPIController> logger)
        //public VillaAPIController(ILogging logger)
        //{
        //    _logger = logger;
        //}
        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
		{
            //_logger.Log("Getting all villas", "");
			//return Ok(VillaStore.VillaList);
            //return Ok(_db.Villas.ToString());

            var villas = _db.Villas.Select(v => new VillaDto
            {
                Id = v.Id,
                Name = v.Name,
                Details = v.Details,
                Rate = v.Rate,
                Sqft = v.Sqft,
                Occupancy = v.Occupancy,
                ImageUrl = v.ImageUrl,
                Amenity = v.Amenity
            }).ToList();

            // Return the result
            return Ok(villas);
        }
         
		//[HttpGet("id")]  
        [HttpGet("{id:int}", Name = "getVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type =typeof(VillaDto))]
        public ActionResult<VillaDto> GetVilla(int id)
		{

			if(id == 0)
			{
                //_logger.Log("Get Villa Error with Id" + id, "error");
				return BadRequest(); 
			}
            //var villa = (VillaStore.VillaList.FirstOrDefault(u => u.Id == id));
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            if (villa == null) {
				return NotFound();
			}

            //return Ok(villa);
            var villaDto = new VillaDto
            {
                Id = villa.Id,
                Name = villa.Name,
                Details = villa.Details,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity
            };

            return Ok(villaDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateVilla([FromBody]VillaDto villaDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (VillaStore.VillaList.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
            if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomeError", "Villa already Exists!");
                return BadRequest(ModelState);
            }
            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }
            if (villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //villaDto.Id = VillaStore.VillaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1; //this is no need for entity fremwork

            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy =villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };


            //VillaStore.VillaList.Add(villaDto);
            _db.Villas.Add(model);
            _db.SaveChanges();

            //return Ok(villaDto);
            return CreatedAtRoute("GetVilla", new { id = model.Id }, villaDto);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteVilla(int id) {
            if(id == 0) {
                return BadRequest();
            }
            //var villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == id);
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null) {
                return NotFound();
            }
            //VillaStore.VillaList.Remove(villa);
            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDto villaDto) {
            if(villaDto == null || id != villaDto.Id) {
                return BadRequest();
            }
            //var villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDto.Name;
            //villa.Sqft = villaDto.Sqft;
            //villa.Occupancy = villaDto.Occupancy;

            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };
            _db.Villas.Update(model);
            _db.SaveChanges();

            return NoContent();

        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            //var villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == id);
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            VillaDto villaDto = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };

            if (villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villaDto, ModelState);
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };

            _db.Villas.Update(model);
            _db.SaveChanges();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}

