//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LocationMatch.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class LocationMatchAnalysi
    {
        public int Id { get; set; }
        public int LocationListId { get; set; }
        public int TrackId { get; set; }
        public string LocationListName { get; set; }
        public string TrackName { get; set; }
        public decimal Radius { get; set; }
        public System.DateTime TimeSubmitted { get; set; }
        public Nullable<System.DateTime> TimeStarted { get; set; }
        public Nullable<System.DateTime> TimeFinished { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
    }
}
