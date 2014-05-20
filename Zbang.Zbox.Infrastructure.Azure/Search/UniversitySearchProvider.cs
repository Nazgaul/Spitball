using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store.Azure;
using SpellChecker.Net.Search.Spell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class UniversitySearchProvider : IUniversityWriteSearchProvider, IUniversityReadSearchProvider
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IZboxReadServiceWorkerRole m_DbReadService;
        private readonly AzureDirectory m_AzureUniversiesDirectory;
        private readonly AzureDirectory m_AzureUniversiesSpellerDirectory;

        const string universityCatalog = "UniversityCatalog";
        const string universitySuggestionCatalog = "UniversitySuggestionCatalog";


        const string IdField = "id";
        const string NameField = "name";
        const string ImageField = "image";
        const string MembersCountField = "membersCount";

        private readonly IndexSearcher m_IndexService;


        public UniversitySearchProvider(IBlobProvider blobProvider, IZboxReadServiceWorkerRole dbReadService)
        {
            m_BlobProvider = blobProvider;
            m_AzureUniversiesDirectory = new AzureDirectory(StorageProvider.ZboxCloudStorage, universityCatalog);
            m_AzureUniversiesSpellerDirectory = new AzureDirectory(StorageProvider.ZboxCloudStorage, universitySuggestionCatalog);
            m_DbReadService = dbReadService;
            m_IndexService = new IndexSearcher(m_AzureUniversiesDirectory, false);
        }
        public void BuildUniversityData()
        {
            var resource = LoadResource("UniversityData.txt");
            var universities = ConvertToObject(resource);
            BuildLucene(universities);
        }

        private async void BuildLucene(IEnumerable<University> universities)
        {

            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            {

                using (IndexWriter indexWriter = new IndexWriter(m_AzureUniversiesDirectory,
                        analyzer,
                        new Lucene.Net.Index.IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH)))
                {
                    foreach (var university in universities)
                    {
                        var extraDetail = await m_DbReadService.GetUniversityDetail(university.Id);
                        var searchQuery = new TermQuery(new Term("id", university.Id.ToString()));
                        indexWriter.DeleteDocuments(searchQuery);
                        //indexWriter.DeleteAll();
                        //indexWriter.Commit();

                        Document doc = new Document();
                        doc.Add(new Field(IdField, university.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        doc.Add(new Field(NameField, university.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));

                        doc.Add(new Field("extra1", university.Extra1, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                        doc.Add(new Field("extra2", university.Extra2, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                        doc.Add(new Field("extra3", university.Extra3, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));

                        doc.Add(new Field(ImageField, extraDetail.Image, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        doc.Add(new Field(MembersCountField, extraDetail.MemberCount.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));


                        indexWriter.AddDocument(doc);
                    }
                    using (SpellChecker.Net.Search.Spell.SpellChecker speller = new SpellChecker.Net.Search.Spell.SpellChecker(m_AzureUniversiesSpellerDirectory))
                    {
                        speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), NameField));
                        //m_AzureUniversiesSpellerDirectory.speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), "extra1"));
                        //speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), "extra2"));
                        //speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), "extra3"));

                    }
                    indexWriter.Optimize();
                    //IndexWriter indexWriter = new IndexWriter(azureDirectory, new StandardAnalyzer(), true);

                }
            }
        }

        public IEnumerable<UniversityByPrefixDto> SearchUniversity(string term)
        {
            // validation
            if (string.IsNullOrEmpty(term.Replace("*", "").Replace("?", "")))
            {
                return null;
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //using (var searcher = new IndexSearcher(m_AzureUniversiesDirectory, false))
            //{
                sw.Stop();
                Debug.WriteLine(sw.ElapsedMilliseconds);
                sw.Restart();
                using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                {
                    sw.Stop();
                    Debug.WriteLine(sw.ElapsedMilliseconds);
                    sw.Restart();
                    //using (SpellChecker.Net.Search.Spell.SpellChecker speller = new SpellChecker.Net.Search.Spell.SpellChecker(m_AzureUniversiesSpellerDirectory))
                    //{
                    //    string[] suggestions = speller.SuggestSimilar(term, 5);
                    //}

                    // search by multiple fields (ordered by RELEVANCE)
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_30, new[] { "name", "extra1", "extra2", "extra3" }, analyzer);
                    parser.AllowLeadingWildcard = true;
                    var query = parseQuery("*" + term + "*", parser);

                    var hits = m_IndexService.Search(query, 50).ScoreDocs;
                    var retVal = new List<UniversityByPrefixDto>();
                    for (int i = 0; i < hits.Length; i++)
                    {

                        Document doc2 = m_IndexService.Doc(hits[i].Doc);//.Doc(i);
                        UniversityByPrefixDto university = new UniversityByPrefixDto(
                            doc2.GetField(NameField).StringValue,
                            doc2.GetField(ImageField).StringValue,
                           Convert.ToInt64(doc2.GetField(IdField).StringValue),
                          Convert.ToInt64(doc2.GetField(MembersCountField).StringValue)
                            );

                        retVal.Add(university);
                        //Console.WriteLine(doc2.GetField("University").StringValue);


                    }
                    sw.Stop();
                    Debug.WriteLine(sw.ElapsedMilliseconds);


                    return retVal;
                }
           // }
            //return new List<SampleData>();
        }
        private Lucene.Net.Search.Query parseQuery(string searchQuery, QueryParser parser)
        {
            Lucene.Net.Search.Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
        private IEnumerable<University> ConvertToObject(string data)
        {
            var universities = new List<University>();
            var universitiesData = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var universitydata in universitiesData)
            {
                var splitData = universitydata.Split(new char[] { '\t' });

                long id = 0;
                long.TryParse(splitData[0], out id);
                var university = new University
                {
                    Id = long.Parse(splitData[0]),
                    Name = splitData[1].Trim(),
                    Extra1 = splitData[2] != null ? splitData[2].Trim() : null,
                    Extra2 = splitData[3] != null ? splitData[3].Trim() : null,
                    Extra3 = splitData[4] != null ? splitData[4].Trim() : null
                };
                //university.Extra = String.Join(" ", splitData.Skip(2)).Trim();

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
