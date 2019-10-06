using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace MyCompany.Infrastructure {
    public static class AutoMapperStartup {
        public static IServiceCollection AddAutoMapperModule(this IServiceCollection @this)
        {
            @this.AddAutoMapper();
            return @this;
        }
    }
}
