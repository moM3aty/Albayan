using Albayan.Areas.Admin.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class SalesOutletGroupedViewModel
    {
        public string Governorate { get; set; }
        public List<SalesOutlet> Outlets { get; set; }
    }

    public class SalesOutletFormViewModel
    {
        public SalesOutlet SalesOutlet { get; set; }
        public SelectList Governorates { get; set; }

        [Display(Name = "محافظة جديدة")]
        public string NewGovernorate { get; set; }
    }
    

}
