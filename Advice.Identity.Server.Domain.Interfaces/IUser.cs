using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Domain.Interfaces
{
    public interface IUser : IEntity
    {
        String Name { get; }
        String Password { get; }
        Boolean Active { get; }
        DateTime ForceExpire { get; }

        void ChangePassword(String oldPassword, String newPassword);
    }
}
