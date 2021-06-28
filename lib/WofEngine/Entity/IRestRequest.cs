using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.Entity
{
    public interface IRestRequest : IEntity
    {
        string GetEndpoint();

        string GetMethod();
    }
}
