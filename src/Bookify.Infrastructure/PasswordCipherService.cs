using Bookify.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure;
public class PasswordCipherService : IPasswordCipherService
{
    //private readonly IDistributedCache _asd;

    //public PasswordCipherService(IDistributedCache asd)
    //{
    //    _asd = asd;
    //}

    public string EncryptPassword(string passwordRaw)
    {
        return "asdasd";
    }


}
