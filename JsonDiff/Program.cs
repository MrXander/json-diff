// See https://aka.ms/new-console-template for more information

using JsonDiff;
using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json.Linq;

var exclude = new List<string>();
var patch = new JsonComparator().GetDiff("{ \"name\": \"John\" }", "{ \"name\": \"John\", \"surname\": \"Surname_John\" }", exclude);