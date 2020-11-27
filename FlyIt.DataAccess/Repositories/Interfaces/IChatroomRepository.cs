using FlyIt.DataAccess.Entities;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IChatroomRepository
    {
        public Task<Chatroom> GetChatroomByIdAsync(int id);
    }
}