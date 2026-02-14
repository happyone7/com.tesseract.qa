# Changelog

## [1.1.0] - 2026-02-14
### Fixed
- Singleton Awake: Destroy duplicate gameObject instead of replacing Instance
- Action null check: All command constructors now throw ArgumentNullException
- OrderBy result: Sort result now properly assigned to targets list
- GetMethod null check: Added null guard before method.Invoke()
- Empty Execute() overrides: Now log warning about missing parameters
- Lambda method names: GetSafeMethodName detects compiler-generated names
- Lazy init: QaCommandContainer uses _initialized flag instead of count check
- Empty input: ExecuteStringCommand returns early on null/whitespace
- Removed unused Start() method
- Added OnDestroy to clean up static Instance

## [1.0.0] - 2026-02-14
### Added
- QaManager: Singleton command manager with string/keycode/button command support
- QaCommandBase: Base class hierarchy for commands
- StringCommand<T1..T4>: Generic string commands with reflection-based parameter parsing
- KeyCodeCommand: Keyboard shortcut commands
- ButtonCommand: UI button commands
- QaCommandContainer: Abstract container for grouping commands
