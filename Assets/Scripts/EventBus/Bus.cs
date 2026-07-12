namespace GameDevTV.RTS.EventBus{

    public static class Bus<T> where T: IEvent
    {
        // A delegate is a type that can hold a function.
        // This just means:
        // "Define a type named Event that represents any function returning void and taking one parameter of type T."
        // this is a (list of) pointer (s) to a function(s) that fits this description (this is called multicasting)
        // we can += or -= to append / remove from this list of pointers
        public delegate void Event(T args);
        
        // Define a variable called "OnEvent" that is a pointer to a function that fits the description
        // The "event" keyword means that only the Bus class can invoke the OnEvent delegate.
        public static event Event OnEvent;

        // Expression-bodied function
        public static void Raise(T evt) => OnEvent?.Invoke(evt);
    }

}