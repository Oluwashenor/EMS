using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EMS.ServerLibrary.Repositories.Implementations
{
    public class CityRepository(AppDbContext context) : IGenericRepositoryInterface<City>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Cities.FindAsync(id);
            if (dep is null) return NotFound();
            context.Cities.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<City>> GetAll() => await context.Cities.ToListAsync();

        public async Task<City> GetById(int id) => await context.Cities.FindAsync(id);

        public async Task<GeneralResponse> Insert(City entity)
        {
            if (!await CheckName(entity.Name)) return new GeneralResponse(false, "City already added");
            context.Cities.Add(entity);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(City entity)
        {
            var dep = await context.Cities.FindAsync(entity.Id);
            if (dep is null) return NotFound();
            dep.Name = entity.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry City not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await context.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await context.Cities.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
