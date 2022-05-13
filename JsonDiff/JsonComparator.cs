using System.Text.RegularExpressions;
using JsonDiff.Models;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;

namespace JsonDiff;

public class JsonComparator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="originalJson"></param>
    /// <param name="newJson"></param>
    /// <param name="pathsToIgnore">Json paths to ingore as regex, ex. "/root/name" or "/root/configuration/1"</param>
    /// <param name="ignoreNulls">If true and in src json a property was null and in dest there is no such property then it is considered as no diff</param>
    /// <returns></returns>
    public HumanReadableDiffResult GetDiff(string originalJson, string newJson, IList<string> pathsToIgnore, bool ignoreNulls = true)
    {
        var jdp = new JsonDiffPatch();
        var orig = JToken.Parse(originalJson);
        var edited = JToken.Parse(newJson);

        SortArrays(orig);
        SortArrays(edited);

        JToken patch = jdp.Diff(orig, edited);

        if (patch == null) return new HumanReadableDiffResult();

        var ops = new MyJsonDeltaFormatter().Format(patch);

        //Do some filtering here if you need

        //apply ignoreNulls
        if (ignoreNulls)
        {
            ops = ops.Where(op => !(op.Op == "remove" && string.IsNullOrEmpty(op.Value.ToString()))).ToArray();
        }

        var opsToExclude = new List<MyOperation>();
        if (pathsToIgnore != null && pathsToIgnore.Any())
        {
            foreach (var p in pathsToIgnore)
            {
                foreach (var op in ops)
                {
                    if (Regex.Match(op.Path, p).Success)
                        opsToExclude.Add(op);
                    // try
                    // {
                    // }
                    // catch (ArgumentException)
                    // {
                    // }                    
                }
            }
        }

        return ConvertToHumanReadable(ops.Except(opsToExclude));
    }

    private HumanReadableDiffResult ConvertToHumanReadable(IEnumerable<MyOperation> ops)
    {
        var added = new List<Added>();
        var removed = new List<Removed>();
        var changed = new List<Changed>();
        foreach (var op in ops)
        {
            switch (op.Op)
            {
                case "add":
                    added.Add(new Added(op.Path, op.Value.ToString()));
                    break;
                case "remove":
                    removed.Add(new Removed(op.Path, op.Value.ToString()));
                    break;
                case "replace":
                    changed.Add(new Changed(op.Path, op.OldValue.ToString(), op.Value.ToString()));
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        return new HumanReadableDiffResult(added, changed, removed);
    }


    private JToken GetSortedArray(JArray arr)
    {
        if (!arr.HasValues || (arr.HasValues && arr.Count == 1)) return arr;

        //array of values
        if (arr.First is JValue)
            return new JArray(arr.OrderBy(v => v));


        //array of objects. Sort by props
        var fObj = (JObject)arr.First;
        IQueryable<JToken> query = arr.AsQueryable();
        using (var enumerator = fObj.GetEnumerator())
        {
            var i = 0;
            while (enumerator.MoveNext())
            {
                var prop = enumerator.Current;
                if (prop.Value is JArray array)
                {
                    fObj[prop.Key] = GetSortedArray(array);
                    continue;
                }

                query = i == 0
                    ? query.OrderBy(o => o[prop.Key])
                    : ((IOrderedQueryable<JToken>)query).ThenBy(o => o[prop.Key]);

                i++;
            }
        }

        return new JArray(query);
    }

    private void SortArrays(JToken json)
    {
        var prop = (JProperty)json.First;

        while (prop != null)
        {
            if (!prop.HasValues)
            {
                prop = prop.Next as JProperty;
                continue;
            }

            //depth first search
            if (prop.Value is JObject value)
            {
                prop = (JProperty)value.First;
                continue;
            }

            if (prop.Value is JArray array)
            {
                prop.Value = GetSortedArray(array);
            }

            prop = prop.Next as JProperty;
        }
    }
}