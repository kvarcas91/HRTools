using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;

namespace Domain.Factory
{
    public struct DataMap
    {
        private Dictionary<string, int> _data;
        private Dictionary<string, string> _propertyNamesMap;
        public DataImportType DataImportType { get; set; }

        public DataMap(IDataMap dataMap, DataImportType dataType)
        {
            DataImportType = dataType;
            _data = new Dictionary<string, int>();
            _propertyNamesMap = dataMap.GetRequiredHeaders();
        }

        public DataMap SetDataMap(string[] headers)
        {
            foreach (var item in _propertyNamesMap)
            {
                _data.Add(item.Key, Array.IndexOf(headers, item.Value));
            }

            return this;
        }

        public string GetStrValue(string prop, string[] list)
        {
            var index = GetIndex(prop);
            if (index == -1) return null;
            return list[index];
        }

        public DateTime GetDateValue(string prop, string[] list)
        {
            var index = GetIndex(prop);
            if (index == -1) return DateTime.MinValue;
            var value = list[index];
            if (value.Contains("UTC"))
            {
                DateTime.TryParseExact(value, DataStorage.AppSettings.RosterWebDateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime date);
                return date;
            }

            return DateTime.TryParse(value, out DateTime dateTime) ? dateTime : DateTime.MinValue;

        }

        public int GetIndex(string prop)
        {
            return _data.ContainsKey(prop) ? _data[prop] : -1;
        }
    }
}
