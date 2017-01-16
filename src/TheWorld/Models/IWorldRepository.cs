using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> getAllTrips();
        Trip GetTripByName(string tripName);

        void AddTrip(Trip trip);
        void AddStop(string tripName, Stop newStop);

        Task<bool> SaveChangesAsync();        
    }
}