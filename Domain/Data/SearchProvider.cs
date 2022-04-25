using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class SearchProvider<T> where T : ISearchable
    {
        private int _searchIteration;
        private readonly int _searchIterationCap;
        private string lookupValue;
        private IEnumerable<T> _list;

        public bool IsSearching { get; set; }

        public SearchProvider()
        {
            _searchIteration = 0;
            _searchIterationCap = 5;
        }

        public void SetValues(string key)
        {
            lookupValue = key;
            if (IsSearching) _searchIteration = 0;
        }

        public void SetValues(string key, IEnumerable<T> list)
        {
            lookupValue = key;
            _list = list;

            if (IsSearching) _searchIteration = 0;
        }

        public Task<List<T>> LookUpAsync()
        {
            if (_list == null) throw new ArgumentNullException();

            IsSearching = true;
            return Task.Run(() =>
            {

                while (_searchIteration < _searchIterationCap)
                {
                    _searchIteration++;
                    Task.Delay(100).Wait();
                }

                List<T> output = new List<T>();

                foreach (T item in _list)
                {
                    if (item.HasValue(lookupValue)) output.Add(item);
                    
                }

                _searchIteration = 0;
                IsSearching = false;

                return output;
            });
        }
    }
}
