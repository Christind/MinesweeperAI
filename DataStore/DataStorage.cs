using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class DataStorage
    {

        private static DataStorage _instance;

        public static DataStorage GetInstance()
        {
            return _instance ?? (_instance = new DataStorage());
        }

        public Dictionary<string, dynamic> Data { get; private set; } 

        private DataStorage()
        {
            Data = new Dictionary<string, dynamic>();
        }
    }
}
