using Ardalis.Specification.EntityFrameworkCore;
using PodcastProxy.Database.Contexts;

namespace PodcastProxy.Database.Repositories;

public class Repository<T>(PodcastDbContext context) : RepositoryBase<T>(context), IRepository<T>
    where T : class;