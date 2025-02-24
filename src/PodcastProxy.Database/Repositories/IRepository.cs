using Ardalis.Specification;

namespace PodcastProxy.Database.Repositories;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
}