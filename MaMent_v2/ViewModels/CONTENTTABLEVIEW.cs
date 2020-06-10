using MaMent_v2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MaMent_v2.ViewModels
{
    public class CONTENTTABLEVIEW
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONTENTTABLEVIEW()
        {
            //this.CONTENTKEYWORDTABLEs = new HashSet<CONTENTKEYWORDTABLE>();
            //this.CONTENTROLETABLEs = new HashSet<CONTENTROLETABLE>();
        }

        public int contentId { get; set; }
        [Required]
        [Display(Name = "Content Title")]
        public string contentTitle { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Content Description")]
        public string contentDescription { get; set; }
        [Display(Name = "Content Link")]
        public string contentLink { get; set; }
        [Display(Name = "Content File")]
        public string contentFilePath { get; set; }
        public Nullable<int> CONTENTTYPEID { get; set; }
        public Nullable<int> USERID { get; set; }
        public Nullable<int> STATUSID { get; set; }
        [Display(Name = "Content Role")]
        public List<CHECKBOXVIEW> contentRole { set; get; }
        public DateTime contentDate { get; set; }
        [Display(Name = "Content Keyword")]
        public string contentKeyword { get; set; }
        public string statusName { get; set; }
        public string userName { get; set; }
        public string contentTypeName { get; set; }
        public string newDate { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CONTENTKEYWORDTABLE> CONTENTKEYWORDTABLEs { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CONTENTROLETABLE> CONTENTROLETABLEs { get; set; }
        public virtual CONTENTTYPETABLE CONTENTTYPETABLE { get; set; }
        public virtual STATUSTABLE STATUSTABLE { get; set; }
        public virtual USERTABLE USERTABLE { get; set; }
    }
}