using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using test.Data;

namespace test.Invocables
{
    public class TestInvocable : IInvocable
    {
        private ApplicationDbContext _context;
        public TestInvocable(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task Invoke()
        {
            await this._context.Test.AddAsync(new TestModel() {
                Name = "test name"
            });

            await this._context.SaveChangesAsync();

            var models = await this._context.Test.Select(t => t).ToListAsync();

            foreach(var model in models) {
                System.Console.WriteLine($"Model Name: {model.Name}, Id: {model.Id}");
            }
        }
    }
}
