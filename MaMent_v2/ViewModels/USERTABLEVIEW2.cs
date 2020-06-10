using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MaMent_v2.ViewModels;
using MaMent_v2.Models;
using System.ComponentModel.DataAnnotations;

namespace MaMent_v2.ViewModels
{
    using System;
    using System.Collections.Generic;
    public class USERTABLEVIEW2
    {
        public USERTABLEVIEW2()
        {

        }
        public int userId { get; set; }
        [Display(Name = "Username")]
        [Required]
        public string userName { get; set; }
        [Display(Name = "Password")]
        [Required]
        public string userPassword { get; set; }
        [Display(Name = "User Role")]
        public List<CHECKBOXVIEW> userRole { get; set; }
        [Display(Name = " User Status")]
        public string STATUSNAME { get; set; }

        //gak yakin

        public Nullable<int> STATUSID { get; set; }
        public virtual STATUSTABLE STATUSTABLE { get; set; }
    }
}