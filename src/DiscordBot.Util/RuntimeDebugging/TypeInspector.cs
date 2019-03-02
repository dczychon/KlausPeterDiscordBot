using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DiscordBot.Util.RuntimeDebugging
{
    class TypeInspector<T>
    {
        private const string _treeMarker = "├──";
        private const string _lastTreeMarker = "└──";
        private const string _emptyTreeMarker = "│";

        private readonly T _inspectObject;

        public TypeInspector(T objectToInspect)
        {
            _inspectObject = objectToInspect;
        }

        public object GetValue(string path)
        {
            string[] areas = path.Trim().Split('.');

            object currentObject = _inspectObject;

            for (int i = 0; i < areas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(areas[i]))
                {
                    throw new ArgumentException("Path can´t start or end with a dot and must contain at least one character between two dots");
                }

                bool foundField = false;

                foreach (FieldInfo field in currentObject.GetType().GetFields())
                {
                    if (string.Equals(field.Name, areas[i], StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (areas.Length == i + 1)
                        {
                            //This is the last area and we found a field
                            return field.GetValue(currentObject) ?? null;
                        }

                        foundField = true;
                        currentObject = field.GetValue(currentObject);
                        break;
                    }
                }

                if (!foundField)
                {
                    foreach (PropertyInfo property in currentObject.GetType().GetProperties())
                    {
                        if (string.Equals(property.Name, areas[i], StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (areas.Length == i + 1)
                            {
                                //This is the last area and we found a property
                                return property.GetValue(currentObject) ?? null;
                            }

                            currentObject = property.GetValue(currentObject);
                            break;
                        }
                    }
                }
            }

            throw new Exception("Path not found");
        }


        public string PrettyPrint(string path = null)
        {
            var all = SimpleTypeInfo.GetAllFromType(_inspectObject.GetType());
            var last = all.LastOrDefault();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"<{_inspectObject.GetType().Name}>");
            foreach (SimpleTypeInfo typeInfo in all)
            {
                builder.Append(PrettyPrintSimpleType(typeInfo, "", typeInfo == last));
            }

            builder.Replace("`", string.Empty);
            return builder.ToString();
        }

        internal StringBuilder PrettyPrintSimpleType(SimpleTypeInfo type, string lineStart = "", bool isLast = false)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(lineStart);
            strBuilder.Append(isLast ? _lastTreeMarker : _treeMarker);
            strBuilder.AppendLine(type.ToString());

            var children = type.GetChildren().ToList();
            var lastChild = children.LastOrDefault();

            foreach (SimpleTypeInfo simpleTypeInfo in children)
            {
                if (simpleTypeInfo.ReturnType != type.ReturnType)
                {
                    strBuilder.Append(PrettyPrintSimpleType(simpleTypeInfo, lineStart += isLast ? "   " : _emptyTreeMarker + "  ", simpleTypeInfo == lastChild));
                }
                else
                {
                    //Return type is the same as itself
                    strBuilder.Append("recursive");
                }
            }

            return strBuilder;
        }
    }
}
