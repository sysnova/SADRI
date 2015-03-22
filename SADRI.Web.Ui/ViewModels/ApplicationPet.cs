using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity.DomainModel;

namespace SADRI.Web.Ui.ViewModels
{
    public class ApplicationPet
    {
        virtual public int id { get; set; }
        virtual public string PetName { get; set; }
        virtual public string Species { get; set; }
        virtual public DateTime Birthday { get; set; }

        virtual public string Speak()
        {
            return "Hi.  My name is '" + PetName + "' and I'm a " + Species + " born on " + Birthday + ".";
        }
    }
    public class ApplicationPetMap : ClassMapping<ApplicationPet>
    {
        public ApplicationPetMap()
        {
            this.Table("ApplicationPets");

            Id(x => x.id, m =>
            {
                m.Column("id");

                m.Generator(Generators.Native, g => g.Params(new
                {
                }));

                m.Length(10);
                //m.Type(new Int32(),"D");
                m.Access(Accessor.Field);
            });

            this.Property(x => x.PetName);

            this.Property(x => x.Species);

            this.Property(x => x.Birthday);

        }
    }


}