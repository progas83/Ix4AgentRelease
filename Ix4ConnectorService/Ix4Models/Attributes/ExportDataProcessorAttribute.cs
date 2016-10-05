using Ix4Models.Interfaces;
using System;
using System.ComponentModel.Composition;

namespace Ix4Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class ExportDataProcessorAttribute : ExportAttribute, INameMetadata
    {
        public string Name { get; private set; }
        public ExportDataProcessorAttribute(string name) : base (typeof(IDataProcessor))
        {
            Name = name;
        }
    }
}
