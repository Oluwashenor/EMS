using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EMS.ServerLibrary.Repositories.Implementations
{
    public class BranchRepository(AppDbContext context) : IGenericRepositoryInterface<Branch>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Branches.FindAsync(id);
            if (dep is null) return NotFound();
            context.Branches.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Branch>> GetAll() => await context.Branches
            .AsNoTracking()
            .Include(d=>d.Department).ToListAsync();

        public async Task<Branch> GetById(int id) => await context.Branches.FindAsync(id);

        public async Task<GeneralResponse> Insert(Branch entity)
        {
            if (!await CheckName(entity.Name)) return new GeneralResponse(false, "Branch already added");
            context.Branches.Add(entity);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Branch entity)
        {
            var branch = await context.Branches.FindAsync(entity.Id);
            if (branch is null) return NotFound();
            branch.Name = entity.Name;
            branch.DepartmentId = entity.DepartmentId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Branch not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await context.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await context.Branches.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
