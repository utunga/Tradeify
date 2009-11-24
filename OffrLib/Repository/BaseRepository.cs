using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
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
                //String serializedList = JSON.Serialize(_list);             
            }
        }
        public void SerializeToFile(string FilePath)
        {
           String serializedList= JSON.Serialize(_list);
           if (FilePath == null) return;
           TextWriter tw = new StreamWriter(FilePath);
           tw.Write(serializedList);
           tw.Close();
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

        public void InitializeFromFile(string jsonFilePath)
        {
            FileInfo jsonFile;
            if (!(jsonFile = new FileInfo(jsonFilePath)).Exists)
            {
                throw new IOException("Cannot find file " + jsonFilePath);
            }

            // OK read into a string builder (probably a better way to do this but this will do for now)
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
            foreach (string key in list.Keys)
            {
               Save(list[key]);
            }
        }
    }
}
