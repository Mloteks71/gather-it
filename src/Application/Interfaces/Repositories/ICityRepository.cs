﻿using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface ICityRepository
{
    public IEnumerable<City> GetCitys();
}
