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
	public class PokemonController : Controller
	{
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository ,
			IReviewRepository reviewRepository,IMapper	mapper)
		{
			_pokemonRepository = pokemonRepository;
			_reviewRepository = reviewRepository;
            _mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type =typeof(IEnumerable<Pokemon>))]
		public IActionResult GetPokemons()
		{
			var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(pokemons);

		}

		[HttpGet("{id}")]
		[ProducesResponseType(200, Type = typeof(Pokemon))]
		public IActionResult GetPokemon(int id)
		{
			if (!(_pokemonRepository.PokemonExists(id)))
			{
				return NotFound();
			}
			var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(id));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(pokemon);
		}

		[HttpGet("{id}/rating")]
		[ProducesResponseType(200, Type = typeof(decimal))]
		public IActionResult GetRating(int id)
		{
			if (!_pokemonRepository.PokemonExists(id))
			{
				NotFound();
			}
			var rating = _pokemonRepository.GetPokemonRating(id);
			if (!ModelState.IsValid) {
				BadRequest(ModelState);
			}
			return Ok(rating);

		}

        [HttpGet("{name}/name")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        public IActionResult GetPokemonByName(string name)
        {
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(name));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId , [FromBody] PokemonDto createPokemon)
		{
			if(createPokemon == null)
			{
				return BadRequest(ModelState);
			}
			var pokemons = _pokemonRepository.GetPokemons().Where(pk => pk.Name.Trim().ToUpper() == createPokemon.Name.Trim().ToUpper()).FirstOrDefault();
			if(pokemons != null)
			{
				ModelState.AddModelError("", "Pokemon already exists");
				return StatusCode(422, ModelState);
			}
			var pokemonMap = _mapper.Map<Pokemon>(createPokemon);
			if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
			{
				ModelState.AddModelError("", "Something went wrong while creating pokemon");
				return StatusCode(500,ModelState);
			}
			return Ok("Successfully created");
		}

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId,
          [FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (pokeId != updatedPokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong when deleting reviews");
            }

            if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return NoContent();
        }
    }
}

