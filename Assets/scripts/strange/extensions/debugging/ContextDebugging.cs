using UnityEngine;
using System.Collections;
using strange.extensions.command.api;
using System;
using strange.extensions.signal.impl;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using strange.extensions.context.impl;
using strange.framework.impl;
using strange.framework.api;

public class ContextDebugging {

    public static void OnCommandExecute(ICommand c) {
#if UNITY_EDITOR
        if (!Context.DebuggingOptions.LogCommandExecutions)
            return;

        Type type = c.GetType();
        string className = type.Name;
        className = FilterEndingsAndJoin(CamelCaseToArray(className));

        Debug.Log(className.Colored(Colors.white) + " - " + DetermineCommandType(type.Name) + " Executed  \n\n" + type.Name + "\n");
#endif
    }

    public static void OnSignalDispatch(BaseSignal s, int nonbaseListeners, params object[] parametersToStrings) {
#if UNITY_EDITOR
        if (!Context.DebuggingOptions.LogSignalDispatches)
            return;

        Type type = s.GetType();
        string className = type.Name;

        if (type.IsNested) {
            string[] parts = type.FullName.Split('.');
            parts = parts[parts.Length - 1].Split('+');
            Assert.IsTrue(parts.Length == 2);
            className = FilterEndingsAndJoin(CamelCaseToArray(parts[0])) + " -> " + ColourNestedSignal(parts[1]);
        } else {
            className = FilterEndingsAndJoin(CamelCaseToArray(className));
        }

        int count = s.BaseSubscriptionCount + nonbaseListeners;

        string p = "";
        if (parametersToStrings.Length > 0) {
            p = "Parameters: (";

            for (int i = 0; i < parametersToStrings.Length; i++) {
                p += parametersToStrings[i].ToString();

                if (i > 0)
                    p += ", ";
            }

            p += ")";
        }

        Debug.Log(className.Colored(Colors.teal) + " - " + DetermineSignalType(type.Name) + " Dispatched to " + count + " subscriber(s) \n" + p + "\n" + type.FullName + "\n");
#endif
    }

    public static void OnNewBinderInstance(IBinder binder) {
#if UNITY_EDITOR
        if (!Context.DebuggingOptions.LogBinderCreation)
            return;

        Debug.Log("New Binder: " + binder.GetBinderName());
#endif
    }

    private static string DetermineSignalType(string name) {
        for (int i = 0; i < Context.DebuggingOptions.SignalTypes.Count; i++) {
            if (name.EndsWith(Context.DebuggingOptions.SignalTypes[i] + "Signal")) {
                return Context.DebuggingOptions.SignalTypes[i] + " Signal";
            }
        }
        return "Signal";
    }

    private static string DetermineCommandType(string name) {
        for (int i = 0; i < Context.DebuggingOptions.CommandTypes.Count; i++) {
            if (name.EndsWith(Context.DebuggingOptions.CommandTypes[i] + "Command")) {
                return Context.DebuggingOptions.CommandTypes[i] + " Command";
            }
        }
        return "Command";
    }

    private static string ColourNestedSignal(string nestedSignalName) {
        if (nestedSignalName == "Success")
            return nestedSignalName.Colored(Colors.green);
        else if (nestedSignalName == "Failure")
            return nestedSignalName.Colored(Colors.red);
        else if (nestedSignalName == "Available")
            return nestedSignalName.Colored(Colors.green);
        else if (nestedSignalName == "NotAvailable")
            return nestedSignalName.Colored(Colors.red);
        else if (nestedSignalName == "Completed")
            return nestedSignalName.Colored(Colors.yellow);
        else
            throw new Exception("Unknown nested signal type: " + nestedSignalName);
    }

    private static string[] CamelCaseToArray(string camelCase) {
        return Regex.Split(camelCase, @"(?=\p{Lu}\p{Ll})| (?<=\p{Ll})(?=\p{Lu})");
    }

    private static string FilterEndingsAndJoin(string[] words) {
        if (Context.DebuggingOptions.IgnoreNamingConventions)
            return string.Join(" ", words);

        Assert.IsTrue(words.Length > 1);
        int ignoreAtEnd = 1;

        string last = words[words.Length - 1];
        if (last == "Signal") {
            if (Context.DebuggingOptions.SignalTypes.Contains(words[words.Length - 2])) {
                ignoreAtEnd = 2;
            }
        } else if (last == "Command") {
            if (Context.DebuggingOptions.CommandTypes.Contains(words[words.Length - 2])) {
                ignoreAtEnd = 2;
            }
        } else {
            throw new Exception("Invalidly named signal or command: " + String.Join("", words));
        }

        return JoinIgnoring(words, words[0] == "" ? 1 : 0, ignoreAtEnd);
    }

    private static string JoinIgnoring(string[] words, int ignoreAtStart, int ignoreAtEnd) {
        Assert.IsTrue(ignoreAtStart + ignoreAtEnd <= words.Length);
        string ret = words[ignoreAtStart];
        for (int i = ignoreAtStart + 1; i < words.Length - ignoreAtEnd; i++) {
            ret += " " + words[i];
        }
        return ret;
    }

}