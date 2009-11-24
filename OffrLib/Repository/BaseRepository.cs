using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json;

namespace Offr.Repository
{
    public abstract class BaseRepository<T> where T : ITopic
    {
        protected SortedList<string, T> _list;
        protected IQueryable<T> _queryable;

        protected BaseRepository()
        {
            _list = new SortedList<string, T>();
            _queryable = _list.Values.AsQueryable();
        }

        public virtual T Get(string id)
        {
            if (_list.ContainsKey(id))
                return _list[id];

            return default(T);
        }

        public virtual void Save(T instance)
        {
            var id = instance.ID;
            if (id == null)
            {
                //create new id and assign it? erm no.. just throw an exception
                //id = _list.Count.ToString().GetHashCode().ToString();
                throw new InvalidOperationException("Not a valid ID");
            }
            else
            {
                _list[id] = instance;
               // string s=JSON.Serialize(_list);
            }
        }
        public virtual void Remove(T instance)
        {
            if (_list.ContainsKey(instance.ID))
                _list.Remove(instance.ID);
        }

        public IQueryable<T> GetAll()
        {
            return _queryable;
        }
    }
}
