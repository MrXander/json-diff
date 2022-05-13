using JsonDiffPatchDotNet.Formatters;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json.Linq;

namespace JsonDiff
{
    public class MyJsonDeltaFormatter : BaseDeltaFormatter<MyJsonFormatContext, IList<MyOperation>>
    {
        protected override bool IncludeMoveDestinations => true;

        public override IList<MyOperation> Format(JToken delta)
        {
            var result = base.Format(delta);
            return ReorderOps(result);
        }

        protected override void Format(
            DeltaType type,
            MyJsonFormatContext context,
            JToken delta,
            JToken leftValue,
            string key,
            string leftKey,
            MoveDestination movedFrom)
        {
            switch (type)
            {
                case DeltaType.Added:
                    FormatAdded(context, delta);
                    break;

                case DeltaType.Node:
                    FormatNode(context, delta, leftValue);
                    break;

                case DeltaType.Modified:
                    FormatModified(context, delta);
                    break;

                case DeltaType.Deleted:
                    FormatDeleted(context, delta);
                    break;

                case DeltaType.Moved:
                    FormatMoved(context, delta);
                    break;

                case DeltaType.Unknown:
                case DeltaType.Unchanged:
                case DeltaType.MoveDestination:
                    break;

                case DeltaType.TextDiff:
                    throw new InvalidOperationException("JSON RFC 6902 does not support TextDiff.");
            }
        }

        protected override void NodeBegin(
            MyJsonFormatContext context,
            string key,
            string leftKey,
            DeltaType type,
            NodeType nodeType,
            bool isLast)
        {
            context.Path.Add(leftKey);
        }

        protected override void NodeEnd(
            MyJsonFormatContext context,
            string key,
            string leftKey,
            DeltaType type,
            NodeType nodeType,
            bool isLast)
        {
            if (context.Path.Count > 0)
                context.Path.RemoveAt(context.Path.Count - 1);
        }

        protected override void RootBegin(MyJsonFormatContext context, DeltaType type, NodeType nodeType)
        {
        }

        protected override void RootEnd(MyJsonFormatContext context, DeltaType type, NodeType nodeType)
        {
        }

        private void FormatNode(MyJsonFormatContext context, JToken delta, JToken left)
        {
            FormatDeltaChildren(context, delta, left);
        }

        private void FormatAdded(MyJsonFormatContext context, JToken delta)
        {
            context.PushCurrentOp(OperationTypes.Add, delta[0]);
        }

        private void FormatModified(MyJsonFormatContext context, JToken delta)
        {
            context.PushCurrentOp(OperationTypes.Replace, delta[0], delta[1]);
        }

        private void FormatDeleted(MyJsonFormatContext context, JToken delta)
        {
            context.PushCurrentOp(OperationTypes.Remove, delta[0]);
        }

        private void FormatMoved(MyJsonFormatContext context, JToken delta)
        {
            context.PushMoveOp(delta[1].ToString());
        }

        private IList<MyOperation> ReorderOps(IList<MyOperation> result)
        {
            var removeOpsOtherOps = PartitionRemoveOps(result);
            var removeOps = removeOpsOtherOps[0];
            var otherOps = removeOpsOtherOps[1];
            Array.Sort(removeOps, new RemoveOperationComparer());
            return removeOps.Concat(otherOps).ToList();
        }

        private IList<MyOperation[]> PartitionRemoveOps(IList<MyOperation> result)
        {
            var left = new List<MyOperation>();
            var right = new List<MyOperation>();

            foreach (var op in result)
            {
                (op.Op.Equals("remove", StringComparison.Ordinal) ? left : right).Add(op);
            }

            return new List<MyOperation[]> { left.ToArray(), right.ToArray() };
        }

        private class RemoveOperationComparer : IComparer<MyOperation>
        {
            public int Compare(MyOperation a, MyOperation b)
            {
                if (a == null) throw new ArgumentNullException(nameof(a));
                if (b == null) throw new ArgumentNullException(nameof(b));

                var splitA = a.Path.Split('/');
                var splitB = b.Path.Split('/');

                return splitA.Length != splitB.Length
                    ? splitA.Length - splitB.Length
                    : CompareByIndexDesc(splitA.Last(), splitB.Last());
            }

            private static int CompareByIndexDesc(string indexA, string indexB) =>
                int.TryParse(indexA, out var a) && int.TryParse(indexB, out var b) ? b - a : 0;
        }
    }
}