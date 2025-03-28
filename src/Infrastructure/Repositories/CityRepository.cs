﻿using Application.Interfaces.Repositories;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories;
public class CityRepository(GatherItDbContext context) : ICityRepository
{
    private readonly GatherItDbContext _context = context;

    public IEnumerable<City> GetCitys()
    {
        return _context.Citys;
    }
}
