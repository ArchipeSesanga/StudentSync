using StudentSync.Data;
using StudentSync.Interfaces;
using StudentSync.Models;

namespace StudentSync.Repositories
{
    public class DBInitializerRepo : IDBInitializer
    {
        public void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed Students
            if (!context.Students.Any())
            {
                var students = new Student[]
                {
                    new Student { StudentNumber = "2021000001", FirstName = "Liam", Surname = "Peters", EnrollmentDate = DateTime.Parse("2021-02-03"), Photo="DefaultPic.png", Email="DefaultEmail@gmail.com" },
                    new Student { StudentNumber = "2012000002", FirstName = "Sophia", Surname = "Green", EnrollmentDate = DateTime.Parse("2021-02-01"), Photo="DefaultPic.png", Email="DefaultEmail@gmail.com" },
                    new Student { StudentNumber = "2021000003", FirstName = "Noah", Surname = "Kim", EnrollmentDate = DateTime.Parse("2021-02-04"), Photo="DefaultPic.png", Email="DefaultEmail@gmail.com" }
                };
                foreach (Student s in students)
                {
                    context.Students.Add(s);
                }
                context.SaveChanges();
            }

            // Seed Consumers
            if (!context.Consumers.Any())
            {
                var consumers = new Consumer[]
                {
                    new Consumer { ConsumerId = "0123456789", Name = "Ethan", Surname = "Williams", EnrollmentDate = DateTime.Parse("2021-02-03"), Email = "Qwe@gmail.com"},
                    new Consumer { ConsumerId = "0223456789", Name = "Ava", Surname = "Brown", EnrollmentDate = DateTime.Parse("2021-02-01"), Email = "tyu@gmail.com" },
                    new Consumer { ConsumerId = "0333456789", Name = "Mason", Surname = "Nguyen", EnrollmentDate = DateTime.Parse("2021-02-04"), Email = "asd@gmail.com" }
                };
                foreach (Consumer c in consumers)
                {
                    context.Consumers.Add(c);
                }
                context.SaveChanges();
            }
        }
    }
}