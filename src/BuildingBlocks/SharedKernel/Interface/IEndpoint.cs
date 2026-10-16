namespace SharedKernel.Interface;


using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Enums;
using SharedKernel.Models;
using System.Net;




public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

public abstract class BaseEndpoint
{
    //protected ApiVersionSet BuildVersion_1_Set(IEndpointRouteBuilder app, string tag)
    //{
    //    return app.NewApiVersionSet(tag)
    //              .HasApiVersion(API_VERSION.V1)

    //              .ReportApiVersions()
    //              .Build();
    //}
    //protected ApiVersionSet BuildVersion_2_Set(IEndpointRouteBuilder app, string tag)
    //{
    //    return app.NewApiVersionSet(tag)

    //              .HasApiVersion(API_VERSION.V2)
    //              .ReportApiVersions()
    //              .Build();
    //}
    protected Ok<ApiResponse> Ok(string message)
      => TypedResults.Ok(new ApiResponse(message));



    protected IResult Ok(object? data)
        => TypedResults.Ok(new ApiResponse(data));

    protected IResult Ok(object data, string message)
        => TypedResults.Ok(new ApiResponse(data, message));

    protected IResult BadRequest(string message)
        => TypedResults.BadRequest(new ApiResponse(message, HttpStatusCode.BadRequest));

    protected IResult BadRequest(string[] messages)
        => TypedResults.BadRequest(new ApiResponse(messages));

    protected IResult BadRequest(ValidationResult result)
        => BadRequest(result.Errors.Select(e => e.ErrorMessage).ToArray());

    protected IResult NotFound(string message)
        => TypedResults.NotFound(new ApiResponse(message, HttpStatusCode.NotFound));

    protected IResult Unauthorized(string message = "Unauthorized")
        => TypedResults.Unauthorized();

    /// <summary>
    /// Extract user ID from JWT token in the authorization header
    /// </summary>
    protected int GetCurrentUserId(HttpContext context, IJwtService jwtService)
    {
        try
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
                return 0;

            var (userType, userId) = jwtService.ExteractToken(authHeader);
            return userId;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Get current user information from JWT token
    /// </summary>
    protected (List<RoleType>, int userId) GetCurrentUser(HttpContext context, IJwtService jwtService)
    {
        try
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
                return ([], 0);

            return jwtService.ExteractToken(authHeader);
        }
        catch
        {
            return ([], 0);
        }
    }

    /// <summary>
    /// Check if current user is admin and return user info
    /// </summary>
    protected (bool isAdmin, int userId, string errorMessage) ValidateAdminAccess(HttpContext context, IJwtService jwtService)
    {
        var (userType, userId) = GetCurrentUser(context, jwtService);

        if (userId == 0)
            return (false, 0, "توکن معتبر نیست");

        if (userType.Any(x => x != RoleType.Admin))
            return (false, userId, "دسترسی غیرمجاز - فقط ادمین‌ها می‌توانند به این بخش دسترسی داشته باشند");

        return (true, userId, string.Empty);
    }


}

