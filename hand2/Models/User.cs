//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace hand2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        private string _userName;
        private string _userPassword;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.items = new HashSet<item>();
        }
        [Required]
        public int UserID { get; set; }
        [Display(Name = "�� �����")]
           public string userName { get; set; }
        [Required]
        [Display(Name = "�����")]
        public string UserPassword { get => _userPassword; set => _userPassword = value; }
        public string UserMail { get; set; }
        public string UserPHone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<item> items { get; set; }
    }
}
