using System.Globalization;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store.Azure;
using SpellChecker.Net.Search.Spell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.Library;
using System.Timers;
using Zbang.Zbox.Infrastructure.Trace;
using Lucene.Net.Analysis;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class UniversitySearchProvider : IUniversityWriteSearchProvider, IUniversityReadSearchProvider, IDisposable
    {
        private readonly IZboxReadServiceWorkerRole m_DbReadService;
        private readonly AzureDirectory m_AzureUniversiesDirectory;
        private readonly AzureDirectory m_AzureUniversiesSpellerDirectory;

        const string UniversityCatalog = "UniversityCatalog";
        const string UniversitySuggestionCatalog = "UniversitySuggestionCatalog";


        const string IdField = "id";
        const string NameField = "name";
        const string ImageField = "image";
        const string MembersCountField = "membersCount";

        private IndexSearcher m_IndexService;
        private readonly Timer m_Timer;

        public UniversitySearchProvider(IZboxReadServiceWorkerRole dbReadService)
        {
            m_AzureUniversiesDirectory = new AzureDirectory(StorageProvider.ZboxCloudStorage, UniversityCatalog);
            m_AzureUniversiesSpellerDirectory = new AzureDirectory(StorageProvider.ZboxCloudStorage, UniversitySuggestionCatalog);
            m_DbReadService = dbReadService;
            //m_IndexService = new IndexSearcher(m_AzureUniversiesDirectory, false);

            m_Timer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            m_Timer.Elapsed += (s, e) =>
            {
                m_IndexService.Dispose();
                m_IndexService = null;
            //    m_IndexService = new IndexSearcher(m_AzureUniversiesDirectory, false);
            };
            m_Timer.Enabled = true;

        }
        public void BuildUniversityData()
        {
            var resource = LoadResource("UniversityData.txt");
            var universities = ConvertToObject(resource);
            BuildLucene(universities);
        }

        private async void BuildLucene(IEnumerable<University> universitiesExtra)
        {

            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            {
                universitiesExtra = universitiesExtra.ToList();
                using (var indexWriter = new IndexWriter(m_AzureUniversiesDirectory,
                        analyzer,
                        new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH)))
                {
                    var universities = await m_DbReadService.GetUniversityDetail();
                    indexWriter.DeleteAll();
                    indexWriter.Commit();
                    foreach (var university in universities)
                    {
                        var extraDetail = universitiesExtra.FirstOrDefault(f => f.Id == university.Id);
                        //var searchQuery = new TermQuery(new Term("id", university.Id.ToString(CultureInfo.InvariantCulture)));
                        //indexWriter.DeleteDocuments(searchQuery);
                       

                        var doc = new Document();
                        doc.Add(new Field(IdField, university.Id.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        doc.Add(new Field(NameField, university.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                        if (extraDetail != null)
                        {
                            doc.Add(new Field("extra1", extraDetail.Extra1, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                            doc.Add(new Field("extra2", extraDetail.Extra2, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                            doc.Add(new Field("extra3", extraDetail.Extra3, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                        }
                        doc.Add(new Field(ImageField, university.Image, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        doc.Add(new Field(MembersCountField, university.MemberCount.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));


                        indexWriter.AddDocument(doc);
                    }
                    indexWriter.Commit();
                    indexWriter.Optimize();
                    try
                    {
                        using (
                            var speller =
                                new SpellChecker.Net.Search.Spell.SpellChecker(m_AzureUniversiesSpellerDirectory))
                        {
                            speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), NameField));
                            speller.Close();
                            //m_AzureUniversiesSpellerDirectory.speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), "extra1"));
                            //speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), "extra2"));
                            //speller.IndexDictionary(new LuceneDictionary(indexWriter.GetReader(), "extra3"));

                        }
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("On build lucene speller", ex);
                    }

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
            HashSet<string> extraWords = new HashSet<string>(StopAnalyzer.ENGLISH_STOP_WORDS_SET);

            extraWords.Add("college");
            extraWords.Add("university");
            extraWords.Add("אוניברסיטה");
            extraWords.Add("ה");


            
            //using (var searcher = new IndexSearcher(m_AzureUniversiesDirectory, false))
            //{
            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30, extraWords))
            {

                //using (SpellChecker.Net.Search.Spell.SpellChecker speller = new SpellChecker.Net.Search.Spell.SpellChecker(m_AzureUniversiesSpellerDirectory))
                //{
                //    string[] suggestions = speller.SuggestSimilar(term, 5);
                //    Debug.WriteLine(string.Join(",", suggestions));
                //}

                // search by multiple fields (ordered by RELEVANCE)
                var parser = new MultiFieldQueryParser
                    (Lucene.Net.Util.Version.LUCENE_30, new[] { "name", "extra1", "extra2", "extra3" }, analyzer)
                {
                    AllowLeadingWildcard = true
                };

                //term = term.Replace(" ", "* *");
                var searchTerm = term + "*";
                var query = parseQuery(searchTerm, parser);
                var values = ProcessHits(query);
                //var hits = m_IndexService.Search(query, 50).ScoreDocs;
                //var retVal = new List<UniversityByPrefixDto>();
                //for (int i = 0; i < hits.Length; i++)
                //{

                //    Document doc2 = m_IndexService.Doc(hits[i].Doc);//.Doc(i);
                //    var university = new UniversityByPrefixDto(
                //        doc2.GetField(NameField).StringValue,
                //        doc2.GetField(ImageField).StringValue,
                //       Convert.ToInt64(doc2.GetField(IdField).StringValue),
                //      Convert.ToInt64(doc2.GetField(MembersCountField).StringValue)
                //        );

                //    retVal.Add(university);
                //    //Console.WriteLine(doc2.GetField("University").StringValue);


                //}
                if (values.Count != 0) return values;
                var similarSearchTerm = term.Replace(" ", "* *");
                similarSearchTerm = "*" + similarSearchTerm + "*";
                var extendQuery = parseQuery(similarSearchTerm, parser);
                values = ProcessHits(extendQuery);
                return values;
            }
            // }
            //return new List<SampleData>();
        }

        private IList<UniversityByPrefixDto> ProcessHits(Lucene.Net.Search.Query query)
        {
            if (m_IndexService == null)
            {
                m_IndexService = new IndexSearcher(m_AzureUniversiesDirectory, false);
            }
            var hits = m_IndexService.Search(query, 20).ScoreDocs;
            var retVal = new List<UniversityByPrefixDto>();
            for (int i = 0; i < hits.Length; i++)
            {

                Document doc2 = m_IndexService.Doc(hits[i].Doc);//.Doc(i);
                var university = new UniversityByPrefixDto(
                    doc2.GetField(NameField).StringValue,
                    doc2.GetField(ImageField).StringValue,
                   Convert.ToInt64(doc2.GetField(IdField).StringValue),
                  Convert.ToInt64(doc2.GetField(MembersCountField).StringValue)
                    );

                retVal.Add(university);
                //Console.WriteLine(doc2.GetField("University").StringValue);


            }
            return retVal;
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
            var universitiesData = data.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var universitydata in universitiesData)
            {
                var splitData = universitydata.Split(new[] { ',' });

                long id;
                long.TryParse(splitData[0], out id);
                var university = new University
                {
                    Id = long.Parse(splitData[0]),
                    // Name = splitData[1].Trim(),
                    Extra1 = splitData[1] != null ? splitData[1].Trim() : null,
                    Extra2 = splitData[2] != null ? splitData[2].Trim() : null,
                    Extra3 = splitData[3] != null ? splitData[3].Trim() : null
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

        public void Dispose()
        {
            if (m_IndexService != null)
            {
                m_IndexService.Dispose();
            }
            m_AzureUniversiesDirectory.Dispose();
            m_AzureUniversiesSpellerDirectory.Dispose();
            m_Timer.Dispose();
        }
    }



}
