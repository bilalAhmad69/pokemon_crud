using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Pokeman.Dto;
using Pokeman.Interfaces;
using Pokeman.Models;

namespace Pokeman.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : Controller
	{
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository , IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type =typeof(IEnumerable<Category>))]
		public IActionResult GetCategories()
		{
			var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
            return Ok(categories);
		}

		[HttpGet("{categoryId}")]
		[ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
		{
			if(!_categoryRepository.CategoryExsits(categoryId))
			{
			return NotFound();
			}
			var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(category);
		}

		[HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
		public IActionResult GetPokemonByCategoryId(int categoryId)
		{
			var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategory(categoryId));
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			return Ok(pokemons);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateCategory([FromBody] CategoryDto createCategory)
		{
			if (createCategory == null)
			{
				return BadRequest(ModelState);
			}
			var category = _categoryRepository.GetCategories().Where(c => c.Name.Trim().ToUpper() == createCategory.Name.Trim().ToUpper()).FirstOrDefault();
			if (category != null)
			{
				ModelState.AddModelError("", "Category already exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var categoryMap = _mapper.Map<Category>(createCategory);
			if (!_categoryRepository.CreateCategory(categoryMap))
			{
				ModelState.AddModelError("", "Some thing went wrong while creating category");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully created");
		}

		[HttpPut("{categoryId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult UpdateCategory(int categoryId , [FromBody] CategoryDto category)
		{
			if (category == null)
				return BadRequest(ModelState);
            if (category.Id != categoryId)
				return BadRequest(ModelState);
            if (!_categoryRepository.CategoryExsits(categoryId))
				return NotFound();
			var categoryMap = _mapper.Map<Category>(category);
            if (!_categoryRepository.UpdateCategory(categoryMap))
			{
				ModelState.AddModelError("", "Something went wrong while updating category");
				return StatusCode(500, ModelState);
			}
			return Ok();
		}

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExsits(categoryId))
            {
                return NotFound();
            }

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return NoContent();
        }

    }


}

