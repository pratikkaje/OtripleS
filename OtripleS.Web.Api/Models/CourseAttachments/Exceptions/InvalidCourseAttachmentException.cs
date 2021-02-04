﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtripleS.Web.Api.Models.CourseAttachments.Exceptions
{
    public class InvalidCourseAttachmentException : Exception
    {
        public InvalidCourseAttachmentException(string parameterName, object parameterValue)
            : base($"Invalid Course Attachment, " +
                  $"ParameterName: {parameterName}, " +
                  $"ParameterValue: {parameterValue}.")
        { }
    }
}
