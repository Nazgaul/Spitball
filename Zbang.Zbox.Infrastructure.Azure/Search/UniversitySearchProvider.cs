using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class UniversitySearchProvider
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly AzureDirectory m_AzureUniversiesDirectory;

        public UniversitySearchProvider(IBlobProvider blobProvider)
        {
            m_BlobProvider = blobProvider;
            m_AzureUniversiesDirectory = new AzureDirectory(StorageProvider.ZboxCloudStorage, "SearchUniversities");
        }
        public void BuildUniversityData()
        {
            var resource = LoadResource("UniversityData.txt");
            var universities = ConvertToObject(resource);
        }

        private void BuildLucene(IEnumerable<University> universities)
        {
            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            {
                using (IndexWriter indexWriter = new IndexWriter(m_AzureUniversiesDirectory,
                        analyzer,
                        new Lucene.Net.Index.IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH)))
                {
                    indexWriter.DeleteAll();
                    indexWriter.Commit();
                    foreach (var university in universities)
                    {
                        Document doc = new Document();
                        doc.Add(new Field("id", university.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        doc.Add(new Field("University", university.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                        indexWriter.AddDocument(doc);
                    }
                    indexWriter.Optimize();
                    //IndexWriter indexWriter = new IndexWriter(azureDirectory, new StandardAnalyzer(), true);

                }
            }
        }

        private IEnumerable<University> ConvertToObject(string data)
        {
            var universities = new List<University>();
            var universitiesData = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var universitydata in universitiesData)
            {
                var splitData = universitydata.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                long id = 0;
                long.TryParse(splitData[0], out id);
                var university = new University { Id = long.Parse(splitData[0]), Name = splitData[1].Trim() };
                university.Extra = String.Join(" ", splitData.Skip(2)).Trim();

                universities.Add(university);
            }

            return universities;

        }

        private string LoadResource(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Zbang.Zbox.Infrastructure.Azure.Search." + resourceName))
            {
                if (stream != null)
                {
                    var content = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(content, 0, (int)stream.Length);
                    return Encoding.UTF8.GetString(content).Replace(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()), string.Empty);
                }
                return string.Empty;
            }
        }
    }
}
