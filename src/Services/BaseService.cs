using Entities;
namespace Services
{
    public class BaseService
    {
        protected readonly SqlDbContext _context;

        public BaseService(SqlDbContext context)
        {
            _context = context;
        }
    }
}
