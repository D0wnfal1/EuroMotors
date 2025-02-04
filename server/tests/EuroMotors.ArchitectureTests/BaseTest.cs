using System.Reflection;
using System.Windows.Input;
using EuroMotors.Api;
using EuroMotors.Domain.Users;
using EuroMotors.Infrastructure.Database;

namespace EuroMotors.ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
