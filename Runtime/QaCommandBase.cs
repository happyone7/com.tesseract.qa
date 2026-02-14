using System;
using UnityEngine;

namespace Tesseract.QA
{
    public abstract class QaCommandBase
    {
        public string Description { get; protected set; }

        public abstract void Execute();
    }

    public class ButtonCommand : QaCommandBase
    {
        public string btnText;
        public Action action;

        public ButtonCommand(string btnText, Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            base.Description = btnText;
            this.btnText = btnText;
            this.action = action;
        }

        public override void Execute()
        {
            action();
        }
    }

    public class KeyCodeCommand : QaCommandBase
    {
        public KeyCode Key;
        public Action action;

        public KeyCodeCommand(KeyCode key, Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            this.Description = GetSafeMethodName(action, key.ToString());
            this.Key = key;
            this.action = action;
        }

        public KeyCodeCommand(KeyCode key, string desc, Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            base.Description = desc;
            this.Key = key;
            this.action = action;
        }

        public override void Execute()
        {
            action();
        }

        private static string GetSafeMethodName(Action action, string fallback)
        {
            string name = action.Method.Name;
            // Compiler-generated names contain '<' (e.g., "<Start>b__0")
            if (string.IsNullOrEmpty(name) || name.Contains("<"))
                return fallback;
            return name;
        }
    }

    public abstract class StringCommandBase : QaCommandBase
    {
        public string Key;

        protected static string GetSafeMethodName(Delegate action, string fallbackKey)
        {
            string name = action.Method.Name;
            if (string.IsNullOrEmpty(name) || name.Contains("<"))
                return fallbackKey.Replace("/", "");
            return name;
        }
    }

    public class StringCommand : StringCommandBase
    {
        public Action action;

        public StringCommand(string key, Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            this.Description = GetSafeMethodName(action, key);
            base.Key = key;
            this.action = action;
        }

        public StringCommand(string key, string desc, Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            base.Description = desc;
            base.Key = key;
            this.action = action;
        }

        public override void Execute()
        {
            action();
        }
    }

    public class StringCommand<T1> : StringCommandBase
    {
        public Action<T1> action;

        public StringCommand(string key, Action<T1> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            this.Description = GetSafeMethodName(action, key);
            base.Key = key;
            this.action = action;
        }

        public StringCommand(string key, string desc, Action<T1> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            base.Description = desc;
            base.Key = key;
            this.action = action;
        }

        public override void Execute()
        {
            Debug.LogWarning($"[QA] StringCommand<{typeof(T1).Name}> '{Key}' requires parameters. Use ExecuteStringCommand() instead.");
        }

        public void Execute(T1 param1)
        {
            action(param1);
        }
    }

    public class StringCommand<T1, T2> : StringCommandBase
    {
        public Action<T1, T2> action;

        public StringCommand(string key, Action<T1, T2> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            this.Description = GetSafeMethodName(action, key);
            base.Key = key;
            this.action = action;
        }

        public StringCommand(string key, string desc, Action<T1, T2> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            base.Description = desc;
            base.Key = key;
            this.action = action;
        }

        public override void Execute()
        {
            Debug.LogWarning($"[QA] StringCommand<{typeof(T1).Name},{typeof(T2).Name}> '{Key}' requires parameters. Use ExecuteStringCommand() instead.");
        }

        public void Execute(T1 param1, T2 param2)
        {
            action(param1, param2);
        }
    }

    public class StringCommand<T1, T2, T3> : StringCommandBase
    {
        public Action<T1, T2, T3> action;

        public StringCommand(string key, Action<T1, T2, T3> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            this.Description = GetSafeMethodName(action, key);
            base.Key = key;
            this.action = action;
        }

        public StringCommand(string key, string desc, Action<T1, T2, T3> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            base.Description = desc;
            base.Key = key;
            this.action = action;
        }

        public override void Execute()
        {
            Debug.LogWarning($"[QA] StringCommand<{typeof(T1).Name},{typeof(T2).Name},{typeof(T3).Name}> '{Key}' requires parameters. Use ExecuteStringCommand() instead.");
        }

        public void Execute(T1 param1, T2 param2, T3 param3)
        {
            action(param1, param2, param3);
        }
    }

    public class StringCommand<T1, T2, T3, T4> : StringCommandBase
    {
        public Action<T1, T2, T3, T4> action;

        public StringCommand(string key, Action<T1, T2, T3, T4> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            this.Description = GetSafeMethodName(action, key);
            base.Key = key;
            this.action = action;
        }

        public StringCommand(string key, string desc, Action<T1, T2, T3, T4> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            base.Description = desc;
            base.Key = key;
            this.action = action;
        }

        public override void Execute()
        {
            Debug.LogWarning($"[QA] StringCommand<{typeof(T1).Name},{typeof(T2).Name},{typeof(T3).Name},{typeof(T4).Name}> '{Key}' requires parameters. Use ExecuteStringCommand() instead.");
        }

        public void Execute(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            action(param1, param2, param3, param4);
        }
    }
}
