using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tesseract.QA
{
    [DefaultExecutionOrder(-1)]
    public class QaManager : MonoBehaviour
    {
        public static QaManager Instance { get; private set; }

        public bool IsCommandChangedThisFrame { get; private set; }
        public bool CheckKeyCodeInput { get; set; } = true;

        private Dictionary<string, List<StringCommandBase>> _stringCommands = new Dictionary<string, List<StringCommandBase>>();
        private Dictionary<KeyCode, KeyCodeCommand> _keyCodeCommands = new Dictionary<KeyCode, KeyCodeCommand>();
        private List<ButtonCommand> _buttonCommands = new List<ButtonCommand>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            if (CheckKeyCodeInput && Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in _keyCodeCommands.Keys)
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        ExecuteKeyCodeCommand(keyCode);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            IsCommandChangedThisFrame = false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public List<ButtonCommand> GetButtonCommands()
        {
            return _buttonCommands;
        }

        public void RegisterCommand(QaCommandContainer container)
        {
            List<QaCommandBase> commands = container.GetCommands();
            foreach (var command in commands)
            {
                RegisterCommand(command);
            }
        }

        public void UnregisterCommand(QaCommandContainer container)
        {
            List<QaCommandBase> commands = container.GetCommands();
            foreach (var command in commands)
            {
                UnregisterCommand(command);
            }
        }

        public void RegisterCommand(QaCommandBase command)
        {
            IsCommandChangedThisFrame = true;
            if (command is StringCommandBase sCommand)
            {
                if (_stringCommands.TryGetValue(sCommand.Key, out var sCommandList) == false)
                {
                    sCommandList = new List<StringCommandBase>();
                    _stringCommands.Add(sCommand.Key, sCommandList);
                }
                sCommandList.Add(sCommand);
            }
            else if (command is KeyCodeCommand kCommand)
            {
                _keyCodeCommands[kCommand.Key] = kCommand;
            }
            else if (command is ButtonCommand bCommand)
            {
                _buttonCommands.Add(bCommand);
            }
        }

        public void UnregisterCommand(QaCommandBase command)
        {
            IsCommandChangedThisFrame = true;
            if (command is StringCommandBase sCommand)
            {
                if (_stringCommands.TryGetValue(sCommand.Key, out var sCommandList))
                {
                    sCommandList.Remove(sCommand);
                }
            }
            else if (command is KeyCodeCommand kCommand)
            {
                _keyCodeCommands.Remove(kCommand.Key);
            }
            else if (command is ButtonCommand bCommand)
            {
                _buttonCommands.Remove(bCommand);
            }
        }

        public void ExecuteStringCommand(string inputCommand)
        {
            if (string.IsNullOrWhiteSpace(inputCommand))
                return;

            string[] split = inputCommand.Split(' ');
            string key = split[0];

            if (_stringCommands.TryGetValue(key, out var stringCommands) == false ||
                stringCommands.Count == 0)
            {
                return;
            }

            int inputArgCount = split.Length - 1;
            List<StringCommandBase> targets = new List<StringCommandBase>(stringCommands);
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                StringCommandBase cmd = targets[i];
                System.Type[] arguments = cmd.GetType().GetGenericArguments();
                if (arguments.Length > inputArgCount)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                bool invalidArgType = false;
                for (int j = 0; j < arguments.Length; j++)
                {
                    var arg = arguments[j];
                    var inputArg = split[j + 1];
                    if (arg == typeof(int))
                    {
                        if (int.TryParse(inputArg, out _) == false)
                        {
                            invalidArgType = true;
                            break;
                        }
                    }
                    else if (arg == typeof(float))
                    {
                        if (float.TryParse(inputArg, out _) == false)
                        {
                            invalidArgType = true;
                            break;
                        }
                    }
                    else if (arg == typeof(string))
                    {
                        // Do nothing
                    }
                    else if (arg == typeof(bool))
                    {
                        if (bool.TryParse(inputArg, out _) == false)
                        {
                            invalidArgType = true;
                            break;
                        }
                    }
                    else
                    {
                        invalidArgType = true;
                        break;
                    }
                }

                if (invalidArgType)
                {
                    targets.RemoveAt(i);
                    continue;
                }
            }

            if (targets.Count == 0)
                return;

            // Sort by parameter count (ascending) and pick the best match
            targets = targets.OrderBy(cmd => cmd.GetType().GenericTypeArguments.Length).ToList();

            // parameter 개수가 가장 비슷한 것으로 적용
            StringCommandBase targetCommand = targets.Last();
            var commandArgs = targetCommand.GetType().GenericTypeArguments;
            var method = targetCommand.GetType().GetMethod("Execute", 0, commandArgs);
            if (method == null)
            {
                Debug.LogWarning($"[QaManager] Execute method not found for command '{key}'");
                return;
            }

            var args = new object[commandArgs.Length];
            for (int i = 0; i < commandArgs.Length; i++)
            {
                var inputArg = split[i + 1];
                if (commandArgs[i] == typeof(int))
                {
                    args[i] = int.Parse(inputArg);
                }
                else if (commandArgs[i] == typeof(float))
                {
                    args[i] = float.Parse(inputArg);
                }
                else if (commandArgs[i] == typeof(string))
                {
                    args[i] = inputArg;
                }
                else if (commandArgs[i] == typeof(bool))
                {
                    args[i] = bool.Parse(inputArg);
                }
            }

            method.Invoke(targetCommand, args);
        }

        public void ExecuteKeyCodeCommand(KeyCode key)
        {
            if (_keyCodeCommands.TryGetValue(key, out var keyCodeCommand) == false)
            {
                Debug.LogWarning($"No command for key '{key}'");
                return;
            }

            keyCodeCommand.Execute();
        }

        [ContextMenu("Log All Commands")]
        public void LogAllCommands()
        {
            string log = GetAllCommandsLog();
            Debug.Log(log);
        }

        public string GetAllCommandsLog()
        {
            StringBuilder sb = new StringBuilder();
            if (_stringCommands.Count > 0)
            {
                sb.AppendLine("<color=cyan>String Commands</color>");
                foreach (var cmdList in _stringCommands.Values)
                {
                    foreach (var stringCommand in cmdList)
                    {
                        sb.AppendLine($"<color=green>Key:</color> <color=white>{stringCommand.Key}</color>  <color=green>{stringCommand.Description}</color>");
                    }
                }
                sb.AppendLine();
            }

            if (_keyCodeCommands.Count > 0)
            {
                sb.AppendLine("<color=cyan>KeyCode Commands</color>");
                foreach (var keyCommand in _keyCodeCommands.Values)
                {
                    sb.AppendLine($"<color=green>Key:</color> <color=white>{keyCommand.Key}</color>  <color=green>{keyCommand.Description}</color>");
                }
                sb.AppendLine();
            }

            if (_buttonCommands.Count > 0)
            {
                sb.AppendLine("<color=cyan>Button Commands</color>");
                foreach (var btnCommand in _buttonCommands)
                {
                    sb.AppendLine($"  <color=green>{btnCommand.Description}</color>");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
