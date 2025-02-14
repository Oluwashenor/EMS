using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EMS.ServerLibrary.Repositories.Implementations
{
    public class TownRepository(AppDbContext context) : IGenericRepositoryInterface<Town>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Towns.FindAsync(id);
            if (dep is null) return NotFound();
            context.Towns.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Town>> GetAll() => await context.Towns.AsNoTracking().Include(x=>x.City).ToListAsync();

        public async Task<Town> GetById(int id) => await context.Towns.FindAsync(id);

        public async Task<GeneralResponse> Insert(Town entity)
        {
            if (!await CheckName(entity.Name)) return new GeneralResponse(false, "Town already added");
            context.Towns.Add(entity);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Town entity)
        {
            var dep = await context.Towns.FindAsync(entity.Id);
            if (dep is null) return NotFound();
            dep.Name = entity.Name;
            dep.CityId = entity.CityId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Town not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await context.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await context.Towns.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
