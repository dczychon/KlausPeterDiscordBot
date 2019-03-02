using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DiscordBot.Util.RuntimeDebugging
{
    internal class SimpleTypeInfo
    {
        private readonly string _prefix;

        public string Name { get; }

        public Type ReturnType { get; }

        public bool IsVoidReturn
            => ReturnType.Name == "Void";

        public bool IsAutoPropertyMethod
            => Name.StartsWith("get_") || Name.StartsWith("set_");

        public SimpleTypeInfo(FieldInfo field)
        {
            Name = field.Name;
            ReturnType = field.FieldType;
            _prefix = "field";
        }

        public SimpleTypeInfo(PropertyInfo property)
        {
            Name = property.Name;
            ReturnType = property.PropertyType;
            _prefix = "prop";
        }

        //public SimpleTypeInfo(MethodInfo method)
        //{
        //    Name = method.Name;
        //    ReturnType = method.ReturnType;
        //    _prefix = "method";
        //}

        public static List<SimpleTypeInfo> GetAllFromType(Type type)
        {
            var allFromType = new List<SimpleTypeInfo>();
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                allFromType.Add(new SimpleTypeInfo(field));
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                allFromType.Add(new SimpleTypeInfo(property));
            }

            //foreach (MethodInfo method in type.GetMethods())
            //{
            //    allFromType.Add(new SimpleTypeInfo(method));
            //}

            return allFromType;
        }

        public List<SimpleTypeInfo> GetChildren()
        {
            if (!IsVoidReturn && !ReturnType.IsValueType && ReturnType.FullName != "System.String")
            {
                return GetAllFromType(ReturnType);
            }

            return new List<SimpleTypeInfo>(0);
        }

        public override string ToString()
        {
            return $"({_prefix}) <{ReturnType.Name}>{Name}";
        }
    }
}
