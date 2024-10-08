﻿using MinimalAPI.Entities;

namespace MinimalAPI.Repository
{
    public interface IRepositoryGenre
    {
        Task<int> Create(Genre genre);
        Task<List<Genre>> GetAll();
        Task<Genre?> GetById(int id);
        Task<bool> Exists(int id);
        Task Update(Genre genre);
        Task Delete(int id);
        Task<List<int>> GenreExists(List<int> ids);
    }
}
