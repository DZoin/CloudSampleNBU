using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using LocationMatch.DataAccess;
using LocationMatch.WebSite.WebRole.Maps;

namespace LocationMatch.WebSite.WebRole
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Collections.IList visibleTables = Global.DefaultModel.VisibleTables;
            if (visibleTables.Count == 0)
            {
                throw new InvalidOperationException("There are no accessible tables. Make sure that at least one data model is registered in Global.asax and scaffolding is enabled or implement custom pages.");
            }

            for (int i = visibleTables.Count - 1; i >= 0; i--)
            {
                var table = visibleTables[i] as MetaTable;
                if (table != null && table.Name == "LocationMatchAnalysis")
                {
                    visibleTables.RemoveAt(i);
                    break;
                }
            }

            Menu1.DataSource = visibleTables;
            Menu1.DataBind();

            if (!IsPostBack)
            {
                loadAnalysisData();
            }
        }

        private void loadAnalysisData()
        {
            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                List<LocationList> locationLists = ctx.LocationLists.OrderBy(ll => ll.Name).ToList();
                locationLists.Insert(0, new LocationList { Id = -1, Name = "None" });
                List<Track> tracks = ctx.Tracks.OrderBy(t => t.Name).ToList();
                tracks.Insert(0, new Track { Id = -1, Name = "None" });

                ddLocationLists.DataSource = locationLists;
                ddLocationLists.DataMember = "Id";
                ddLocationLists.DataValueField = "Id";
                ddLocationLists.DataTextField = "Name";
                ddLocationLists.DataBind();

                ddTracks.DataSource = tracks;
                ddTracks.DataMember = "Id";
                ddTracks.DataValueField = "Id";
                ddTracks.DataTextField = "Name";
                ddTracks.DataBind();
            }
        }

        protected void btnRunLocationMatchAnalysis_Click(object sender, EventArgs e)
        {
            int locationListId = int.Parse(ddLocationLists.SelectedValue);
            int trackId = int.Parse(ddTracks.SelectedValue);

            if (locationListId == -1 || trackId == -1)
            {
                lblErrorMessage.Text = "Location List and Track must be selected to run Location Match Analysis";
                return;
            }

            decimal radius;
            if (!decimal.TryParse(tbRadius.Text, out radius))
            {
                lblErrorMessage.Text = "Radius must be a decimal number";
                return;
            }

            lblErrorMessage.Text = string.Empty;

            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                var analysis = new LocationMatchAnalysi();
                analysis.TimeSubmitted = DateTime.Now;
                analysis.Status = AnalysisStatus.Submitted.ToString();
                analysis.LocationListId = locationListId;
                analysis.TrackId = trackId;
                analysis.Radius = radius;
                analysis.LocationListName = ddLocationLists.SelectedItem.Text;
                analysis.TrackName = ddTracks.SelectedItem.Text;

                ctx.LocationMatchAnalysis.Add(analysis);
                ctx.SaveChanges();
            }

            loadResults(0);
        }

        protected void btnLoadResults_Click(object sender, EventArgs e)
        {
            loadResults(0);
        }

        private void loadResults(int pageIndex)
        {
            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                var analysisResults = ctx.LocationMatchAnalysis
                    .OrderByDescending(ar => ar.TimeSubmitted)
                    .Select(ar => new
                    {
                        ar.Id,
                        ar.LocationListName,
                        ar.TrackName,
                        ar.Radius,
                        ar.TimeSubmitted,
                        ar.TimeStarted,
                        ar.TimeFinished,
                        ar.Status,
                        ar.Result
                    })
                    .ToList();

                gridResults.DataSource = analysisResults;
                gridResults.PageIndex = pageIndex;
                gridResults.SelectedIndex = -1;
                gridResults.DataBind();
            }
        }

        protected void gridResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (gridResults.SelectedRow == null ||
                gridResults.SelectedRow.Cells == null ||
                gridResults.SelectedRow.Cells.Count == 0)
                {
                    return;
                }
                string analysisIdString = gridResults.SelectedRow.Cells[1].Text;

                if (string.IsNullOrWhiteSpace(analysisIdString))
                {
                    return;
                }

                int analysisId = int.Parse(analysisIdString);

                var mapDrafter = new MapDrafter();
                string mapJson = mapDrafter.DraftJsonMapForAnalysisResult(analysisId);

                ClientScript.RegisterStartupScript(this.GetType(), "DrawMap", "DrawMap('" + mapJson + "')", true);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                ClientScript.RegisterStartupScript(this.GetType(), "ShowErrorMessage", "ShowErrorMessage('" + errorMessage + "')", true);
            }
        }

        protected void gridResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            loadResults(e.NewPageIndex);
        }
    }
}