using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Api.Helpers
    {
    public class APIMessageHelper
        {
        public static string ListNotFoundMessage(string entity)
            {
            return "No " + entity + " found";
            }

        public static string EntityNotFoundMessage(string entity, int id)
            {
            return entity + " with the provided Id " + id + " not found";
            }
        }
    }