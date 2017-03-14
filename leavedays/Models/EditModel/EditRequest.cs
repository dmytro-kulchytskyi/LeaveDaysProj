﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace leavedays.Models.EditModel
{
    public class EditRequest
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string Status { get; set; }
        public string RequestBase { get; set; }
        public string VacationDates { get; set;}
    }
}