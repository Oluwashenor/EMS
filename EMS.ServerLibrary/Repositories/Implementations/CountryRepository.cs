using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EMS.ServerLibrary.Repositories.Implementations
{
    public class CountryRepository(AppDbContext context) : IGenericRepositoryInterface<Country>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Countries.FindAsync(id);
            if (dep is null) return NotFound();
            context.Countries.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Country>> GetAll() => await context.Countries.ToListAsync();

        public async Task<Country> GetById(int id) => await context.Countries.FindAsync(id);

        public async Task<GeneralResponse> Insert(Country entity)
        {
            if (!await CheckName(entity.Name)) return new GeneralResponse(false, "Country already added");
            context.Countries.Add(entity);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Country entity)
        {
            var dep = await context.Countries.FindAsync(entity.Id);
            if (dep is null) return NotFound();
            dep.Name = entity.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Country not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await context.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await context.Countries.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
