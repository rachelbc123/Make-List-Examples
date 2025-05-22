 using SWEWebReporting.Infrastructure;
using System;

namespace SWEWebReporting.ListViews
{
    public class IncidentListView
    {
        // int? means this property is a nullable integer (can hold null or a number).
        // { get; set; } creates a public getter and setter automatically.
        // = 0 sets a default value when a new object is created.
        public int? IncidentID { get; set; } = 0;

        // string properties are reference types and can be null by default.
        // Assigning = string.Empty sets them to an empty string ("") to avoid null issues.
        public string IncidentType { get; set; } = string.Empty;

        // Nullable int with default 0.
        public int? IncidentTypeID { get; set; } = 0;

        // String defaulted to empty to prevent nulls.
        public string ResponsibleParty { get; set; } = string.Empty;

        public string RegionName { get; set; } = string.Empty;

        public string LocationName { get; set; } = string.Empty;

        // Nullable DateTime (can hold null or a date).
        // Uses a constant for a default "null" date placeholder.
        public DateTime? IncDate { get; set; } = Constants.NullDateConstant;

        public string ActivityBefore { get; set; } = string.Empty;

        public string WhatHappened { get; set; } = string.Empty;

        public int? LocationID { get; set; } = 0;

        public int? RegionID { get; set; } = 0;

        public int? ResponsiblePartyID { get; set; } = 0;
    }
}
