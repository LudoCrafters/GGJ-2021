using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace UnityEditor.PostProcessing
{
    public static class ReflectionUtils
    {
        private static readonly Dictionary<KeyValuePair<object, string>, FieldInfo> s_FieldInfoFromPaths = new Dictionary<KeyValuePair<object, string>, FieldInfo>();

        public static FieldInfo GetFieldInfoFromPath(object source, string path)
        {
            KeyValuePair<object, string> kvp = new KeyValuePair<object, string>(source, path);

            if (!s_FieldInfoFromPaths.TryGetValue(kvp, out FieldInfo field))
            {
                string[] splittedPath = path.Split('.');
                Type type = source.GetType();

                foreach (string t in splittedPath)
                {
                    field = type.GetField(t, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (field == null)
                    {
                        break;
                    }

                    type = field.FieldType;
                }

                s_FieldInfoFromPaths.Add(kvp, field);
            }

            return field;
        }

        public static string GetFieldPath<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryExpression ue = expr.Body as UnaryExpression;
                    me = (ue != null ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            List<string> members = new List<string>();
            while (me != null)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                sb.Append(members[i]);
                if (i > 0)
                {
                    sb.Append('.');
                }
            }

            return sb.ToString();
        }

        public static object GetFieldValue(object source, string name)
        {
            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                {
                    return f.GetValue(source);
                }

                type = type.BaseType;
            }

            return null;
        }

        public static object GetFieldValueFromPath(object source, ref Type baseType, string path)
        {
            string[] splittedPath = path.Split('.');
            object srcObject = source;

            foreach (string t in splittedPath)
            {
                FieldInfo fieldInfo = baseType.GetField(t, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (fieldInfo == null)
                {
                    baseType = null;
                    break;
                }

                baseType = fieldInfo.FieldType;
                srcObject = GetFieldValue(srcObject, t);
            }

            return baseType == null
                   ? null
                   : srcObject;
        }

        public static object GetParentObject(string path, object obj)
        {
            string[] fields = path.Split('.');

            if (fields.Length == 1)
            {
                return obj;
            }

            FieldInfo info = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            obj = info.GetValue(obj);

            return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
        }
    }
}
