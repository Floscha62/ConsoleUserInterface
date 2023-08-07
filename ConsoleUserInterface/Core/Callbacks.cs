namespace ConsoleUserInterface.Core;

public class Callbacks {

    private int nextIndex = 0;
    private readonly List<object> callbacks = new ();
    private readonly List<HashSet<object>> callbackDependencies = new ();

    internal Callbacks() {

    }

    internal void Reset() {
        nextIndex = 0;
    }

    private object CreateInternal(object callback, HashSet<object> dependencies) {
        if (callbacks.Count <= nextIndex) {
            callbacks.Add(callback);
            callbackDependencies.Add(dependencies);
            nextIndex++;
            return callback;
        } else if (callbackDependencies[nextIndex].SetEquals(dependencies)) {
            return callbacks[nextIndex++];
        } else {
            callbacks[nextIndex] = callback;
            callbackDependencies[nextIndex] = dependencies;
            nextIndex++;
            return callback;
        }
    }

    public Action Create(Action action, HashSet<object> dependencies) => (Action)CreateInternal(action, dependencies);

    public Action<T> Create<T>(Action<T> action, HashSet<object> dependencies) => (Action<T>)CreateInternal(action, dependencies);

    public Action<T1, T2> Create<T1, T2>(Action<T1, T2> action, HashSet<object> dependencies) => (Action<T1, T2>)CreateInternal(action, dependencies);

    public Action<T1, T2, T3> Create<T1, T2, T3>(Action<T1, T2, T3>action, HashSet<object> dependencies) => (Action<T1, T2, T3>)CreateInternal(action, dependencies);

    public Func<TResult> Create<TResult>(Func<TResult> action, HashSet<object> dependencies) => (Func<TResult>)CreateInternal(action, dependencies);

    public Func<T1, TResult> Create<T1, TResult>(Func<T1, TResult> action, HashSet<object> dependencies) => (Func<T1, TResult>)CreateInternal(action, dependencies);

    public Func<T1, T2, TResult> Create<T1, T2, TResult>(Func<T1, T2, TResult> action, HashSet<object> dependencies) => (Func<T1, T2, TResult>)CreateInternal(action, dependencies);

    public Func<T1, T2, T3, TResult> Create<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> action, HashSet<object> dependencies) => (Func<T1, T2, T3, TResult>)CreateInternal(action, dependencies);
}
