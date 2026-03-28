using System.Collections.Concurrent;
using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public IEnumerable<User> GetAll() => _users.Values;

    public User? GetById(Guid id)
        => _users.TryGetValue(id, out var user) ? user : null;

    public User Create(User user)
    {
        // Caller is responsible for choosing the Id.
        _users[user.Id] = user;
        return user;
    }

    public bool Update(Guid id, User updatedUser)
    {
        if (!_users.TryGetValue(id, out _))
            return false;

        updatedUser.Id = id; // Keep route Id as the source of truth.
        _users[id] = updatedUser;
        return true;
    }

    public bool Delete(Guid id)
        => _users.TryRemove(id, out _);
}

