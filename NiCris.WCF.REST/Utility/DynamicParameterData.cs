using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Text;

namespace NiCris.WCF.REST.Utility
{
    public class DynamicParameterData : DynamicObject
    {
        private readonly Dictionary<string, string> members;

        public DynamicParameterData()
        {
            this.members = new Dictionary<string, string>();
        }

        public DynamicParameterData(IEnumerable<KeyValuePair<string, string>> data)
            : this()
        {
            foreach (var pair in data)
            {
                members.Add(pair.Key.ToUpper(), pair.Value);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return members.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string ret;
            members.TryGetValue(binder.Name.ToUpper(), out ret);
            result = ret;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return false;
            }

            members[binder.Name.ToUpper()] = value as string;
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var member in members)
            {
                sb.AppendFormat("{0}: {1}; ", member.Key, member.Value ?? "null");
            }
            return sb.ToString();
        }
    }

}