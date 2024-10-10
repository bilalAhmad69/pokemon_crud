using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Pokeman.Dto;
using Pokeman.Interfaces;
using Pokeman.Models;
using Pokeman.Repository;

namespace Pokeman.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountryController : Controller
	{
		private readonly ICountryRepository _countryRepository;
		private readonly IMapper _mapper;
        public CountryController(ICountryRepository countryRepository , IMapper mapper)
		{
			_countryRepository = countryRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
		public IActionResult GetCountries()
		{
			var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			return Ok(countries);
		}

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
		public IActionResult GetCountry(int countryId)
		{
			if (!_countryRepository.CountryExists(countryId))
			{
				return NotFound();
			}
			var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			return Ok(country);
		}

		[HttpGet("owners/{ownerId}")]
		[ProducesResponseType(200, Type = typeof(Country))]
		public IActionResult GetCountryByOwner(int ownerId)
		{
			var Country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			return Ok(Country);
		}

        [HttpGet("countries/{countryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
		public IActionResult GetOwnerFromACountry(int countryId)
		{
			var owners = _mapper.Map<List<OwnerDto>>(_countryRepository.GetOwnersFromACountry(countryId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(owners);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateCountry( [FromBody] CountryDto createCountry)
		{
			if(createCountry == null)
			{
				return BadRequest(ModelState);
			}
			var country = _countryRepository.GetCountries().Where(c => c.Name.Trim().ToUpper() == createCountry.Name.Trim().ToUpper()).FirstOrDefault();
			if(country != null)
			{
				ModelState.AddModelError("", "Country already exists");
				return StatusCode(422, ModelState);
			}
			var countryMap = _mapper.Map<Country>(createCountry);
			if (!_countryRepository.CreateCountry(countryMap))
			{
				ModelState.AddModelError("", "Some thing went wrong while creating country");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully created");
		}

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int countryId, [FromBody] CountryDto country)
        {
            if (country == null)
                return BadRequest(ModelState);
            if (country.Id != countryId)
                return BadRequest(ModelState);
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            var countryMap = _mapper.Map<Country>(country);
            if (!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating country");
                return StatusCode(500, ModelState);
            }
            return Ok();
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            var countryToDelete = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return NoContent();
        }
    }
}

