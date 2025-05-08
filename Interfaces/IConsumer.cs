using StudentSync.Models;

namespace StudentSync.Interfaces
{
    public interface IConsumer
    {
        IQueryable<Consumer> GetConsumer(string searchString, string sortOrder);
        Consumer Details(string id);
        Consumer Create(Consumer consumer);
        Consumer Edit(Consumer consumer);
        bool Delete(Consumer consumer);
        bool IsExist(string id);
        void Delete(string id);
    }
}
