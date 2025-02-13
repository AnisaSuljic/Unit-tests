﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PerfectSound.Filters
{
    public class ErrorFilter:ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentNullException)
            {
                context.ModelState.AddModelError("ERROR", context.Exception.Message);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (context.Exception is DbUpdateException)
            {
                context.ModelState.AddModelError("ERROR", context.Exception.Message + ", or you can't delete records before " +
                    "deleting records in linked tables. ");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                context.ModelState.AddModelError("ERROR", "Server error!");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var list = context.ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(x => x.Key, y => y.Value.Errors.Select(z => z.ErrorMessage));

            context.Result = new JsonResult(list);
        }
    }
}
