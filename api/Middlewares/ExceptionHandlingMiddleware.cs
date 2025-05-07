using System.Net;
using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.TransactionDtos;
using api.Exceptions;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (WalletHubException ex)
            {
                _logger.LogWarning(ex, "Handled WalletHub exception");
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                code = (int)statusCode,
                message
            };

            await context.Response.WriteAsJsonAsync(response);
        }

    }

}