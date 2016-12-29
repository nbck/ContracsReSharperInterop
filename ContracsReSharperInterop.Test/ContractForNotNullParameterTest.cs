﻿namespace ContracsReSharperInterop.Test
{
    using Xunit;

    public class ContractForNotNullParameterTest : ContractForNotNullCodeFixVerifier
    {
        [Fact]
        public void NoDiagnosticIsGeneratedIfAllNotNullParametersHaveContracts()
        {
            const string originalCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        void Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);
        }
    }
}";

            VerifyCSharpDiagnostic(originalCode);
        }

        [Fact]
        public void DiagnosticIsGeneratedIfNotAllNotNullParametersHaveContracts()
        {
            const string originalCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        void Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);

            arg = arg2;
        }
    }
}";

            var expected = new DiagnosticResult(9, 60, "arg2");

            VerifyCSharpDiagnostic(originalCode, expected);

            const string fixedCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        void Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);

            arg = arg2;
        }
    }
}";

            VerifyCSharpFix(originalCode, fixedCode, null, true);
        }

        [Fact]
        public void DiagnosticIsGeneratedIfAllNotNullParametersHaveNoContracts()
        {
            const string originalCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        void Method([NotNull] object arg, [NotNull] object arg2)
        {
            arg = arg2;
        }
    }
}";

            var expected1 = new DiagnosticResult(9, 38, "arg");
            var expected2 = new DiagnosticResult(9, 60, "arg2");

            VerifyCSharpDiagnostic(originalCode, expected1, expected2);

            const string fixedCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        void Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);
            arg = arg2;
        }
    }
}";

            VerifyCSharpFix(originalCode, fixedCode, null, true);
        }

        [Fact]
        public void DiagnosticIsGeneratedIfResultHasNoContract()
        {
            const string originalCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        [NotNull]
        object Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);

            arg = arg2;
            return null;
        }
    }
}";

            var expected = new DiagnosticResult(10, 16, "Method");

            VerifyCSharpDiagnostic(originalCode, expected);

            const string fixedCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        [NotNull]
        object Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);
            Contract.Ensures(Contract.Result<object>() != null);

            arg = arg2;
            return null;
        }
    }
}";

            VerifyCSharpFix(originalCode, fixedCode, null, true);
        }

        [Fact]
        public void DiagnosticIsGeneratedOnContractClassIfResultHasNoContract()
        {
            const string originalCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    [ContractClass(typeof(InterfaceContract))]
    interface Interface
    {
        [NotNull]
        object Method();
    }

    [ContractClassFor(typeof(Interface))]
    abstract class InterfaceContract : Interface
    {
        public object Method()
        {
            return null;
        }
    }
}";

            var expected = new DiagnosticResult(11, 16, "Method");

            VerifyCSharpDiagnostic(originalCode, expected);

            const string fixedCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    [ContractClass(typeof(InterfaceContract))]
    interface Interface
    {
        [NotNull]
        object Method();
    }

    [ContractClassFor(typeof(Interface))]
    abstract class InterfaceContract : Interface
    {
        public object Method()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            return null;
        }
    }
}";

            VerifyCSharpFix(originalCode, fixedCode, null, true);
        }

        [Fact]
        public void NoDiagnosticIsGeneratedOnContractClassIfResultHasAlreadyContract()
        {
            const string originalCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    [ContractClass(typeof(InterfaceContract))]
    interface Interface
    {
        [NotNull]
        object Method();
    }

    [ContractClassFor(typeof(Interface))]
    abstract class InterfaceContract : Interface
    {
        public object Method()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            return null;
        }
    }
}";

            VerifyCSharpDiagnostic(originalCode);
        }

        [Fact]
        public void DiagnosticIsGeneratedIfGenericResultHasNoContract()
        {
            const string originalCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        [NotNull]
        public System.Collections.Generic.IEnumerable<object> Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);

            arg = arg2;
            return null;
        }
    }
}";

            var expected = new DiagnosticResult(10, 63, "Method");

            VerifyCSharpDiagnostic(originalCode, expected);

            const string fixedCode = @"
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Test
{
    class Class
    {
        [NotNull]
        public System.Collections.Generic.IEnumerable<object> Method([NotNull] object arg, [NotNull] object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);
            Contract.Ensures(Contract.Result<System.Collections.Generic.IEnumerable<object>>() != null);

            arg = arg2;
            return null;
        }
    }
}";

            VerifyCSharpFix(originalCode, fixedCode, null, true);
        }
    }
}

namespace Test1
{
    using System.Diagnostics.Contracts;

    using JetBrains.Annotations;

    [ContractClass(typeof(InterfaceContract))]
    interface Interface
    {
        [NotNull]
        object Method([NotNull] object arg, [NotNull] object arg2);
    }

    [ContractClassFor(typeof(Interface))]
    abstract class InterfaceContract : Interface
    {
        public object Method(object arg, object arg2)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg2 != null);

            return null;
        }
    }
}
