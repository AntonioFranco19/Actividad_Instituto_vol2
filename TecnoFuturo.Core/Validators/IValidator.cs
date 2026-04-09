namespace TecnoFuturo.Core.Validators;

public interface IValidator<T>
{
    bool Validate(T entity);
}