using StudentSync.Models;

namespace StudentSync.Interfaces
{
    public interface IStudent
    {
        IQueryable<Student> GetStudents(string searchString, string sortOrder);
        Student Details(string id);
        Student Create(Student student);
        Student Edit(Student student);
        void Delete(string id);

        bool IsExist(string id);
    }
}
