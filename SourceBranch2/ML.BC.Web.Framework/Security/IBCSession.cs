using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework
{
    public interface IBCSession
    {
        SessionUserDto User{get;}
        bool IsAuthenticated{get;}
        void Init();
        void Logout();
        void Login(string username);       
    }
}
