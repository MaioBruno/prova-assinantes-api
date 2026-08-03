using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AssinantesApi.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Ocorreu um erro na requisição.",
                Detail = exception.Message
            };

            switch (exception)
            {
                case ArgumentException:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Erro de validação.";
                    break;

                case KeyNotFoundException:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Recurso não encontrado.";
                    break;

                case InvalidOperationException:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Operação inválida.";
                    break;

                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Erro interno do servidor.";
                    problemDetails.Detail = "Um erro inesperado ocorreu.";
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}