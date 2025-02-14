using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.ServerLibrary.Repositories.Implementations
{
    public class EmployeeRepository(AppDbContext context) : IGenericRepositoryInterface<Employee>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await context.Employees.FindAsync(id);
            if (item is null) return NotFound();
            context.Employees.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Employee>> GetAll()
        {
            var employees = await context.Employees
                .AsNoTracking()
                .Include(t => t.Town)
                .ThenInclude(c=>c.City)
                .ThenInclude(c=>c.Country)
                .Include(b => b.Branch)
                .ThenInclude(d => d.Department)
                .ThenInclude(gd => gd.GeneralDepartment)
                .ToListAsync();
            return employees;
        }

        public async Task<Employee> GetById(int id)
        {
            var employee = await context.Employees
                .AsNoTracking()
                .Include(t => t.Town)
                .ThenInclude(c => c.City)
                .ThenInclude(c => c.Country)
                .Include(b => b.Branch)
                .ThenInclude(d => d.Department)
                .ThenInclude(gd => gd.GeneralDepartment).FirstOrDefaultAsync(ei => ei.Id == id);
            return employee!;
        }

        public async Task<GeneralResponse> Insert(Employee item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Employee already added");
            context.Employees.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Employee item)
        {
            var findUser = await context.Employees.FirstOrDefaultAsync(e => e.Id == item.Id);
            if (findUser is null) return new GeneralResponse(false, "Employee does not exist");
            findUser.Name = item.Name;
            findUser.Other = item.Other;
            findUser.Address = item.Address;
            findUser.TelephoneNumber = item.TelephoneNumber;
            findUser.BranchId = item.BranchId;
            findUser.TownId = item.TownId;
            findUser.CivilId = item.CivilId;
            findUser.FileNumber = item.FileNumber;
            findUser.JobName = item.JobName;
            findUser.Photo = item.Photo;
            await context.SaveChangesAsync();
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Town not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await context.Branches.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
