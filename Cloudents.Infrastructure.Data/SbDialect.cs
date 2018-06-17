using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace Cloudents.Infrastructure.Data
{
    public class SbDialect : MsSql2012Dialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            //RegisterFunction("freeText", new StandardSQLFunction("freetext", null));
            //RegisterFunction("FullTextContains", new ContainsFunction("contains", null));
            RegisterFunction("FullTextContains", new StandardSQLFunction("contains", null));
        }
    }


    //public class ContainsFunction : ISQLFunction
    //{
    //    private IType returnType = null;
    //    protected readonly string name;

    //    /// <summary>
    //    /// Initializes a new instance of the StandardSQLFunction class.
    //    /// </summary>
    //    /// <param name="name">SQL function name.</param>
    //    public ContainsFunction(string name)
    //    {
    //        this.name = name;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the StandardSQLFunction class.
    //    /// </summary>
    //    /// <param name="name">SQL function name.</param>
    //    /// <param name="typeValue">Return type for the function.</param>
    //    public ContainsFunction(string name, IType typeValue)
    //        : this(name)
    //    {
    //        returnType = typeValue;
    //    }


    //    public IType ReturnType(IType columnType, IMapping mapping)
    //    {
    //        return returnType ?? columnType;
    //    }

    //    public bool HasArguments
    //    {
    //        get { return true; }
    //    }

    //    public bool HasParenthesesIfNoArguments
    //    {
    //        get { return true; }
    //    }

    //    public virtual SqlString Render(IList args, ISessionFactoryImplementor factory)
    //    {
    //        //SqlStringBuilder buf = new SqlStringBuilder();
    //        //buf.Add(name)
    //        //    .Add("(");
    //        //for (int i = 0; i < args.Count; i++)
    //        //{
    //        //    object arg = args[i];
    //        //    if (arg is Parameter || arg is SqlString)
    //        //    {
    //        //        buf.AddObject(arg);
    //        //    }
    //        //    else
    //        //    {
    //        //        buf.Add(arg.ToString());
    //        //    }
    //        //    if (i < (args.Count - 1)) buf.Add(", ");
    //        //}
    //        //return buf.Add(")").ToSqlString();

    //        //contains(this_.Text, ?)
    //        SqlStringBuilder buf = new SqlStringBuilder();

    //        if (args.Count != 2)
    //        {
    //            throw new ArgumentOutOfRangeException();
    //        }

    //        buf.Add("CONTAINS(");
    //        buf.AddObject(args[0]).Add(", ");
    //        buf.Add("'\"").AddObject(args[1]).Add("\"')");
    //        return buf.ToSqlString();
    //    }
    //}
}