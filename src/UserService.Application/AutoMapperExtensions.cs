namespace UserService.Application;

public static class AutoMapperExtensions
{
    public static Result<TEntity> MapWithResult<TEntity, TDto>(this IMapper mapper, TDto dto)
    {
		try
		{
			TEntity mappedEntity = mapper.Map<TEntity>(dto);
			return Result.Ok(mappedEntity);
        }
		catch (Exception exception)
		{
			return Result.Fail<TEntity>(exception.Message);
		}
    }
}
