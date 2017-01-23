using System.Linq;

namespace ShaneYu.HotCommander.Helpers
{
    // TODO: Update copy code to take into account object properties...deep copying...

    public static class PropertyCopier
    {
        public static void Copy<T>(T source, T target)
            where T: class
        {
            var properties = source.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                property.SetValue(target, property.GetValue(source));
            }
        }

        public static T Copy<T>(T source)
            where T: class, new()
        {
            var target = new T();
            Copy(source, target);
            return target;
        }
    }

    /*
    /// <summary>
    /// Non-generic class allowing properties to be copied from one instance
    /// to another existing instance of a potentially different type.
    /// </summary>
    public static class PropertyCopier
    {
        /// <summary>
        /// Copies all public, readable properties from the source object to the
        /// target. The target type does not have to have a parameterless constructor,
        /// as no new instance needs to be created.
        /// </summary>
        /// <remarks>Only the properties of the source and target types themselves
        /// are taken into account, regardless of the actual types of the arguments.</remarks>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <param name="source">Source to copy properties from</param>
        /// <param name="target">Target to copy properties to</param>
        public static void Copy<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            PropertyCopier<TSource, TTarget>.Copy(source, target);
        }
    }

    /// <summary>
    /// Generic class which copies to its target type from a source
    /// type specified in the Copy method. The types are specified
    /// separately to take advantage of type inference on generic
    /// method arguments.
    /// </summary>
    public static class PropertyCopier<TTarget> where TTarget : class, new()
    {
        /// <summary>
        /// Copies all readable properties from the source to a new instance
        /// of TTarget.
        /// </summary>
        public static TTarget CopyFrom<TSource>(TSource source) where TSource : class
        {
            return PropertyCopier<TSource, TTarget>.Copy(source);
        }
    }

    /// <summary>
    /// Static class to efficiently store the compiled delegate which can
    /// do the copying. We need a bit of work to ensure that exceptions are
    /// appropriately propagated, as the exception is generated at type initialization
    /// time, but we wish it to be thrown as an ArgumentException.
    /// Note that this type we do not have a constructor constraint on TTarget, because
    /// we only use the constructor when we use the form which creates a new instance.
    /// </summary>
    internal static class PropertyCopier<TSource, TTarget>
    {
        /// <summary>
        /// Delegate to create a new instance of the target type given an instance of the
        /// source type. This is a single delegate from an expression tree.
        /// </summary>
        private static readonly Func<TSource, TTarget> Creator;

        /// <summary>
        /// List of properties to grab values from. The corresponding targetProperties 
        /// list contains the same properties in the target type. Unfortunately we can't
        /// use expression trees to do this, because we basically need a sequence of statements.
        /// We could build a DynamicMethod, but that's significantly more work :) Please mail
        /// me if you really need this...
        /// </summary>
        private static readonly List<PropertyInfo> SourceProperties = new List<PropertyInfo>();
        private static readonly List<PropertyInfo> TargetProperties = new List<PropertyInfo>();
        private static readonly Exception InitializationException;

        internal static TTarget Copy(TSource source)
        {
            if (InitializationException != null)
            {
                throw InitializationException;
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Creator(source);
        }

        internal static void Copy(TSource source, TTarget target)
        {
            if (InitializationException != null)
            {
                throw InitializationException;
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            for (var i = 0; i < SourceProperties.Count; i++)
            {
                TargetProperties[i].SetValue(target, SourceProperties[i].GetValue(source, null), null);
            }
        }

        static PropertyCopier()
        {
            try
            {
                Creator = BuildCreator();
                InitializationException = null;
            }
            catch (Exception e)
            {
                Creator = null;
                InitializationException = e;
            }
        }

        private static Func<TSource, TTarget> BuildCreator()
        {
            var sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var bindings = new List<MemberBinding>();

            foreach (var sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!sourceProperty.CanRead)
                {
                    continue;
                }

                var targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);

                if (targetProperty == null || !targetProperty.CanWrite ||
                    targetProperty.GetCustomAttribute<JsonIgnoreAttribute>() != null ||
                    (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0 ||
                    !targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    continue;
                }

                bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                SourceProperties.Add(sourceProperty);
                TargetProperties.Add(targetProperty);
            }

            Expression initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);

            return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
        }
    }*/
}
