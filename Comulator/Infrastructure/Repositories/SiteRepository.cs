//using Application.Interfaces.Repositories;
//using Domain;
//using Domain.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace Infrastructure.Repositories;
//public class SiteRepository(GatherItDbContext context) : ISiteRepository
//{
//    private readonly GatherItDbContext _context = context;

//    public IEnumerable<Site> GetSites()
//    {
//        return _context.Sites.Include(x => x.Searchs);
//    }
//}
