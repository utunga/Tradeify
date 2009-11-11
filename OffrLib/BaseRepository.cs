﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Repository
{
    public abstract class BaseRepository<T> where T : ITopic
    {
        private SortedList<string, T> _list;
        private IQueryable<T> _queryable;

        protected BaseRepository()
        {
            _list = new SortedList<string, T>();
            _queryable = _list.Values.AsQueryable();
        }

        protected T Get(string id)
        {
            if (_list.ContainsKey(id))
                return _list[id];

            return default(T);
        }

        protected void Save(T instance)
        {
            var id = instance.ID;
            if (id == null)
            {
                //create new id and assign it?
                // erm no.. just throw an exception
                throw new InvalidOperationException("Not a valid ID");
            }
            else
            {
                _list[id] = instance;
            }
        }

        protected void Remove(T instance)
        {
            if (_list.ContainsKey(instance.ID))
                _list.Remove(instance.ID);
        }

        protected IQueryable<T> GetAll()
        {
            return _queryable;
        }
    }
}