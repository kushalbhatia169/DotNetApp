using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using MyApp.Api.Infrastructure.Repositories.Common;
using MyApp.Api.Repositories.Interfaces;
using Path = System.IO.Path;

namespace MyApp.Api
{
    public class CategoryRepository(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) : GenericJsonRepository<Category>(Path.Combine(env.ContentRootPath, "Data", "categories.json")), ICategoryRepository
    {
        public async Task<IEnumerable<Category>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var items = await GetAllAsync(cancellationToken);
            var set = new HashSet<string>(keys);
            return items.Where(i => set.Contains(i.Id));
        }
    }
}
