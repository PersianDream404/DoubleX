using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Contract.DTOs.Payments;
using SharedKernel.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services;

//public sealed class PaymentCallbackUrlFactory(
// LinkGenerator linkGenerator,
// IHttpContextAccessor httpContextAccessor
//) : IPaymentCallbackUrlFactory, IScopedDependency
//{
//    public string CreateUrl(PaymentCallbackRouteDto callbackRoute)
//    {
//        var httpContext = httpContextAccessor.HttpContext;

//        if (httpContext is null)
//            throw new InvalidOperationException("HttpContext is not available.");

//        var routeValues = new Dictionary<string, object?>(
//            callbackRoute.RouteValues,
//            StringComparer.OrdinalIgnoreCase);

//        routeValues["controller"] = callbackRoute.Controller;
//        routeValues["action"] = callbackRoute.Action;

//        if (!string.IsNullOrWhiteSpace(callbackRoute.Area))
//            routeValues["area"] = callbackRoute.Area;

//        var url = linkGenerator.GetUriByAction(
//            httpContext: httpContext,
//            action: callbackRoute.Action,
//            controller: callbackRoute.Controller,
//            values: routeValues,
//            scheme: httpContext.Request.Scheme,
//            host: httpContext.Request.Host);

//        if (string.IsNullOrWhiteSpace(url))
//            throw new InvalidOperationException("Could not generate payment callback URL.");

//        return url;
//    }
//}