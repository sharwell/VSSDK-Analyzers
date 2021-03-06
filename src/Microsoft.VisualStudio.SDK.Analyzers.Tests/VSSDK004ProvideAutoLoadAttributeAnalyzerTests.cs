﻿// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.SDK.Analyzers;
using Microsoft.VisualStudio.SDK.Analyzers.Tests;
using Xunit;
using Xunit.Abstractions;

public class VSSDK004ProvideAutoLoadAttributeAnalyzerTests : DiagnosticVerifier
{
    private DiagnosticResult expect = new DiagnosticResult
    {
        Id = VSSDK004ProvideAutoLoadAttributeAnalyzer.Id,
        SkipVerifyMessage = true,
        Severity = DiagnosticSeverity.Warning,
    };

    public VSSDK004ProvideAutoLoadAttributeAnalyzerTests(ITestOutputHelper logger)
        : base(logger)
    {
    }

    [Fact]
    public void NoPackageBaseClassProvidesNoDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"")]
class Test {
}
";
        this.VerifyCSharpDiagnostic(test);
    }

    [Fact]
    public void NoProvideAutoLoadProducesNoDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

class Test : AsyncPackage {
}
";
        this.VerifyCSharpDiagnostic(test);
    }

    [Fact]
    public void BasicProvideAutoLoadProducesDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"")]
class Test : AsyncPackage {
}
";
        this.expect.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 2, 4, 59) };
        this.VerifyCSharpDiagnostic(test, this.expect);
    }

    [Fact]
    public void ProvideAutoLoadWithFlagsButNoBackgroundLoadProducesDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"", PackageAutoLoadFlags.None)]
class Test : AsyncPackage {
}
";
        this.expect.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 2, 4, 86) };
        this.VerifyCSharpDiagnostic(test, this.expect);
    }

    [Fact]
    public void MultipleProvideAutoLoadProducesMultipleDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{A184B08F-C81C-45F6-A57F-5ABD9991F28F}"", PackageAutoLoadFlags.None)]
[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"", PackageAutoLoadFlags.None)]
class Test : AsyncPackage {
}
";
        DiagnosticResult[] expected = new[]
        {
            new DiagnosticResult()
            {
                Id = VSSDK004ProvideAutoLoadAttributeAnalyzer.Id,
                SkipVerifyMessage = true,
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 2, 4, 86) },
            },
            new DiagnosticResult()
            {
                Id = VSSDK004ProvideAutoLoadAttributeAnalyzer.Id,
                SkipVerifyMessage = true,
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 2, 5, 86) },
            },
        };

        this.VerifyCSharpDiagnostic(test, expected);
    }

    [Fact]
    public void ProvideAutoLoadWithNamedFlagsButNoBackgroundLoadProducesDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"", flags: PackageAutoLoadFlags.None)]
class Test : AsyncPackage {
}
";
        this.expect.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 2, 4, 93) };
        this.VerifyCSharpDiagnostic(test, this.expect);
    }

    [Fact]
    public void ProvideAutoLoadWithSkipFlagProducesNoDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"", PackageAutoLoadFlags.SkipWhenUIContextRulesActive)]
class Test : Package {
}
";
        this.VerifyCSharpDiagnostic(test);
    }

    [Fact]
    public void ProvideAutoLoadWithBackgroundLoadFlagProducesNoDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"", PackageAutoLoadFlags.BackgroundLoad)]
class Test : AsyncPackage {
}
";
        this.VerifyCSharpDiagnostic(test);
    }

    [Fact]
    public void ProvideAutoLoadOnPackageWithBackgroundLoadFlagProducesDiagnostics()
    {
        var test = @"
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad(""{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"", PackageAutoLoadFlags.BackgroundLoad)]
class Test : Package {
}
";
        this.expect.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 2, 4, 96) };
        this.VerifyCSharpDiagnostic(test, this.expect);
    }

    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new VSSDK004ProvideAutoLoadAttributeAnalyzer();
}
