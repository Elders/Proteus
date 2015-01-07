using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Elders.Cronus.DomainModeling;

namespace Elders.Proteus.Tests
{
    [DataContract(Name = "32089dd2-5f88-4b34-834c-d10c4c3249be")]
    public class RegisterAccount
    {
        RegisterAccount() { }

        public RegisterAccount(AccountId id, string username, string password, Email email)
        {
            Id = id;
            Username = username;
            Password = password;
            Email = email;
        }

        [DataMember(Order = 1)]
        public AccountId Id { get; private set; }

        [DataMember(Order = 2)]
        public string Username { get; private set; }

        [DataMember(Order = 3)]
        public string Password { get; private set; }

        [DataMember(Order = 4)]
        public Email Email { get; private set; }

        public bool IsValid()
        {
            return
                GuidId.IsValid(Id) &&
                !String.IsNullOrWhiteSpace(Username) &&
                !String.IsNullOrWhiteSpace(Password) &&
                Email.IsValid(Email);
        }
    }

    [DataContract(Name = "24c59143-b95e-4fd6-8bbf-8d5efffe3185")]
    public class AccountId : GuidId
    {
        protected AccountId() { }
        public AccountId(Guid id) : base(id, "Account") { Gay = id; }

        [DataMember(Order = 2)]
        public Guid Gay { get; private set; }
    }

    [DataContract(Name = "a1c442da-76c8-4c88-8729-3249f1592572")]
    public class Email : IValueObject<Email>
    {
        const string EmailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        public Email(string address)
        {
            Address = address;
        }

        Email() { }

        [DataMember(Order = 1)]
        public string Address { get; private set; }

        public static bool operator ==(Email left, Email right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(null, left))
                return false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Email left, Email right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var casted = obj as Email;
            if (casted != null)
                return Equals(casted);
            else
                return false;
        }

        public bool Equals(Email other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Address.Equals(other.Address);
        }

        public bool Equals(Email left, Email right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(left, right)) return true;

            return left.Address == right.Address;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 101 ^ Address.GetHashCode();
                return result;
            }
        }

        public int GetHashCode(Email obj)
        {
            return obj.GetHashCode();
        }


        public static bool IsValid(string email)
        {
            Regex pattern = new Regex(EmailPattern);
            Match match = pattern.Match(email);
            return match.Success;
        }

        public static bool IsValid(Email email)
        {
            if (email == null)
                return false;
            else
                return IsValid(email.Address);
        }
        public override string ToString()
        {
            return Address;
        }

    }
}
