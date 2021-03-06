﻿using System.IO;
using Latino;
using Latino.TextMining;
using Latino.Visualization;
using System;

namespace LatinoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // load documents
            Utils.VerboseLine("Loading documents ...");
            string[] docs = File.ReadAllLines("C:\\newwork\\testclustering\\data\\yahoofinance.txt");
            BowSpace bowSpace = new BowSpace();
            bowSpace.StopWords = StopWords.EnglishStopWords;
            bowSpace.Stemmer = new PorterStemmer();
            bowSpace.WordWeightType = WordWeightType.TfIdf;
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            bowSpace.Tokenizer = tokenizer;
            bowSpace.Initialize(docs);
            // compute layout
            SemanticSpaceLayout semSpc = new SemanticSpaceLayout(bowSpace);
            Vector2D[] coords = semSpc.ComputeLayout();
            // build spatial index
            //Utils.VerboseLine("Building spatial index ...");
            //SpatialIndex2D spatIdx = new SpatialIndex2D();
            //spatIdx.BuildIndex(coords);
            //spatIdx.InsertPoint(9000, new Vector2D(1000, 1000));
            //ArrayList<IdxDat<Vector2D>> points = spatIdx.GetPoints(new Vector2D(0.5, 0.5), 0.1);
            //Utils.VerboseLine("Number of retrieved points: {0}.", points.Count);

            ArrayList<Vector2D> tmp = new ArrayList<Vector2D>(coords);
            tmp.Shuffle();
            //tmp.RemoveRange(1000, tmp.Count - 1000);

            // compute elevation
            StreamWriter writer = new StreamWriter("c:\\elev.txt");
            LayoutSettings ls = new LayoutSettings(800, 600);
            ls.AdjustmentType = LayoutAdjustmentType.Soft;
            ls.StdDevMult = 2;
            ls.FitToBounds = true;
            ls.MarginVert = 50;
            ls.MarginHoriz = 50;
            double[,] zMtx = VisualizationUtils.ComputeLayoutElevation(tmp, ls, 150, 200);
            VisualizationUtils.__DrawElevation__(tmp, ls, 300, 400).Save("c:\\elev.bmp");
            for (int row = 0; row < zMtx.GetLength(0); row++)
            {
                for (int col = 0; col < zMtx.GetLength(1); col++)
                {
                    writer.Write("{0}\t", zMtx[row, col]);
                }
                writer.WriteLine();
            }
            writer.Close();
            
            // output coordinates
            StreamWriter tsvWriter = new StreamWriter("c:\\layout.tsv");
            for (int i = 0; i < coords.Length; i++)
            {
                //if (i < points.Count)
                //{
                //    tsvWriter.WriteLine("{0}\t{1}\t{2}\t{3}", coords[i].X, coords[i].Y, points[i].Dat.X, points[i].Dat.Y);
                //}
                //else
                {
                    tsvWriter.WriteLine("{0}\t{1}", coords[i].X, coords[i].Y);
                }
            }
            tsvWriter.Close();
            //// get document names
            //int k = 0;
            //ArrayList<Pair<string, Vector2D>> layout = new ArrayList<Pair<string, Vector2D>>();
            //foreach (string doc in docs)
            //{
            //    string[] docInfo = doc.Split(' ');
            //    layout.Add(new Pair<string, Vector2D>(docInfo[0], coords[k++]));
            //}
            //Console.WriteLine(coords.Length);
            //Console.WriteLine(layout.Count);
            //StreamWriter writer = new StreamWriter("c:\\vidCoords.txt");
            //foreach (Pair<string, Vector2D> docPos in layout)
            //{
            //    writer.WriteLine("{0}\t{1}\t{2}", docPos.First, docPos.Second.X, docPos.Second.Y);
            //}
            //writer.Close();
        }
    }
}
