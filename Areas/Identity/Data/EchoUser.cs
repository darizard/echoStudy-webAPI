using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace echoStudy_webAPI.Areas.Identity.Data
{
    public class EchoUser : IdentityUser
    {
        public DateTime? DateCreated { get; set; }
    }
}
