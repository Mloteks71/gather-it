﻿using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface ICompanyNameRepository
{
    public IEnumerable<CompanyName> GetCompanyNames();
}
