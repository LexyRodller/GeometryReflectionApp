using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GeometryReflectionApp.Services
{
    public class ReflectionService
    {
        public Assembly? LoadAssembly(string path)
        {
            try { return Assembly.LoadFrom(path); }
            catch { return null; }
        }

        public IEnumerable<Type> GetShapeTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(Shape).IsAssignableFrom(t));
        }

        public Shape? CreateShape(Type type, object?[] parameters)
        {
            try
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                
                var instance = Activator.CreateInstance(type, parameters);
                if (instance is Shape shape)
                    return shape;
                
                return null;
            }
            catch (MissingMethodException)
            {
                // Попробуем конструктор без параметров
                try
                {
                    return Activator.CreateInstance(type) as Shape;
                }
                catch
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateShape error: {ex}");
                return null;
            }
        }

        public object? InvokeMethod(Shape instance, MethodInfo method, object?[] parameters)
        {
            try
            {
                return method.Invoke(instance, parameters);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.InnerException?.Message ?? ex.Message}";
            }
        }
    }
}