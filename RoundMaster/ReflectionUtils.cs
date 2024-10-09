namespace RoundMaster.Reflection {
    using System.Linq.Expressions;
    using System.Reflection;
    using System;

    public abstract class MemberAccessor<Class, Member> {
        private readonly Func<Class, Member> getValue;
        private readonly Action<Class, Member> setValue;
        public MemberAccessor(MemberInfo info) {
            ParameterExpression parInstance = Expression.Parameter(typeof(Class));
            ParameterExpression parValue = Expression.Parameter(typeof(Member));
            MemberExpression memberAccess = Expression.MakeMemberAccess(parInstance, info);
            getValue = Expression.Lambda<Func<Class, Member>>(memberAccess, parInstance).Compile();
            setValue = Expression.Lambda<Action<Class, Member>>(Expression.Assign(memberAccess, parValue), parInstance, parValue).Compile();
        }
        public Member GetValue(Class instance) {
            return getValue(instance);
        }
        public void GetValueNonAlloc(Class instance, out Member value) {
            value = getValue(instance);
        }
        public void SetValue(Class instance, Member value) {
            setValue(instance, value);
        }
    }
    public class FieldAccessor<Class, Field> : MemberAccessor<Class, Field> {
        public FieldAccessor(FieldInfo info) : base(
            (info == null || info.DeclaringType != typeof(Class) || info.FieldType != typeof(Field))
            ? throw new ArgumentException("FieldInfo must match the generic parameters")
            : info) {}
    }
    public class PropertyAccessor<Class, Property> : MemberAccessor<Class, Property> {
        public PropertyAccessor(PropertyInfo info) : base(
            (info == null || info.DeclaringType != typeof(Class) || info.PropertyType != typeof(Property))
            ? throw new ArgumentException("PropertyInfo must match the generic parameters")
            : info) {}
    }
}