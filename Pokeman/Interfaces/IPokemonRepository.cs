using System;
using Pokeman.Models;

namespace Pokeman.Interfaces
{
	public interface IPokemonRepository
	{
		ICollection<Pokemon> GetPokemons();
		Pokemon GetPokemon(int id);
		Pokemon GetPokemon(string name);
		decimal GetPokemonRating(int pokeId);
		bool PokemonExists(int pokeId);
		bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
		bool Save();
		bool UpdatePokemon( Pokemon pokemon);
        bool DeletePokemon(Pokemon pokemon);
    }
}

