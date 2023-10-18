using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Infastructure;

namespace UnitTests.BusinessTests
{
    internal static class UnitTestHelper
    {
        public static Mapper GetMapper()
        {
            var profile = new MappingConfig();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            return new Mapper(configuration);
        }
    }
}
