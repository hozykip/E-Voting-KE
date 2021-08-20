using E_Vote_System.Helper_Code.Common;
using E_Vote_System.Models;
using E_Vote_System.Models.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace E_Vote_System.Hubs
{
    [HubName("electionsHub")]
    public class ElectionsHub : Hub
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [HubMethodName("fetchVotes")]
        public async Task<List<VoteResultModel>> FetchVotes()
        {
            List<VoteResultModel> votes = new List<VoteResultModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "select v.Id as Id, v.VoterId as VoterId, v.CandidateId as CandidateId, c.PositionId as PositionId from tb_Votes v Inner Join tb_ElectionCandidates c on c.Id = v.CandidateId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Notification = null;
                        DataTable dt = new DataTable();

                        SqlDependency.Start(connectionString);

                        SqlDependency dependency = new SqlDependency(command);
                        dependency.OnChange += Dependency_OnChange;
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }

                        var reader = command.ExecuteReader();
                        dt.Load(reader);


                        votes = CommonMethod.ConvertToList<VoteResultModel>(dt);
                    }

                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }

            

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ElectionsHub>();



            return await context.Clients.All.ReceiveVotes(votes);

        }



        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if(e.Type == SqlNotificationType.Change)
            {
                ElectionsHub nHub = new ElectionsHub();
                nHub.FetchVotes();
            }
        }
    }
}