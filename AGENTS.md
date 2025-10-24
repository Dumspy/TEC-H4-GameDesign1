# AGENTS.md

## Build, Lint, and Test Commands
- **Build:** Use Unity Editor (Ctrl+B) or CLI (`unity -quit -batchmode -buildTarget <platform> -projectPath <path>`)
- **Test:** Use Unity Test Runner (Editor: Window > General > Test Runner)
- **Run Single Test:** In Test Runner, select and run individual tests. CLI: `unity -runTests -testPlatform PlayMode -testFilter <TestName>`
- **Lint:** No custom linter; follow C# and Unity conventions.

## Code Style Guidelines
- **Imports:** Use `using` statements at the top, group Unity namespaces first.
- **Formatting:** 4 spaces per indent, braces on new lines, no trailing whitespace.
- **Types:** Use explicit types; prefer `var` only for obvious types.
- **Naming:**
  - Classes: PascalCase
  - Methods: PascalCase
  - Private fields: camelCase, use `[SerializeField]` for inspector
  - Constants: PascalCase or ALL_CAPS for static readonly
- **Error Handling:** Use Unity patterns (null checks, `Destroy`, `DontDestroyOnLoad`). Avoid exceptions for control flow.
- **MonoBehaviour:** Use Unity lifecycle methods (`Start`, `Update`, `Awake`).
- **Singletons:** Use `Instance` property, destroy duplicates in `Awake`.
- **Comments:** Use XML doc comments for public APIs, inline comments for complex logic.
- **Serialization:** Use `[Serializable]` and `[SerializeField]` for Unity inspector fields.

_No Cursor or Copilot rules detected._
