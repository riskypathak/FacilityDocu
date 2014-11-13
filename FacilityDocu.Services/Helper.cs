using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacilityDocu.Services
{
    public static class Helper
    {
        public static bool IsNew(string id)
        {
            bool returnValue;
            returnValue = (string.IsNullOrEmpty(id) || id.Length >= 15 || id.Equals("0") || id.StartsWith("-")) ? true : false;

            return returnValue;
        }

        public static int GetUniqueID()
        {
             Random rand = new Random();

             return rand.Next(int.MinValue, -1);
        }
    }
}