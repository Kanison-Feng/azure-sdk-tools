// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier = Azure.ClientSdk.Analyzers.Tests.AzureAnalyzerVerifier<Azure.ClientSdk.Analyzers.ClientAssemblyNamespaceAnalyzer>;

namespace Azure.ClientSdk.Analyzers.Tests
{
    public class AZC0001Tests
    {
        [Fact]
        public async Task AZC0001ProducedForInvalidNamespaces()
        {
            const string code = @"
namespace RandomNamespace
{
    public class Program { }
}";

            var diagnostic = Verifier.Diagnostic("AZC0001")
                .WithMessage("Namespace 'RandomNamespace' shouldn't contain public types. Use one of the following pre-approved namespace groups: " +
                             "Azure.AI, Azure.Analytics, Azure.Data, Azure.Iot, Azure.Media, Azure.Messaging, Azure.Security, Azure.Storage, Microsoft.Extensions.Azure")
                .WithSpan(2, 11, 2, 26);

            await Verifier.VerifyAnalyzerAsync(code, diagnostic);
        }

        [Fact]
        public async Task AZC0001ProducedOneErrorPerNamespaceDefinition()
        {
            const string code = @"
namespace [|RandomNamespace|]
{
    public class Program { }
}

namespace [|RandomNamespace|]
{
    public class Program2 { }
}";
            await Verifier.VerifyAnalyzerAsync(code, "AZC0001");
        }

        [Fact]
        public async Task AZC0001NotProducedForNamespacesWithPrivateMembersOnly()
        {
            const string code = @"
namespace RandomNamespace
{
    internal class Program { }
}";
            await Verifier.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task AZC0001NotProducedForAllowedNamespaces()
        {
            const string code = @"
namespace Azure.Storage.Hello
{
    public class Program { }
}";
            await Verifier.VerifyAnalyzerAsync(code);
        }
    }
}