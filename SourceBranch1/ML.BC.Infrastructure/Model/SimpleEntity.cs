using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.Model
{
    public interface ISimpleEntity
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }

    public interface ISimpleEntity<T> : ISimpleEntity
    {
        T Value { get; set; }
    }

    public class SimpleEntity : ISimpleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class SimpleListModel {
        public string Id { get; set; }
        public string Name { get; set; }

        public bool selected = false;
    }

    public class SimpleEntity<T> : SimpleEntity, ISimpleEntity<T>
    {
        public T Value { get; set; }
    }
}
