# Changelog

## [1.0.0] - 2026-02-14
### Added
- QaManager: Singleton command manager with string/keycode/button command support
- QaCommandBase: Base class hierarchy for commands
- StringCommand<T1..T4>: Generic string commands with reflection-based parameter parsing
- KeyCodeCommand: Keyboard shortcut commands
- ButtonCommand: UI button commands
- QaCommandContainer: Abstract container for grouping commands
