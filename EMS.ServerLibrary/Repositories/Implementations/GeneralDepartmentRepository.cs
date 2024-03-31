using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EMS.ServerLibrary.Repositories.Implementations
{
    public class GeneralDepartmentRepository(AppDbContext context) : IGenericRepositoryInterface<GeneralDepartment>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.GeneralDepartments.FindAsync(id);
            if (dep is null) return NotFound();
            context.GeneralDepartments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<GeneralDepartment>> GetAll() => await context.GeneralDepartments.ToListAsync();

        public async Task<GeneralDepartment> GetById(int id) => await context.GeneralDepartments.FindAsync(id);

        public async Task<GeneralResponse> Insert(GeneralDepartment entity)
        {
            if (!await CheckName(entity.Name)) return new GeneralResponse(false, "Department already added");
            context.GeneralDepartments.Add(entity);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(GeneralDepartment entity)
        {
            var dep = await context.GeneralDepartments.FindAsync(entity.Id);
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
            var item = await context.GeneralDepartments.FirstOrDefaultAsync(x=>x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
