using System;
using System.Reflection;

namespace GeometryReflectionApp.ViewModels
{
    public class MethodParameterViewModel
    {
        public string Name { get; set; } = string.Empty;
        public Type Type { get; set; } = typeof(object);
        public object? Value { get; set; }

        public MethodParameterViewModel(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
                throw new ArgumentNullException(nameof(parameterInfo));

            Name = parameterInfo.Name ?? "unnamed";
            Type = parameterInfo.ParameterType;
            Value = parameterInfo.HasDefaultValue 
                ? parameterInfo.DefaultValue 
                : parameterInfo.ParameterType.IsValueType 
                    ? Activator.CreateInstance(parameterInfo.ParameterType) 
                    : null;
        }
    }
}