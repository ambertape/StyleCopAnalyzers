﻿namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1603DocumentationMustContainValidXml"/>-
    /// </summary>
    [TestClass]
    public class SA1603UnitTests : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        public string DiagnosticId { get; } = SA1603DocumentationMustContainValidXml.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestTextDocumentation()
        {
            var testCode = @"
/// Foo
public class Foo { }";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyElementDocumentation()
        {
            var testCode = @"
/// <summary/>
public class Foo { }";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestElementDocumentation()
        {
            var testCode = @"
/// <summary></summary>
public class Foo { }";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCDataDocumentation()
        {
            var testCode = @"
/// <![CDATA[Foo]]>
public class Foo { }";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestElementStartTagSkippedToken()
        {
            var testCode = @"
/// <summary=></summary>
public class Foo { }";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The documentation header is composed of invalid Xml: Invalid token.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 13)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestElementEndTagSkippedToken()
        {
            var testCode = @"
/// <summary></summary=>
public class Foo { }";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The documentation header is composed of invalid Xml: Invalid token.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 23)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyElementSkippedToken()
        {
            var testCode = @"
/// <summary=/>
public class Foo { }";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The documentation header is composed of invalid Xml: Invalid token.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 13)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestElementTagsNotMatching()
        {
            var testCode = @"
/// <summary>a</sumary>
public class Foo { }";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The documentation header is composed of invalid Xml: The 'summary' start tag does not match the end tag of 'sumary'.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 5),
                                new DiagnosticResultLocation("Test0.cs", 2, 15)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1603DocumentationMustContainValidXml();
        }
    }
}
