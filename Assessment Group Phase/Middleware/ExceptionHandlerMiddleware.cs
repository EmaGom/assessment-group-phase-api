using Assessment.Group.Phase.Models.Exceptions;
using Assessment.Group.Phase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Net.Mime;

namespace Assessment.Group.Phase.Middleware
{
    public class ExceptionHandlerMiddleware : IExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                // log the error
                _logger.LogError($"Exception  Request {context.Request?.Method}: {context.Request?.Path.Value}\nError: ${ex.Message}");
                var response = context.Response;
                response.ContentType = "application/json";

                // get the response code and message
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            // Default exception
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = string.IsNullOrEmpty(ex.Message) ? ApiResponseErrorMessages.InternalServerError : ex.Message;


            if (ex is BadRequestException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = string.IsNullOrEmpty(ex.Message) ? ApiResponseErrorMessages.BadRequest : ex.Message;
            }
            else if (ex is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = string.IsNullOrEmpty(ex.Message) ? ApiResponseErrorMessages.NotFound : ex.Message;
            }

            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = statusCode;
            var exception = new ExceptionHandlingModel(statusCode, message);
            return context.Response.WriteAsync(JsonConvert.SerializeObject(exception));
        }

        internal class ExceptionHandlingModel
        {
            public ExceptionHandlingModel(int statusCode, string message)
            {
                StatusCode = statusCode;
                ErrorMessage = message;
            }
            public int StatusCode { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
