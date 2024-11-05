using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>
    /// Class representing an issue detected during message processing.
    /// </summary>
    public class Issue
    {
        /// <summary>
        /// Severity of the issue
        /// </summary>
        public readonly OperationOutcome.IssueSeverity? Severity;

        /// <summary>
        /// Type of the issue
        /// </summary>
        public readonly OperationOutcome.IssueType? Type;

        /// <summary>
        /// Human readable description of the issue
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Construct a new Issue
        /// </summary>
        /// <param name="severity">the severity of the issue</param>
        /// <param name="type">the type of issue</param>
        /// <param name="description">a human readable description of the issue</param>
        public Issue(OperationOutcome.IssueSeverity? severity, OperationOutcome.IssueType? type, string description)
        {
            Severity = severity;
            Type = type;
            Description = description;
        }
    }
}