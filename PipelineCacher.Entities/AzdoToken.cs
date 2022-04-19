using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Entities
{
    [DataContract]
    public class AzdoToken
    {
        public int Id { get; set; }
        [DataMember(Name = "access_token")]
        public String AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public String TokenType { get; set; }

        [DataMember(Name = "refresh_token")]
        public String RefreshToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        public bool IsPending { get; set; }        
    }
}
