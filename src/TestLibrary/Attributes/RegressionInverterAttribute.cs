using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLibrary.Attributes
{
    public class RegressionInverterAttribute : TestActionAttribute
    {
        private class ProductVersion : IComparable<ProductVersion>
        {
            public static ProductVersion Trunk { get; } = new ProductVersion("trunk", new int[] { });

            public static ProductVersion Parse(string version)
            {
                if (version.ToLower() == "trunk")
                {
                    return Trunk;
                }

                return new ProductVersion(version);
            }

            private string version;
            private int[] parsed;

            private ProductVersion(string version, int[] parsed = null)
            {
                this.version = version;
                this.parsed = parsed == null ? Parse() : parsed;
            }

            private int[] Parse()
            {
                return (from part in version.Split('_')[0].Split('.') select ParseSection(part)).Reverse().SkipWhile((i) => i == 0).Reverse().ToArray();
            }
            
            private int ParseSection(string section)
            {
                try
                {
                    return int.Parse(section);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            public override string ToString()
            {
                return version;
            }

            public string ToNormalisedString()
            {
                if (parsed.Length == 0)
                {
                    return "trunk";
                }
                return (from part in parsed select part.ToString()).Aggregate((s1, s2) => s1 + "." + s2);
            }

            public int CompareTo(ProductVersion other)
            {
                int l1 = parsed.Length;
                int l2 = other.parsed.Length;


                if (l1 == 0 || l2 == 0)
                {
                    return l2 - l1;
                }

                for (int i = 0; i < ((l1 < l2) ? l1 : l2); i++)
                {
                    int d = parsed[i] - other.parsed[i];
                    if (d != 0)
                    {
                        return d;
                    }
                }

                return l1 - l2;
            }
        }

        public override ActionTargets Targets { get; } = ActionTargets.Test;

        public override void AfterTest(ITest test)
        {
            if (!test.RunState.HasFlag(RunState.Skipped)
                && !test.RunState.HasFlag(RunState.Ignored)
                && IsBug(test)
                && "true".Equals(TestContext.Parameters.Get("Regression", "false").ToLower())
                && IsVersionMatch(test))
            {
                // mark inconsistent if failed, but fail on pass
                switch (TestContext.CurrentContext.Result.Outcome.Status)
                {
                    case TestStatus.Failed:
                        Assume.That(false, "Triaged test case failure");
                        break;
                    case TestStatus.Passed:
                        Assert.That(false, "Test Cases Marked as Bug, but passed.");
                        break;
                    default:
                        break;
                }
            }
            base.AfterTest(test);
        }

        private bool IsVersionMatch(ITest test)
        {
            // get current version
            var versionStr = TestContext.Parameters.Get("ProductVersion", string.Empty);
            if (versionStr == string.Empty)
            {
                return true;
            }
            var version = ProductVersion.Parse(versionStr);

            var bugs = new List<string>();

            ITest pointer = test;
            do
            {
                if (pointer.Properties.ContainsKey(BugAttribute.PropertyName))
                {
                    foreach (var entry in pointer.Properties[BugAttribute.PropertyName])
                    {
                        bugs.Add(entry.ToString());
                    }
                }
            } while (null != (pointer = pointer.Parent));

            foreach (var bug in bugs)
            {
                var bugSpec = bug.Split(':');
                var from = bugSpec.Length > 1 ? bugSpec[1] : string.Empty;
                var until = bugSpec.Length > 2 ? bugSpec[2] : string.Empty;

                bool isFrom = from == string.Empty || ProductVersion.Parse(from).CompareTo(version) <= 0;
                bool isUntil = until == string.Empty || ProductVersion.Parse(until).CompareTo(version) > 0;

                if (isFrom && isUntil)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBug(ITest test)
        {
            ITest pointer = test;
            do
            {
                if (pointer.Properties.ContainsKey(BugAttribute.PropertyName))
                {
                    return true;
                }
            } while (null != (pointer = pointer.Parent));
            return false;
        }
    }
}
