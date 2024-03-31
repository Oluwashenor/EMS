using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EMS.ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository(AppDbContext context) : IGenericRepositoryInterface<Department>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Departments.FindAsync(id);
            if (dep is null) return NotFound();
            context.Departments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Department>> GetAll() => await context.Departments.ToListAsync();

        public async Task<Department> GetById(int id) => await context.Departments.FindAsync(id);

        public async Task<GeneralResponse> Insert(Department entity)
        {
            if (!await CheckName(entity.Name)) return new GeneralResponse(false, "Department already added");
            context.Departments.Add(entity);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Department entity)
        {
            var dep = await context.Departments.FindAsync(entity.Id);
            if (dep is null) return NotFound();
            dep.Name = entity.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Department not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await context.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await context.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
