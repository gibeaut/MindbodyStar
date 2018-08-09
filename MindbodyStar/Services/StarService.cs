using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using MindbodyStar.MindbodyClassService;
using MindbodyStar.Models;

namespace MindbodyStar.Services
{
    public class StarService
    {
        public List<int> GetStudios()
        {
            if (HttpContext.Current.Cache["StudioList"] != null)
            {
                return HttpContext.Current.Cache["StudioList"] as List<int>;
            }
            return new List<int>();
        }

        public StudioStars GetClientVisits(string username, string password, int studioid)
        {
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now.AddDays(-1);

            var studioString = $"StudioID{studioid}";
            HttpContext.Current.Response.Cache.SetValidUntilExpires(true);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddDays(1));

            StudioStars studioInfo;
            if (HttpContext.Current.Cache[studioString] != null)
            {
                studioInfo = HttpContext.Current.Cache[studioString] as StudioStars;

                if (studioInfo != null && 
                    studioid.ToString().Equals(studioInfo.StudioID) && 
                    studioInfo.StartDate.ToShortDateString().Equals(startDate.ToShortDateString()))
                {
                    return studioInfo;
                }
            }
            if (username.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace())
            {
                throw new Exception("Studio does not have a star board setup.");
            }

            var classes = GetClasses(username, password, studioid, startDate, endDate);
                var clients = GetClients(username, password, studioid, classes);

                var clientVisits = clients.Select(c => c.Value).ToList();
            

                studioInfo = new StudioStars()
                {
                    ClientList = clientVisits,
                    StartDate = startDate,
                    EndDate = endDate,
                    StudioID = studioid.ToString()
                };

                HttpContext.Current.Cache[studioString] = studioInfo;

               var studioList = new List<int>();

                if (HttpContext.Current.Cache["StudioList"] != null)
                {
                    studioList = HttpContext.Current.Cache["StudioList"] as List<int>;
                }

                studioList.Add(studioid);

                HttpContext.Current.Cache["StudioList"] = studioList;
            

            return studioInfo;
        }

        public Class[] GetClasses(string username, string password, int studioid, DateTime startDate, DateTime endDate)
        {

            var sourceCredentials = new MindbodyClassService.SourceCredentials()
            {
                SourceName = Credentials.SourceName,
                Password = Credentials.Password,
                SiteIDs = new int[] { studioid }
            };

            var userCredentials = new UserCredentials() { Password = password, Username = username, SiteIDs = new int[] { studioid } };

            var classService = new MindbodyClassService.ClassService();
            var results =
                classService.GetClasses(new GetClassesRequest()
                {
                    SourceCredentials = sourceCredentials,
                    StartDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0),
                    EndDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59)
                });
            return results.Classes;
        }

        public Dictionary<String, ClientVisits> GetClients(string username, string password, int studioid, Class[] classes)
        {
            var names = new List<string>();

            var sourceCredentials = new MindbodyClassService.SourceCredentials()
            {
                SourceName = Credentials.SourceName,
                Password = Credentials.Password,
                SiteIDs = new int[] { studioid }
            };
            var userCredentials = new UserCredentials() { Password = password, Username = username, SiteIDs = new int[] { studioid } };

            var classService = new MindbodyClassService.ClassService();
            var clients = new Dictionary<String, ClientVisits>();
            foreach (var _class in classes)
            {
                var classID = _class.ID.Value;

                var results =
                    classService.GetClassVisits(new GetClassVisitsRequest()
                    {
                        ClassID = classID,
                        SourceCredentials = sourceCredentials,
                        UserCredentials = userCredentials
                    });
                if (!results.Status.Equals(MindbodyStar.MindbodyClassService.StatusCode.Success))
                {
                    throw new Exception(results.Message);
                }
                if (results.Class?.Visits != null)
                {
                    foreach (var classVisit in results.Class.Visits)
                    {

                        if (classVisit.Client != null)
                        {
                            var client = classVisit.Client;
                            var clientVisits = new ClientVisits();
                            if (clients.ContainsKey(client.ID))
                            {
                                clients.TryGetValue(client.ID, out clientVisits);
                                clients.Remove(client.ID);
                            }
                            // Add Client Data
                            if (clientVisits.ClientID == null)
                            {
                                clientVisits.ClientID = client.ID;
                                var lastNameLength = 1;
                                var displayName = String.Format("{0} {1}", client.FirstName,
                                    client.LastName.Substring(0, lastNameLength));
                                while (names.Contains(displayName) && client.LastName.Length < lastNameLength - 1)
                                {
                                    lastNameLength++;
                                    displayName = String.Format("{0} {1}", client.FirstName,
                                        client.LastName.Substring(0, lastNameLength));
                                }

                                clientVisits.DisplayName = displayName;
                            }
                            clientVisits.NumberOfVisits++;

                            clients.Add(client.ID, clientVisits);
                        }
                    }
                }
            }
            return clients;
        }
    }
}