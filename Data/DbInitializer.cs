using echoStudy_webAPI.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(EchoStudyDB context)
        {
            context.Database.EnsureCreated();

            //if(!context.)
        }
    }
}
