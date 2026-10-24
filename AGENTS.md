# Format documentation

Before changing a reader, directory, descriptor, or format-specific test, read
the relevant `docs/formats/<FORMAT>.md` and its same-named counterpart in the
Java implementation. Check related container and metadata format documents too.
Start with the [format documentation index](#format-documentation-index).
Read [.github/copilot-instructions.md](.github/copilot-instructions.md) for
.NET build, API, style, sibling-checkout, and corpus guidance.

Keep format-specific behavior, limits, references, and implementation details in
`docs/formats/<FORMAT>.md`, not the root README. The README is a quick project
overview. Use the same path and uppercase format filename in Java and .NET,
independent of their source package layouts.
Keep verified format facts and shared behavior identical in both documents.
Clearly separate Java-only and .NET-only behavior, intentional API differences,
confirmed capability gaps, and questions not yet verified. Do not copy a claim
of support from one implementation to the other without checking code and tests.
Update both documents when verified behavior changes; aim to eliminate genuine
capability gaps rather than conceal them in documentation.

## Format documentation index

| Format | Documentation |
| --- | --- |
| Adobe Digital Negative | [DNG.md](docs/formats/DNG.md) |
| EBML detection | [EBML.md](docs/formats/EBML.md) |
| HEIF / HEIC / AVIF references | [HEIF.md](docs/formats/HEIF.md) |
| ICC profiles | [ICC.md](docs/formats/ICC.md) |
| ISO base media / MP4 | [MP4.md](docs/formats/MP4.md) |
| Matroska / WebM | [MKV.md](docs/formats/MKV.md) |
| QuickTime | [QUICKTIME.md](docs/formats/QUICKTIME.md) |

This is an initial migration of substantive Java package documentation, not an
exhaustive support matrix. Formats without a document still require code, test,
and specification review; add a document when recording useful format knowledge.

## Focused custom agents

- [Format parity](.github/agents/format-parity.agent.md): investigate and fix a
  bounded cross-language discrepancy using specifications and identical inputs.
- [Parser value validation](.github/agents/parser-value-validation.agent.md):
  systematically trace input-controlled values and harden parser error handling
  and resource bounds within an explicitly selected scope.

Both agents prepare changes and PR content locally. Obtain explicit per-action
user approval before any push, PR creation, comment, or other shared write.
