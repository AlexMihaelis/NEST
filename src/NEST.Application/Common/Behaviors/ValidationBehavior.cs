using FluentValidation;
using MediatR;

namespace NEST.Application.Common.Behaviors;

// Pipeline Behavior - промежуточный этап, который выполняется перед обработкой команды или запроса через MediatR
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    // Все валидаторы, зарегистрированный для данного типа запроса
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle
    (
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        // Запускаем все найденные валидаторы для данного запроса
        var validationResult = await Task.WhenAll(_validators
                .Select(v => v.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken)));
        
        // Собираем все ошибки из результатов проверки
        var failures = validationResult.SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        // Если есть хотя бы одна ошибка - прекращается обработка запроса
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }
        
        // Если ошибок нет, то передает дальше в сл. этап Pipeline, где в итоге будет вызвать соответствующий Handler
        return await next();
    }
}