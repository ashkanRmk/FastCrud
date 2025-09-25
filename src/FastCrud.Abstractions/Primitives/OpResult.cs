namespace FastCrud.Abstractions.Primitives;

public record OpResult<TData> (bool Succeeded, string Message , TData? Data = default);
