using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using SocialApps.Models;

namespace SocialApps.Controllers
{
    //  The container of returned data for conversion into JSON.
    //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
    [DataContract]
    public class ExpenseListReturn
    {
        [DataMember]
        public bool Success;

        [DataMember]
        public string Message;

        [DataMember]
        public ExpenseNameWithCategory[] ExpenseList;
    }

    //  https://www.evernote.com/shard/s132/nl/14501366/d03bc138-ab63-470b-8b99-df02ec42f205
    public class EncryptedPair
    {
        private string _EncryptedName;
        private string _Id;

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public string EncryptedName
        {
            get { return _EncryptedName != null ? _EncryptedName.Trim() : null; }
            set { _EncryptedName = value; }
        }
    }

    //  https://www.evernote.com/shard/s132/nl/14501366/d03bc138-ab63-470b-8b99-df02ec42f205
    [DataContract]
    public class EncryptedPairs
    {
        [DataMember]
        public EncryptedPair[] encryptedPairs;
    }
}
