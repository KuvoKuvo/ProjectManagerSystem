using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.BLL.Mapping;
using ProjectManager.DAL;

namespace ProjectManager.Tests.Helpers
{
    public static class TestHelper
    {
        public static AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static IMapper CreateRealMapper()
        {
            var config = new MapperConfiguration(
                (Action<IMapperConfigurationExpression>)(cfg =>
                {
                    cfg.AddMaps(typeof(MappingProfile).Assembly);
                }),
                NullLoggerFactory.Instance 
            );

            return config.CreateMapper();
        }
    }
}
