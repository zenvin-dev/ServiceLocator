using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;

namespace Zenvin.Services.SourceGenerator
{
	internal static class DiagnosticConstants
	{
		public static readonly DiagnosticDescriptor WarningWholeType = Create (Warning, 1000, "Type must be partial for injection code to be generated.");
		public static readonly DiagnosticDescriptor WarningAbstractMember = Create (Warning, 1001, "Cannot inject into an abstract member.");
		public static readonly DiagnosticDescriptor WarningStaticMember = Create (Warning, 1002, "Cannot inject into a static member.");
		public static readonly DiagnosticDescriptor WarningExternMember = Create (Warning, 1003, "Cannot inject into a extern member.");


		private static DiagnosticDescriptor Create (DiagnosticSeverity severity, int code, string title, string message = null)
		{
			return new DiagnosticDescriptor (
				$"ZSL {code}",
				title,
				message,
				"unity",
				severity,
				true
			);
		}
	}
}
