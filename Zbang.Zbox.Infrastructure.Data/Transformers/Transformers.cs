using NHibernate.Transform;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Data.Transformers
{
    public static class Transformers
    {
        public static IResultTransformer AliasToDerivedClasses(params System.Type[] types)
        {
            return new AliasToDerivedClassesTransformer(types);
        }

        public static IResultTransformer AliasToDerivedClassesCtorTransformer(params System.Type[] types)
        {
            return new AliasToDerivedClassesCtorTransformer(types);
        }

        public static IResultTransformer AliasToCompositeClasses<T>() where T : class, IComposite<T>
        {
            return new AliasToCompositeClassTransformer<T>();
        }
    }
}
