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
    public class MasterModule : NancyModule
    {
        public MasterModule() : base("/Master")
        {
            Get["/All"] = x =>
            {
                IList<MasterDTO> data = new List<MasterDTO>();

                using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
                {
                    foreach (var tool in context.Tools)
                    {
                        data.Add(new MasterDTO()
                        {
                            Type = "Tools",
                            Id = tool.ToolID,
                            Description = tool.ToolName
                        });
                    }

                    foreach (var resource in context.Resources)
                    {
                        data.Add(new MasterDTO()
                        {
                            Type = resource.Type,
                            Id = resource.ResourceID,
                            Description = resource.ResourceName
                        });
                    }

                    foreach (var lg in context.LiftingGears)
                    {
                        data.Add(new MasterDTO()
                        {
                            Type = "LiftingGears",
                            Id = lg.Id,
                            Description = lg.Description
                        });
                    }

                    foreach (var risk in context.Risks)
                    {
                        data.Add(new MasterDTO()
                        {
                            Type = "Risks",
                            Id = risk.Id,
                            Description = risk.Description
                        });
                    }
                }
                return Response.AsJson(data);
            };

            Post["/All"] = x =>
            {
                try
                {
                    var bodyText = Request.Body.AsString();

                    var masterData = this.Bind<IList<MasterDTO>>(); //Breakpoint

                    using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
                    {
                        if (masterData.First().Type == "LiftingGears")
                        {
                            IList<int> idsToDelete = masterData.Select(m => m.Id).ToList();
                            //Get all those which are not present in input
                            var liftingGearsToDelete = context.LiftingGears.Where(l => !idsToDelete.Contains(l.Id)).ToList();
                            context.LiftingGears.RemoveRange(liftingGearsToDelete);

                            //Add new
                            masterData.Where(m => m.Id == 0).ToList().ForEach(m => context.LiftingGears.Add(new LiftingGear() { Description = m.Description }));

                            //Update Rest
                            masterData.Where(m => m.Id != 0).ToList().ForEach(m => context.LiftingGears.Single(l => l.Id == m.Id).Description = m.Description);
                        }
                        else if (masterData.First().Type == "Risks")
                        {
                            IList<int> idsToDelete = masterData.Select(m => m.Id).ToList();
                            //Get all those which are not present in input
                            var liftingGearsToDelete = context.Risks.Where(l => !idsToDelete.Contains(l.Id)).ToList();
                            context.Risks.RemoveRange(liftingGearsToDelete);

                            //Add new
                            masterData.Where(m => m.Id == 0).ToList().ForEach(m => context.Risks.Add(new Risk() { Description = m.Description }));

                            //Update Rest
                            masterData.Where(m => m.Id != 0).ToList().ForEach(m => context.Risks.Single(l => l.Id == m.Id).Description = m.Description);
                        }
                        else if (masterData.First().Type == "People")
                        {
                            //See Delete later

                            //Add new
                            masterData.Where(m => m.Id == 0).ToList().ForEach(m => context.Resources.Add(new Resource() { ResourceName = m.Description, Type = "People" }));

                            //Update Rest
                            masterData.Where(m => m.Id != 0).ToList().ForEach(m => context.Resources.Single(l => l.ResourceID == m.Id).ResourceName = m.Description);
                        }
                        else if (masterData.First().Type == "Machine")
                        {
                            //See Delete later

                            //Add new
                            masterData.Where(m => m.Id == 0).ToList().ForEach(m => context.Resources.Add(new Resource() { ResourceName = m.Description, Type = "Machine" }));

                            //Update Rest
                            masterData.Where(m => m.Id != 0).ToList().ForEach(m => context.Resources.Single(l => l.ResourceID == m.Id).ResourceName = m.Description);
                        }
                        else if (masterData.First().Type == "Tools")
                        {
                            //Won't to deleting work as it will efect existing
                            //Add new
                            masterData.Where(m => m.Id == 0).ToList().ForEach(m => context.Tools.Add(new Tool() { ToolName = m.Description }));

                            //Update Rest
                            masterData.Where(m => m.Id != 0).ToList().ForEach(m => context.Tools.Single(l => l.ToolID == m.Id).ToolName = m.Description);
                        }

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return HttpStatusCode.InternalServerError;
                }
                return HttpStatusCode.OK;
            };
        }
    }
}
