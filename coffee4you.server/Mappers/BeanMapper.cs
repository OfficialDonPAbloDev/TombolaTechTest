using Coffee4You.Server.Domain.Entities;
using Coffee4You.Server.Dtos;

namespace Coffee4You.Server.Mappers;

public static class BeanMapper
{
    public static BeanDto ToDto(Bean bean, Guid? todayBotdBeanId)
    {
        var isBeanOfTheDay = todayBotdBeanId.HasValue && todayBotdBeanId.Value == bean.Id;

        return new(
        bean.Id,
        bean.Name,
        bean.Description,
        bean.ImageUrl,
        bean.Cost,
        bean.CurrencyCode,
        bean.Colour.Name,
        bean.Country.Name,
        isBeanOfTheDay
    );
    }
}
