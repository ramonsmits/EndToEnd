using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

public class ScanLogs
{
    static List<Dictionary<string, string>> GetData(string path)
    {
        var logs = Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories);

        var items = new List<Dictionary<string, string>>();

        foreach (var log in logs)
        {
            var d = new Dictionary<string, string>();

            var di = new DirectoryInfo(log);

            //d["Log"] = Uri.UnescapeDataString(new Uri(path).MakeRelativeUri(new Uri(log)).ToString());
            d["Category"] = di.Parent.Parent.Parent.Name;
            d["Fixture"] = di.Parent.Parent.Name;
            d["Test"] = di.Parent.Name;
            d["LogTimestamp"] = File.GetLastWriteTimeUtc(log).ToString("s", CultureInfo.InvariantCulture);
            d["LogSize"] = new FileInfo(log).Length.ToString(CultureInfo.InvariantCulture);

            foreach (var line in File.ReadLines(log))
            {
                var i = line.IndexOf("|Statistics|", StringComparison.Ordinal);
                if (i != -1)
                {
                    var sub = line.Substring(i);
                    var data = sub.Split(new[]
                    {
                        '|',
                        ':',
                        ' '
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (data.Length > 2)
                        d[data[1]] = data[2];
                }
                i = line.IndexOf("|Permutation|", StringComparison.Ordinal);
                if (i != -1)
                {
                    var sub = line.Substring(i);
                    var data = sub.Split(new[]
                    {
                        '|',
                        ':',
                        ' '
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (data.Length > 2)
                        d[data[1]] = data[2];
                }
            }

            items.Add(d);
        }

        return items;
    }



    public static string ToCsvString(string path)
    {
        var items = GetData(path);
        var keys = items.SelectMany(x => x.Keys).Distinct().ToArray();


        var sb = new StringBuilder();
        foreach (var k in keys)
        {
            var l = items
                .Select(x => x[k])
                .Max(x => x.Length);

            l = Math.Max(l, k.Length);
            var f = "{0," + l + "};";
            sb.AppendFormat(CultureInfo.InvariantCulture, f, k);
        }
        sb.AppendLine();

        foreach (var i in items)
        {
            foreach (var k in keys)
            {
                var l = items
                    .Select(x => x[k])
                    .Max(x => x.Length);

                l = Math.Max(l, k.Length);

                var v = i[k];
                var f = "{0," + l + "};";
                sb.AppendFormat(CultureInfo.InvariantCulture, f, v);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static string ToIniString(string path)
    {
        var items = GetData(path);

        var sb = new StringBuilder();

        var keyLength = items.SelectMany(x => x.Keys).Max(x => x.Length);
        var valueLength = items.SelectMany(x => x.Values).Max(x => x.Length);

        var format = "{0,-" + keyLength + "} = {1," + valueLength + "}";
        foreach (var item in items)
        {
            if (items.Count > 1)
                sb.AppendFormat("[{0}-{1}-{2}]", item["Category"], item["Fixture"], item["Test"]).AppendLine();

            foreach (var x in item)
            {
                sb.AppendFormat(format, x.Key, x.Value).AppendLine();
            }
        }
        return sb.ToString();
    }
}
