using System;
using Pokeman.Models;

namespace Pokeman.Interfaces
{
	public interface IReviewerRepository
	{
		ICollection<Reviewer> GetReviewers();
		Reviewer GetReviewer(int reviwerId);
		ICollection<Review> GetReviewsByReviewer(int reviwerId);
		bool ReviewerExists(int reviwerId);
		bool CreateReviewer(Reviewer reviewer);
		bool Save();
		bool UpdateReviewer(Reviewer reviewer);
        bool DeleteReviewer(Reviewer reviewer);

    }
}

