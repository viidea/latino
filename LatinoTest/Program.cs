using System;
using System.IO;
using Latino;
using Latino.Model;
using Latino.TextMining;
using Latino.Visualization;

using SparseVector = Latino.SparseVector<double>.ReadOnly;
using System.Collections.Generic;

namespace LatinoTest
{
    class Program
    {
        static string GetKeywordsStr<LblT>(Cluster cluster, int n, IExampleCollection<LblT, SparseVector> dataset, BowSpace bow_space)
        {
            string keywords_str = "KEYWORDS: ";
            ArrayList<KeyDat<double, string>> keywords = new ArrayList<KeyDat<double, string>>();
            foreach (IdxDat<double> item in cluster.ComputeCentroid(dataset, CentroidType.NrmL2))
            {
                keywords.Add(new KeyDat<double, string>(item.Dat, bow_space.Words[item.Idx].MostFrequentForm));
            }
            keywords.Sort(new DescSort<KeyDat<double, string>>());
            if (keywords.Count > 0)
            {
                keywords_str += keywords[0].Dat.ToUpper();
                for (int i = 1; i < Math.Min(5, keywords.Count); i++)
                {
                    keywords_str += ", " + keywords[i].Dat.ToUpper();
                }
            }
            return keywords_str;
        }




        class DistFunc : IDistance<int>
        {
            private SparseMatrix<double>.ReadOnly m_sim_mtx;
            
            public DistFunc(SparseMatrix<double>.ReadOnly sim_mtx)
            {
                m_sim_mtx = sim_mtx;
            }

            public double GetDistance(int a, int b)
            {
                try
                {
                    if (a > b) { return m_sim_mtx[b, a]; }
                    else { return m_sim_mtx[a, b]; }
                }
                catch 
                {
                    return 0;
                }
            }

            public void Save(BinarySerializer dummy)
            {
                throw new NotImplementedException();
            }
        }



