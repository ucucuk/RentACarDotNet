﻿using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace UtilitiesClassLibrary.Exceptions
{
	public class ErrorHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		public ErrorHandlerMiddleware(RequestDelegate next)
		{
			_next = next;
		}
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception error)
			{
				await HandleExceptionAsync(context, error);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception error)
		{
			var response = context.Response;
			response.ContentType = "application/json";
			var responseModel = ApiResponse<string>.Fail(error.Message);
			switch (error)
			{
				case NotFoundException e:
					// custom application error
					response.StatusCode = (int)HttpStatusCode.NotFound;
					break;
				case AlreadyExistsException e:
					// custom application error
					response.StatusCode = 403;
					break;
				case KeyNotFoundException e:
					// not found error
					response.StatusCode = (int)HttpStatusCode.NotFound;
					break;
				default:
					// unhandled error
					response.StatusCode = (int)HttpStatusCode.InternalServerError;
					break;
			}
			var result = JsonSerializer.Serialize(responseModel);
			Log.Error(result);
			await response.WriteAsync(result);
		}
	}
}
