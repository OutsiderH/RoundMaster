namespace RoundMaster.Reflection {
    using System.Linq.Expressions;
    using System.Reflection;
    using System;

    public abstract class ValueAccessor<Class, Value> {
        private readonly Func<Class, Value> getValue;
        private readonly Action<Class, Value> setValue;
        public ValueAccessor(MemberInfo info) {
            ParameterExpression parInstance = Expression.Parameter(typeof(Class));
            ParameterExpression parValue = Expression.Parameter(typeof(Value));
            MemberExpression memberAccess = Expression.MakeMemberAccess(parInstance, info);
            getValue = Expression.Lambda<Func<Class, Value>>(memberAccess, parInstance).Compile();
            setValue = Expression.Lambda<Action<Class, Value>>(Expression.Assign(memberAccess, parValue), parInstance, parValue).Compile();
        }
        public Value GetValue(Class instance) {
            return getValue(instance);
        }
        public void GetValueNonAlloc(Class instance, out Value value) {
            value = getValue(instance);
        }
        public void SetValue(Class instance, Value value) {
            setValue(instance, value);
        }
    }
    public class FieldAccessor<Class, Field> : ValueAccessor<Class, Field> {
        public FieldAccessor(FieldInfo info) : base(
            (info == null || info.DeclaringType != typeof(Class) || info.FieldType != typeof(Field))
                ? throw new ArgumentException("FieldInfo must match the generic parameters")
                : info) {}
    }
    public class PropertyAccessor<Class, Property> : ValueAccessor<Class, Property> {
        public PropertyAccessor(PropertyInfo info) : base(
            (info == null || info.DeclaringType != typeof(Class) || info.PropertyType != typeof(Property))
                ? throw new ArgumentException("PropertyInfo must match the generic parameters")
                : info) {}
    }
    public static class BindingFlagsPreset {
        public const BindingFlags publicInstance =
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |
            BindingFlags.Instance;
        public const BindingFlags publicStatic =
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |
            BindingFlags.Static;
        public const BindingFlags nonPublicInstance =
            BindingFlags.DeclaredOnly |
            BindingFlags.NonPublic |
            BindingFlags.Instance;
        public const BindingFlags nonPublicStatic =
            BindingFlags.DeclaredOnly |
            BindingFlags.NonPublic |
            BindingFlags.Static;
        public const BindingFlags allInstance =
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance;
        public const BindingFlags allStatic =
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static;
    }
}