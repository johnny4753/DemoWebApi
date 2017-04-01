using AutoMapper;
using DemoWebApi.Models.DataTransferObject;
using DemoWebApi.Models.Domain;

namespace DemoWebApi.Tools
{
    public class AutoMapperConfig
    {
        public static readonly MapperConfiguration MapperConfig = new MapperConfiguration
        (
                cfg =>
                {
                    cfg.AddProfile<MyProfile>();
                }
        );

        public static IMapper Mapper = MapperConfig.CreateMapper();
    }

    public class MyProfile : Profile
    {
        public MyProfile()
        {
            // 在此處 CreateMap<src, dest>() , 集中管理類別對應轉換
            CreateMap<Order, OrderDto>();
        }
        
    }

}