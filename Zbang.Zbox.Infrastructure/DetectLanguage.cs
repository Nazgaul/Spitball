using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using NTextCat;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Zbox.Infrastructure
{
    public interface IDetectLanguage
    {
        Language DoWork(string text);
    }
    public class DetectLanguage : IDetectLanguage
    {
        private readonly RankedLanguageIdentifier m_Identifier;
        public DetectLanguage()
        {
            var factory = new RankedLanguageIdentifierFactory();
            if (HttpContext.Current != null)
            {
                var path = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = path.Replace("file:\\", string.Empty);

                m_Identifier = factory.Load(Path.Combine(path, "Wiki82.profile.xml"));
            }
            else
            {
                m_Identifier = factory.Load("Wiki82.profile.xml");
            }
        }

        public Language DoWork(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Language.Undefined;
            }
            var languages = m_Identifier.Identify(text);
            var mostCertainLanguage = languages.FirstOrDefault();

            //if (mostCertainLanguage?.Item1.Iso639_3 == "simple")
            //    return Language.EnglishUs;
            if (mostCertainLanguage?.Item1.Iso639_3 == "en")
                return Language.EnglishUs;
            if (mostCertainLanguage?.Item1.Iso639_3 == "he")
                return Language.Hebrew;
            return Language.Undefined;
        }
    }
}
