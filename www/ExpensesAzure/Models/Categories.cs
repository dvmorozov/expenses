//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialApps.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Categories
    {
        public Categories()
        {
            this.ExpensesCategories = new HashSet<ExpensesCategories>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public System.Guid DataOwner { get; set; }
        public Nullable<double> Limit { get; set; }
        public string EncryptedName { get; set; }
    
        public virtual ICollection<ExpensesCategories> ExpensesCategories { get; set; }
    }
}
