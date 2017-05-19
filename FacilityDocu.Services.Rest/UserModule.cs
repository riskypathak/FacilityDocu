using FacilityDocu.Data;
using FacilityDocu.DTO;
using FacilityDocu.DTO.Models;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.Services.Rest
{
    public class UserModule : NancyModule
    {
        public UserModule() : base("/User")
        {
            Post["/Login"] = x =>
            {
                try
                {
                    var bodyText = Request.Body.AsString();
                    var user = this.Bind<UserDTO>(); //Breakpoint

                    using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
                    {
                        var currentUser = context.Users.SingleOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);

                        if(currentUser != null)
                        {
                            return Response.AsText(currentUser.Role);
                        }
                        else
                        {
                            return HttpStatusCode.Forbidden;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return HttpStatusCode.Forbidden;
                }
            };
        }
    }
}
