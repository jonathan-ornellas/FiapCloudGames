using AutoMapper;
using Fiap.Game.Api.Contracts.Request;
using Fiap.Game.Api.Contracts.Response;
using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Record;
using Fiap.Game.Domain.ValueObjects;

namespace Fiap.Game.Api.Mapping
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<Domain.Entities.Game, GameResponse>();
            CreateMap<LibraryView, LibraryItemResponse>();
            
            CreateMap<CreateGameRequest, Domain.Entities.Game>()
                .ConstructUsing(src => new Domain.Entities.Game(src.Title, src.Description, src.Price, src.ReleaseDate));

            CreateMap<RegisterRequest, User>()
                .ConstructUsing((src, context) =>
                {
                    var passwordHasher = context.Items["PasswordHasher"] as Domain.Interface.Service.IPasswordHasherService;
                    var hashedPassword = passwordHasher?.Hash(src.Password) ?? throw new InvalidOperationException("Password hasher not available");
                    return new User(src.Name, new Email(src.Email), hashedPassword);
                });

            CreateMap<string, Email>().ConstructUsing(src => new Email(src));
            CreateMap<Email, string>().ConstructUsing(src => src.Value);
        }
    }
}
