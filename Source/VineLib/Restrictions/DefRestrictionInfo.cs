using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RVCRestructured.Source.VineLib.Restrictions
{
    public class DefRestrictionInfo
    {
        public static DefRestrictionInfo Empty => new DefRestrictionInfo();

        private readonly Def def;

        private Def user;

        private bool userWhitelisted;
        private bool userBlacklisted;
        private bool isRequired;

        public Def Def => def;

        public Def User => user;
        
        public bool UserWhitelisted => userWhitelisted;

        public bool UserBlacklisted => userBlacklisted;

        public bool IsRequired => isRequired;

        public bool CanUse => IsRequired || UserWhitelisted || (!def.IsRestricted() && !UserBlacklisted);

        private DefRestrictionInfo() 
        {
            userWhitelisted = false;
            userBlacklisted = false;
            isRequired = false;
        }

        public DefRestrictionInfo(Def def) : this()
        {
            this.def = def;
        }

        public void WhiteListUser(Def def)
        {
            user = def;
            userWhitelisted = true;
            RestrictionsChecker.MarkRestricted(def);
        }

        public void BlackListUser(Def def)
        {
            user = def;
            userBlacklisted = true;
        }

        public void SetRequired(Def def)
        {
            user = def;
            isRequired = true;
        }
    }
}
