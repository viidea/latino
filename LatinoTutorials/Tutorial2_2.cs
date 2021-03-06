﻿/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Tutorial2_2.cs
 *  Desc:    Tutorial 2.2: SparseMatrix
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using Latino;

namespace LatinoTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create SparseMatrix
            Console.WriteLine("Create SparseMatrix ...");
            SparseMatrix<string> matrix = new SparseMatrix<string>();
            matrix[0] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "a"),
                new IdxDat<string>(3, "b"),
                new IdxDat<string>(4, "c") });
            matrix[2] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(2, "d"),
                new IdxDat<string>(4, "e"),
                new IdxDat<string>(5, "f") });
            matrix[3] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "g"),
                new IdxDat<string>(3, "h"),
                new IdxDat<string>(5, "i") });
            matrix[4] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "j"),
                new IdxDat<string>(2, "k"),
                new IdxDat<string>(4, "l") });
            Console.WriteLine(matrix.ToString("E"));
            // get rows
            Console.WriteLine("Get rows ...");
            Console.WriteLine(matrix[0]);
            Console.WriteLine(matrix[3]);
            // set rows
            Console.WriteLine("Set rows ...");
            matrix[1] = new SparseVector<string>(new IdxDat<string>[] { new IdxDat<string>(0, "j"), new IdxDat<string>(3, "k") });
            matrix[2] = null;
            matrix[4] = null;
            Console.WriteLine(matrix.ToString("E"));
            // count rows
            Console.WriteLine("Count rows ...");
            Console.WriteLine("{0} != {1}", matrix.GetRowCount(), matrix.GetLastNonEmptyRowIdx() + 1);
            // trim rows
            Console.WriteLine("Trim rows ...");
            matrix.TrimRows(); 
            Console.WriteLine(matrix.ToString("E"));
            // add more items
            Console.WriteLine("Add more items ...");
            matrix[0].Add("*");
            matrix[3].AddRange(new IdxDat<string>[] {
                new IdxDat<string>(1, "!"),
                new IdxDat<string>(2, "?"),
                new IdxDat<string>(4, "&") });
            matrix[2] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(2, "d"),
                new IdxDat<string>(4, "e"),
                new IdxDat<string>(5, "f") });
            Console.WriteLine(matrix.ToString("E"));
            // get items
            Console.WriteLine("Get items ...");
            Console.WriteLine(matrix[0, 1]);
            Console.WriteLine(matrix[2, 2]);
            Console.WriteLine(matrix[2][4]);
            Console.WriteLine(matrix.TryGet(2, 4, "missing"));
            Console.WriteLine(matrix.TryGet(2, 6, "missing"));
            // set items
            Console.WriteLine("Set items ...");
            matrix[0, 1] = "l";
            matrix[2, 3] = "m";
            matrix[3][4] = "n";
            Console.WriteLine(matrix.ToString("E"));
            // check for items
            Console.WriteLine("Check for items ...");
            Console.WriteLine(matrix.ContainsAt(0, 1));
            Console.WriteLine(matrix.ContainsAt(1, 1));
            Console.WriteLine(matrix.Contains("c"));
            Console.WriteLine(matrix.Contains("C"));
            int rowIdx = -1, colIdx = -1;
            matrix.IndexOf("c", ref rowIdx, ref colIdx);
            Console.WriteLine("{0}, {1}", rowIdx, colIdx);
            // check for rows and columns
            Console.WriteLine("Check for rows and columns ...");
            Console.WriteLine(matrix.ContainsColAt(0));
            Console.WriteLine(matrix.ContainsColAt(100));
            Console.WriteLine(matrix.ContainsRowAt(0));
            Console.WriteLine(matrix.ContainsRowAt(100));
            // get first and last non-empty row and column index
            Console.WriteLine("Get first and last non-empty row and column index ...");
            Console.WriteLine(matrix.GetFirstNonEmptyRowIdx());
            Console.WriteLine(matrix.GetLastNonEmptyRowIdx());
            Console.WriteLine(matrix.GetFirstNonEmptyColIdx());
            Console.WriteLine(matrix.GetLastNonEmptyColIdx());       
            // get first and last item in row
            Console.WriteLine("Get first and last item in row ...");
            Console.WriteLine(matrix[0].First);
            Console.WriteLine(matrix[3].Last);
            // create another SparseMatrix
            Console.WriteLine("Create another SparseMatrix ...");
            SparseMatrix<string> matrix2 = new SparseMatrix<string>();
            matrix2[0] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "A"),
                new IdxDat<string>(2, "B"),
                new IdxDat<string>(3, "C") });
            matrix2[2] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "D"),
                new IdxDat<string>(3, "E") });
            matrix2[3] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "G"),
                new IdxDat<string>(1, "H"),
                new IdxDat<string>(2, "I") });
            Console.WriteLine(matrix2.ToString("E"));
            // concatenate
            Console.WriteLine("Concatenate ...");
            matrix.AppendCols(matrix2, matrix.GetLastNonEmptyColIdx() + 1);
            Console.WriteLine(matrix.ToString("E"));
            // remove items
            Console.WriteLine("Remove items ...");
            matrix.RemoveAt(0, 1);
            matrix.RemoveAt(3, 5);
            Console.WriteLine(matrix.ToString("E"));
            // directly access to items
            Console.WriteLine("Directly access to items ...");
            int idx = matrix[0].GetDirectIdx(4);
            Console.WriteLine(idx);
            Console.WriteLine(matrix[0].GetDirect(idx));
            matrix[0].SetDirect(idx, "C");
            Console.WriteLine(matrix[1].GetDirect(0));
            matrix[1].RemoveDirect(0);
            Console.WriteLine(matrix.ToString("E"));
            // get properties
            Console.WriteLine("Get properties ...");
            Console.WriteLine("{0:0.00}%", matrix.GetSparseness(matrix.GetLastNonEmptyRowIdx() + 1, matrix.GetLastNonEmptyColIdx() + 1) * 100.0);
            Console.WriteLine(matrix.IsSymmetric());
            Console.WriteLine(matrix.ContainsDiagonalElement());
            Console.WriteLine(matrix.CountValues());
            // perform unary operation
            Console.WriteLine("Perform unary operation ...");
            matrix.PerformUnaryOperation(delegate(string item) { return item.ToUpper(); });
            Console.WriteLine(matrix.ToString("E"));
            // merge
            Console.WriteLine("Merge ...");
            matrix.Merge(matrix2, delegate(string a, string b) { return string.Format("{0}+{1}", a, b); });
            Console.WriteLine(matrix.ToString("E"));
            // clear row and column
            Console.WriteLine("Clear row and column ...");
            matrix.RemoveRowAt(2);
            matrix.RemoveColAt(1);
            Console.WriteLine(matrix.ToString("E"));
            // purge row and column
            Console.WriteLine("Purge row and column ...");
            matrix.PurgeRowAt(2);
            matrix.PurgeColAt(1);
            Console.WriteLine(matrix.ToString("E"));
            // get column copy
            Console.WriteLine("Get column copy ...");
            Console.WriteLine(matrix.GetColCopy(0));
            // transpose
            Console.WriteLine("Transpose ...");
            Console.WriteLine(matrix.GetTransposedCopy().ToString("E"));            
            // set diagonal 
            Console.WriteLine("Set diagonal ...");
            matrix.SetDiagonal(matrix.GetLastNonEmptyColIdx() + 1, "X");
            Console.WriteLine(matrix.ToString("E"));
            // make symmetric
            Console.WriteLine("Make symmetric ...");
            matrix.Symmetrize(delegate(string a, string b) { return string.Format("{0}+{1}", a, b); });
            Console.WriteLine(matrix.ToString("E"));
        }
    }
}
