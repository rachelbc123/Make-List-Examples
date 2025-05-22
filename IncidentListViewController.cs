 using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For EF Core features like AsNoTracking()
using SWEWebReporting.DatabaseContext; // For database context class
using SWEWebReporting.Extensions; // Possibly for custom extensions
using SWEWebReporting.ListViews; // For IncidentListView class (data structure)
using SWEWebReporting.Managers; // For session manager class
using SWEWebReporting.Models; // For entity models like Incident
using System.Data.SqlClient;

namespace SWEWebReporting.Controllers
{
    // Attribute to define this class as a Web API controller with route prefix "api/IncidentListView"
    [Route("api/[controller]")]
    [ApiController] // Enables automatic model validation, response formatting, etc.
    public class IncidentListViewController : Controller
    {
        // Private readonly field to store the database context instance,
        // used for querying and updating the database.
        private readonly SWESafetyDatabaseContext db;

        // Constructor for this controller, accepting a DB context through dependency injection
        // This sets up 'db' so we can use it in other methods.
        public IncidentListViewController(SWESafetyDatabaseContext context)
        {
            db = context;
        }

        // HTTP GET method mapped to "api/IncidentListView/getList"
        // Returns a JSON list of IncidentListView objects.
        [HttpGet("getList")]
        public JsonResult GetList()
        {
            // Session manager reads session variables like filters (dates, IDs)
            HttpSessionVariableManager m = new HttpSessionVariableManager(HttpContext);
            string errorMessage = string.Empty;

            // Create an empty list to store results.
            List<IncidentListView> l = new List<IncidentListView>();

            try
            {
                // Execute a raw SQL stored procedure with parameters from session variables.
                // SqlQuery<T> returns entities of type T.
                // AsNoTracking improves performance by not tracking these entities for changes.
                // ToList() executes the query and collects results into a List.
                l = db.Database
                    .SqlQuery<IncidentListView>($"EXEC getIncidentListForDGV @StartDate={m.CurrentSafetyStartDate}, @EndDate={m.CurrentSafetyEndDate}, @IncidentTypeID={m.CurrentSafetyIncidentTypeID}, @LocationID={m.CurrentSafetyLocationID}, @RegionID={m.CurrentSafetyRegionID}, @ResponsiblePartyID={m.CurrentSafetyResponsiblePartyID}")
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                // If an error occurs, immediately return JSON with error info
                return new JsonResult(new { error = true, message = ex.ToString() });
            }

            // If errorMessage is not empty, return a custom error JSON; otherwise return the list as JSON.
            return errorMessage.Length > 0 ? new JsonErrorResult(new { message = errorMessage }) : Json(l);
        }

        // Another HTTP GET method mapped to "api/IncidentListView/GetIncidentCounts"
        // Similar structure, but with IncidentTypeID fixed to 0 
        [HttpGet("GetIncidentCounts")]
        public JsonResult GetIncidentCounts()
        {
            HttpSessionVariableManager m = new HttpSessionVariableManager(HttpContext);
            string errorMessage = string.Empty;

            List<IncidentListView> l = new List<IncidentListView>();

            try
            {
                l = db.Database
                    .SqlQuery<IncidentListView>($"EXEC getIncidentListForDGV @StartDate={m.CurrentSafetyStartDate}, @EndDate={m.CurrentSafetyEndDate}, @IncidentTypeID=0, @LocationID={m.CurrentSafetyLocationID}, @RegionID={m.CurrentSafetyRegionID}, @ResponsiblePartyID={m.CurrentSafetyResponsiblePartyID}")
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                // Instead of returning immediately, store error message and continue.
                errorMessage = $"Failed to retrieve incidents. The error was: {ex.Message}";
            }

            return errorMessage.Length > 0 ? new JsonErrorResult(new { message = errorMessage }) : Json(l);
        }

        // HTTP GET method mapped to "api/IncidentListView/deleteIncident/{IncidentID}"
        // Deletes an incident by ID (soft delete by setting Deleted date).
        [HttpGet("deleteIncident/{IncidentID}")]
        public JsonResult DeleteIncident(int IncidentID)
        {
            string errorMessage = string.Empty;

            // Query the database for the incident with the given ID.
            // If none found, create a new Incident (with default ID 0).
            Incident io = db.Incidents.FirstOrDefault(i => i.IncidentID == IncidentID) ?? new Incident();

            try
            {
                if (io.IncidentID != 0) // If incident exists
                {
                    // Soft delete: set Deleted timestamp and user.
                    io.Deleted = DateTime.Now;
                    io.DeletedBy = "unknown"; // Ideally this comes from logged in user info.
                    db.Incidents.Update(io); // Tell EF to update this record.
                    db.SaveChanges(); // Save changes to database.
                }
            }
            catch (Exception ex)
            {
                // Save error message if exception happens
                errorMessage = $"Failed to delete incident. The error was: {ex.Message}";
            }

            // Return error JSON if there was a problem; otherwise success message.
            return errorMessage.Length > 0
                ? new JsonErrorResult(new { message = errorMessage })
                : new JsonResult(new { message = "Incident deleted." });
        }
    }
}
