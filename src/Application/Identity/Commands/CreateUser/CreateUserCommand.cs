using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryQuery;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Identity.Commands.CreateUser;
public class CreateUserCommand : IRequest<string>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
    public FileDto? File { get; set; }  
    public JobRole JobRole { get; set; }
    public List<string> GroupIds { get; set; } = new List<string>();
    public List<string> NotificationGroupIds { get; set; } = new List<string>();

    public class Mapping : Profile
    {
            public Mapping()
            {
                CreateMap<CreateUserCommand, CreateUserRequest>();
            }
    }
}
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IIdentityService identityService,
                                    IApplicationGroupManager applicationGroupManager,
                                    IMapper mapper,
                                    ILogger<CreateUserCommandHandler> logger)
    {
        _identityService = identityService;
        _applicationGroupManager = applicationGroupManager;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUserRequest = _mapper.Map<CreateUserRequest>(request);
        var result =  await _identityService.CreateUserAsync(createUserRequest, cancellationToken);
        if (!result.Result.Succeeded)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("Add user goes wrongly:");
            //foreach (var error in result.Result.Errors)
            //{
            //    sb.AppendLine(error);
            //}
            throw new Exception();
        }
        return result.UserId;
    }
}
