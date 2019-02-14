using Advice.Ranoi.Core.Domain;
using Advice.Identity.Server.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Domain
{
    public class User : Entity, IUser
    {
        public User(String name, String password) : base(Guid.NewGuid())
        {
            Name = name;
            Password = password;
            Active = true;
            ForceExpire = DateTime.MinValue;
        }

        public string Name { get; private set; }

        public string Password { get; private set; }

        public bool Active { get; private set; }

        public DateTime ForceExpire { get; private set; }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (this.Password.Equals(oldPassword))
            {
                this.Password = newPassword;
                this.ForceExpire = DateTime.Now;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
