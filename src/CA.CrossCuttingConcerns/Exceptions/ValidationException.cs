﻿using System;
using System.Collections.Generic;

namespace CA.CrossCuttingConcerns.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(string message) : base(message)
        {
        }

        //public ValidationException(List<ValidationFailure> failures)
        //    : this()
        //{
        //    var propertyNames = failures
        //        .Select(e => e.PropertyName)
        //        .Distinct();

        //    foreach (var propertyName in propertyNames)
        //    {
        //        var propertyFailures = failures
        //            .Where(e => e.PropertyName == propertyName)
        //            .Select(e => e.ErrorMessage)
        //            .ToArray();

        //        Failures.Add(propertyName, propertyFailures);
        //    }
        //}

        public IDictionary<string, string[]> Failures { get; }
    }
}
