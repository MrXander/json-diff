using JsonDiffPatchDotNet.Formatters;
using JsonDiffPatchDotNet.Formatters.JsonPatch;

namespace JsonDiff
{
	public class MyJsonFormatContext : IFormatContext<IList<MyOperation>>
	{
		public MyJsonFormatContext()
		{
			Operations = new List<MyOperation>();
			Path = new List<string>();
		}

		public IList<MyOperation> Operations { get; }

		public IList<string> Path { get; }

		public IList<MyOperation> Result()
		{
			return Operations;
		}

		public void PushCurrentOp(string op, object value)
		{
			Operations.Add(new MyOperation(op, CurrentPath(), null, null, value));
		}

		public void PushCurrentOp(string op, object oldValue, object newValue)
		{
			Operations.Add(new MyOperation(op, CurrentPath(), null, oldValue, newValue));
		}

		public void PushMoveOp(string to)
		{
			Operations.Add(new MyOperation(OperationTypes.Move, ToPath(to), CurrentPath()));
		}

		private string CurrentPath()
		{
			return $"/{string.Join("/", Path)}";
		}

		private string ToPath(string toPath)
		{
			var to = Path.ToList();
			to[to.Count - 1] = toPath;
			return $"/{string.Join("/", to)}";
		}
	}
}
