using System;
using Pokeman.Models;

namespace Pokeman.Interfaces
{
	public interface IOwnerRepository
	{
		ICollection<Owner> GetOwners();
		Owner GetOwner(int ownerId);
		ICollection<Owner> GetOwnerOfAPokemon(int pokeId);
		ICollection<Pokemon> GetPokemonsByOwner(int ownerId);
		bool OwnerExists(int ownerId);
		bool CreateOwner(Owner owner);
		bool Save();
		bool UpdateOwner(Owner owner);
        bool DeleteOwner(Owner owner);

    }
}

