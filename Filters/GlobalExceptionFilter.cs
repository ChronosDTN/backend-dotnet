namespace ChronosDotnetApi.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class GlobalExceptionFilter : IExceptionFilter {

    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) {
        _logger = logger;
    }

    public void OnException(ExceptionContext context) {
        var (statusCode, message) = context.Exception switch {
            KeyNotFoundException ex   => (StatusCodes.Status404NotFound,    ex.Message),
            ArgumentException ex      => (StatusCodes.Status400BadRequest,   ex.Message),
            DbUpdateConcurrencyException ex => (StatusCodes.Status409Conflict, ex.Message),
            DbUpdateException ex      => (StatusCodes.Status409Conflict,     "Conflito ao persistir dados. Verifique se as referências existem e não há duplicatas."),
            _                         => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno inesperado.")
        };

        _logger.LogError(context.Exception, "Exceção não tratada: {Message}", context.Exception.Message);

        context.Result = new ObjectResult(new {
            error = message,
            type  = context.Exception.GetType().Name
        }) {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
