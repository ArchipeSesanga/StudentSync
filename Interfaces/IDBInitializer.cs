using StudentSync.Data;

namespace StudentSync.Interfaces
{
    public interface IDBInitializer
    {
        void Initialize(SQLiteDBContext context);
    }
}