        static void Main(string[] args)
        {


            //SortedDictionary<int, string> waka = new SortedDictionary<int, string>();
            //waka.Add(2, "berta");
            //waka.Add(1, "abrams");

            //foreach (string value in waka.Values)
            //{
            //    Console.WriteLine(value);
            //}

            //return;

            DocumentAtlas.Exec();
            return;



            //SparseVectorDataset<int> ds = new SparseVectorDataset<int>();
            //ds.Add(0, new SparseVector<double>(new double[] { 1, 2, 3 }));
            //ds.Add(0, new SparseVector<double>(new double[] { 4, 5, 6 }));
            //ds.Add(0, new SparseVector<double>(new IdxDat<double>[] { new IdxDat<double>(0, 7), new IdxDat<double>(1, 8), new IdxDat<double>(3, 10) }));

            //double[] dpsim = ds.GetDotProductSimilarity(new SparseVector<double>(new double[] { 1, 2, 3 }));

            //Console.WriteLine(new ArrayList<double>(dpsim));
            //Console.WriteLine(ds.GetDotProductSimilarity(0).ToString("E"));

            //return;
            //string str = "Quick brown fox jumped over a stupid dog.";
            //RegexTokenizer tok = new RegexTokenizer(str);
            //// tok.Filter = TokenizerFilter.AlphaLoose;
            //foreach (string token in tok)
            //{
            //    foreach (string token2 in tok)
            //    {
            //        Console.WriteLine("{0} x {1}", token, token2);
            //    }
            //}


            //IStemmer stemmer = new Lemmatizer(Language.Slovene);
            //Console.WriteLine(stemmer.GetStem("zavednega"));


            //return;



            //Console.WriteLine("There is no spoon.");
            //Cluster a = new Cluster();
            //a.Items.AddRange(new Pair<double, int>[] { new Pair<double, int>(1, 1) });
            //Cluster b = new Cluster();
            //Cluster c = new Cluster();
            //Cluster d = new Cluster();
            //a.Children.AddRange(new Cluster[] { b, c });
            //b.Children.Add(d);
            ////BinarySerializer mem_writer = new BinarySerializer();
            ////a.Save(mem_writer);
            ////mem_writer.Stream.Position = 0;
            ////Cluster e = new Cluster(mem_writer);
            //Cluster e = a.DeepClone();
            //Console.WriteLine(e.Children.Count);
            //Console.WriteLine(e.Children[0].Children.Count);
            //Console.WriteLine(e.Children[1].Children.Count);
            ////Console.WriteLine(e.ToString("TC"));

            //ClusteringResult r = new ClusteringResult();
            //r.Roots.Add(e);
            //r.Roots.Add(e.DeepClone());
            //r.Roots.Add(e.DeepClone());

            //Console.WriteLine(r.ToString());
            //Console.WriteLine(r.ToString("TC"));

            // compute bow space
            StreamReader reader = new StreamReader("C:\\newwork\\testclustering\\data\\yahoofinance.txt");
            //LineDocFileReader reader = new LineDocFileReader("C:\\newwork\\testclustering\\data\\yahoofinance.txt");
            string line;
            ArrayList<string> docs = new ArrayList<string>();
            while ((line = reader.ReadLine()) != null)
            {
                docs.Add(line);
            }
            reader.Close();
            BowSpace bow_space = new BowSpace();
            Set<string> stopwords = new Set<string>(StopWords.EnglishStopWords);
            stopwords.AddRange(StopWords.FrenchStopWords);
            stopwords.Add("media");
            // ...
            bow_space.StopWords = stopwords;
            bow_space.Stemmer = new PorterStemmer();
            bow_space.WordWeightType = WordWeightType.TfIdf;
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            bow_space.Tokenizer = tokenizer;
            bow_space.Initialize(docs);
            //reader.Close();
            StreamWriter writer = new StreamWriter("C:\\newwork\\testclustering\\data\\stats.txt");
            bow_space.OutputStats(writer);
            writer.Close();
            // perform clustering
            object dummy = new object();
            SparseVectorDataset<object> dataset = new SparseVectorDataset<object>();
            foreach (SparseVector bow_vec in bow_space.BowVectors)
            {
                dataset.Add(dummy, bow_vec);
            }

            DateTime start = DateTime.Now;
            SparseMatrix<double> sim_mtx = dataset.GetDotProductSimilarity(0, false);
            Console.WriteLine(DateTime.Now - start);

            //Console.WriteLine(sim_mtx[222, 4069]);
            //DotProductSimilarity dpsim = new DotProductSimilarity();
            //Console.WriteLine(dpsim.GetSimilarity(dataset[222].Example, dataset[4069].Example));


            //StressMajorization sm = new StressMajorization(dataset.Count, new DistFunc(sim_mtx));
            //VectorD[] pos_vec = sm.ComputeLayout();

            KMeansFast<object> k_means = new KMeansFast<object>(100);
            k_means.Eps = 0.01;
            //k_means.Similarity = new DotProductSimilarity();
            k_means.Random = new Random(1);
            k_means.Trials = 1;
            ClusteringResult clustering = k_means.Cluster(dataset);
            writer = new StreamWriter("C:\\newwork\\testclustering\\data\\clusters.txt");
            foreach (Cluster cluster in clustering.Roots)
            {
                writer.WriteLine(GetKeywordsStr(cluster, 5, dataset, bow_space));
                foreach (Pair<double, int> item in cluster.Items)
                {
                    writer.WriteLine(docs[item.Second]);
                }
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
            }
            writer.Close();


            // create dataset of centroids
            SparseVectorDataset<int> dscent = new SparseVectorDataset<int>();
            foreach (Cluster cluster in clustering.Roots)
            {
                dscent.Add(0, cluster.ComputeCentroid(dataset, CentroidType.NrmL2));
            }
            // compute layout
            sim_mtx = dscent.GetDotProductSimilarity(0, false);
            StressMajorization sm = new StressMajorization(dscent.Count, new DistFunc(sim_mtx));
            sm.MinDiff = 0.00001;
            sm.Random = new Random();
            Vector2D[] pos_vec = sm.ComputeLayout();




            // train a classifier
            IDataset<Cluster, SparseVector> classif_dataset = clustering.GetClusteringDataset(dataset);
            CentroidClassifier<Cluster> classifier = new CentroidClassifier<Cluster>();
            classifier.Similarity = new DotProductSimilarity();
            classifier.NormalizeCentroids = false;
            classifier.Train(classif_dataset);
            SparseVector new_vec = bow_space.ProcessDocument("We specialize in advanced Web 2.0-powered sites. In particular, we develop content management systems, video-hosting portals, and Wiki-like systems.");
            ClassifierResult<Cluster> result = classifier.Classify(new_vec);
            Console.WriteLine(GetKeywordsStr(result[0].Dat, 5, dataset, bow_space));

            KnnClassifier<Cluster, SparseVector> classifier_2 = new KnnClassifier<Cluster, SparseVector>(new DotProductSimilarity());
            classifier_2.K = 10;
            classifier_2.Train(classif_dataset);
            result = classifier_2.Classify(new_vec);
            //Console.WriteLine(result);
            Console.WriteLine(GetKeywordsStr(result[0].Dat, 5, dataset, bow_space));

            BinarySerializer mem_ser = new BinarySerializer();
            classifier_2.Save(mem_ser);
            mem_ser.Stream.Position = 0;
            KnnClassifier<Cluster, SparseVector> classifier_3 = new KnnClassifier<Cluster, SparseVector<double>.ReadOnly>(mem_ser);
            result = classifier_3.Classify(new_vec);
            Console.WriteLine(GetKeywordsStr(result[0].Dat, 5, dataset, bow_space));

        }
    }
}
