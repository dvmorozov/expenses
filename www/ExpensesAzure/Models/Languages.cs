
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
    
public partial class Languages
{

    public Languages()
    {

        this.Strings = new HashSet<Strings>();

    }


    public int ID { get; set; }

    public string Language { get; set; }



    public virtual ICollection<Strings> Strings { get; set; }

}

}
