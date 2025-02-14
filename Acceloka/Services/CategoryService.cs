using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Acceloka.Services
{
    public class CategoryService
    {
        private readonly AccelokaContext _db;

        public CategoryService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryModel>> GetCategories()
        {
            var response = await _db.Categories
                .Select(Q => new CategoryModel()
                {
                    Id = Q.Id,
                    Name = Q.Name
                }).ToListAsync();

            return response;
        }

        public async Task<List<CategoryModel>> UpdateCategory(CategoryModel request)
        {
            var category = await _db.Categories
                .Where(Q => Q.Id == request.Id)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                throw new KeyNotFoundException($"Kategori {request.Id} tidak ditemukan");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Nama tidak boleh kosong");
            } 
            
            if (category.Name == request.Name)
            {
                throw new ArgumentException("Nama tidak boleh sama");
            }

            category.Name = request.Name;

            _db.Update(category);
            await _db.SaveChangesAsync();

            var response = await _db.Categories
                .Select(Q => new CategoryModel()
                {
                    Id = Q.Id,
                    Name = Q.Name
                }).ToListAsync();

            return response;
        }

    }
}
