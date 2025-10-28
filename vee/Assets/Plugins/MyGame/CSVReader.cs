using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    public sealed class CSVReader
    {
        public List<Dictionary<string, object>> Read(string csv)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                var parsed = CSVReader.ParseCommaInCells(csv);
                var rows = CSVReader.GetJoinedRows(parsed);
                var keys = CSVReader.ReadLine(rows[0]);
                rows.RemoveAt(0);
                foreach (var row in rows)
                {
                    if (string.IsNullOrEmpty(row))
                    {
                        break;
                    }
                    var values = CSVReader.ReadLine(row);
                    values = CSVReader.UnparseCommaInCells(values);
                    values = CSVReader.UnparseDoubleQuoteInCells(values);
                    var dic = new Dictionary<string, object>();
                    for (var i = 0; i < keys.Length; i++)
                    {
                        var key = keys[i];
                        if (i >= values.Length)
                        {
                            UnityEngine.Debug.Log($"error. index of script key. :{key}");
                            continue;
                        }
                        dic[key] = values[i];
                    }
                    result.Add(dic);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
            return result;
        }

        private static string ParseCommaInCells(string csv)
        {
            var sb = new System.Text.StringBuilder();
            var index = 0;
            var addFlag = false;
            foreach (var c in csv)
            {
                addFlag = true;
                var ch = c;
                if (c == '"')
                {
                    addFlag = false;
                    index++;
                    sb.Append(ch);
                    continue;
                }
                if (c == ',')
                {
                    if (index % 2 != 0)
                    {
                        ch = '，';
                    }
                }
                if (addFlag)
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        private static string[] UnparseCommaInCells(string[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Replace('，', ',');
            }
            return array;
        }

        private static string[] UnparseDoubleQuoteInCells(string[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Replace("\"", "");
            }
            return array;
        }

        // TODO: ダブルクォートでくくる以外にも対応させる
        private static List<string> GetJoinedRows(string csv)
        {
            var rows = csv.Replace("\r", "").Split('\n');
            var joinedRows = new List<string>();
            var builder = new System.Text.StringBuilder();
            var joinable = false;
            foreach (var row in rows)
            {
                builder.Append(row);
                var values = CSVReader.ReadLine(row);
                foreach (var value in values)
                {
                    if (value.StartsWith("\""))
                    {
                        joinable = true;
                    }
                    if (value.EndsWith("\""))
                    {
                        joinable = false;
                    }
                }
                if (joinable)
                {
                    builder.Append("\n");
                    continue;
                }
                joinedRows.Add(builder.ToString());
                builder.Length = 0;
            }
            return joinedRows;
        }

        private static string[] ReadLine(string str)
        {
            return str.Split(',');
        }
    }
}
