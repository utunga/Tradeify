﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using NLog;
using Offr.Common;
using Offr.Json;

namespace Offr.Repository
{

    public abstract class BaseRepository<T> : IMemCache where T : ITopic
    {
        protected SortedList<string, T> _list;
        protected IQueryable<T> _queryable;
        public string FilePath { get; set; }
        protected bool dirty;
        
        public bool IsDirty
        {
            get { return dirty; }
        }

        protected BaseRepository()
        {
           InitalizeInMemory();
        }

        public void Invalidate()
        {
            InitalizeInMemory();
        }

        protected IQueryable<T> GetAll()
        {
            return _queryable;
        }

        public virtual T Get(string id)
        {
            if (_list.ContainsKey(id))
                return _list[id];

            return default(T);
        }

        public virtual void Save(T instance)
        {
            lock (this)
            {
                var id = instance.ID;
                if (id == null)
                {
                    throw new InvalidOperationException("Not a valid ID");
                }
                else
                {
                    _list[id] = instance;
                    dirty = true;
                    //String serializedList = JSON.Serialize(_list);             
                }
            }
        }

        public virtual void Remove(T instance)
        {
            //Note2Miles we should probably lock this one too
            lock (this)
            {
                if (_list.ContainsKey(instance.ID))
                {
                    _list.Remove(instance.ID);
                    dirty = true;
                }
            }
        }

        public virtual void SerializeToFile()
        {
            if (FilePath == null) throw new ApplicationException("Please set the FilePath before calling this method");

            LogManager.GetLogger("Global").Info("serializing " + this.ToString());
            //Console.WriteLine(");
            lock (this)
            {
                String serializedList = JSON.Serialize(_list);
                TextWriter tw = new StreamWriter(FilePath);
                tw.Write(serializedList);
                tw.Close();
            }
            dirty = false;
        }

        public void InitializeFromFile()
        {
            if (FilePath == null) throw new ApplicationException("Please set the FilePath before calling this method");
            FileInfo jsonFile;
            if (!(jsonFile = new FileInfo(FilePath)).Exists)
            {
                throw new IOException("Cannot find file " + FilePath);
            }
            
            // read data into a string builder (probably a better way to do this but this will do for now)
            StringBuilder stringBuilder = new StringBuilder();
            using (StreamReader sr = new StreamReader(jsonFile.FullName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    stringBuilder.AppendLine(line);
                }
            }

            SortedList<string, T> list = JSON.Deserialize<SortedList<string, T>>(stringBuilder.ToString());
            if (list != null)
            {
                foreach (string key in list.Keys)
                {
                    Save(list[key]);
                }
            }
            else
            {
                LogManager.GetLogger("Global").Warn(jsonFile.FullName + "was empty");
                //Console.WriteLine(jsonFile.FullName + "was empty");
            }
        }

        private void InitalizeInMemory()
        {
            _list = new SortedList<string, T>();
            _queryable = _list.Values.AsQueryable();
        }
    }
}
