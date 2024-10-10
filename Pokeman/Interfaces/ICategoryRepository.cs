using System;
using Pokeman.Models;

namespace Pokeman.Interfaces
{
	public interface ICategoryRepository
	{
		ICollection<Category> GetCategories();
		Category GetCategory(int categoryId);
		ICollection<Pokemon> GetPokemonByCategory(int categoryId);
		bool CategoryExsits(int categoryId);
		bool CreateCategory(Category category);
		bool Save();
		bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
    }
}

