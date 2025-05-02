using StudentSync.Data;
using StudentSync.Interfaces;
using StudentSync.Models;
using SQLitePCL;


namespace StudentSync.Repositories
{
    public class ConsumerRepo : IConsumer
    {
        private readonly ApplicationDbContext _context;

        public ConsumerRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Consumer Create(Consumer consumer)
        {
            _context.Add(consumer);
            _context.SaveChanges();
            return consumer;
        }

        public bool Delete(Consumer consumer)
        {
            _context.Remove(consumer);
            _context.SaveChanges();
            return IsExist(consumer.ConsumerId);
        }

        private bool IsExist(int consumerId)
        {
            throw new NotImplementedException();
        }

        public Consumer Details(string id)
        {
            var consumer = _context.Consumers?.FirstOrDefault(x => x.ConsumerId == id);
            return consumer;
        }

        public Consumer Edit(Consumer consumer)
        {
            _context.Update(consumer);
            _context.SaveChanges();
            return consumer;
        }

        public IQueryable<Consumer> GetStudents(string searchString, string sortOrder)
        {
            var consumer = _context.Consumers
               .ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                consumer = consumer.Where(s => s.ConsumerId.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "number_desc":
                    consumer = consumer.OrderByDescending(s => s.ConsumerId).ToList();
                    break;
                case "name_desc":
                    consumer = consumer.OrderByDescending(s => s.Surname).ToList();
                    break;
                case "Date":
                    consumer = consumer.OrderBy(s => s.EnrollmentDate).ToList();
                    break;
                case "date_desc":
                    consumer = consumer.OrderByDescending(s => s.EnrollmentDate).ToList();
                    break;
                default:
                    consumer = consumer.OrderBy(s => s.Surname).ToList();
                    break;
            }

            return consumer.AsQueryable();
        }

        public bool IsExist(string id)
        {
            bool isExist = false;
            Consumer existStudent = Details(id);
            if (existStudent == null)
            {
                isExist = true;
            }
            return isExist;
        }

        Consumer IConsumer.Create(Consumer consumer)
        {
            throw new NotImplementedException();
        }

        bool IConsumer.Delete(Consumer consumer)
        {
            throw new NotImplementedException();
        }

        Consumer IConsumer.Details(string id)
        {
            throw new NotImplementedException();
        }

        Consumer IConsumer.Edit(Consumer consumer)
        {
            throw new NotImplementedException();
        }

        IQueryable<Consumer> IConsumer.GetConsumer(string searchString, string sortOrder)
        {
            throw new NotImplementedException();
        }

        bool IConsumer.IsExist(string id)
        {
            return _context.Consumers.Any(x => x.ConsumerId == id);
        }
    }
}
