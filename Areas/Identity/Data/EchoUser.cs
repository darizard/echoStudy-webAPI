using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace echoStudy_webAPI.Areas.Identity.Data
{
    public class EchoUser : IdentityUser
    {
        public DateTime? DateCreated
        {
            /**
             * Grabs the creation date or sets it if it does not exist
             */
            get
            {
                if(DateCreated == null)
                {
                    DateCreated = DateTime.Now;
                }
                return DateCreated;
            }
            private set {}
        }
    }
}
