using FlyIt.DataAccess.Entities;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IChatroomRepository
    {
        public Task<Chatroom> AddChatroomAsync(Chatroom chatroom);

        public Task<Chatroom> GetChatroomByFlightIdAsync(int flightId);

        public Task<Chatroom> GetChatroomByIdAsync(int id);
    }
}