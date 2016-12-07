using System;
using System.Threading.Tasks;

using Orleans;

namespace Orleankka
{
    using Core;
    using Services;
    using Utility;

    public abstract class Actor
    {
        ActorRef self;
        
        protected Actor()
        {}

        protected Actor(string id, IActorRuntime runtime, Dispatcher dispatcher = null)
        {
            Requires.NotNull(runtime, nameof(runtime));
            Requires.NotNullOrWhitespace(id, nameof(id));

            Runtime = runtime;
            Dispatcher = dispatcher ?? ActorType.Dispatcher(GetType());
            Path = GetType().ToActorPath(id);
        }

        internal void Initialize(ActorEndpoint endpoint, ActorPath path, IActorRuntime runtime, Dispatcher dispatcher)
        {
            Path = path;
            Runtime = runtime;
            Dispatcher = Dispatcher ?? dispatcher;
            Endpoint = endpoint;
        }

        public string Id => Path.Id;
        internal ActorEndpoint Endpoint {get; private set;}

        public ActorPath Path           {get; private set;}
        public IActorRuntime Runtime    {get; private set;}
        public Dispatcher Dispatcher    {get; private set;}

        public IActorSystem System           => Runtime.System;
        public IActivationService Activation => Runtime.Activation;
        public IReminderService Reminders    => Runtime.Reminders;
        public ITimerService Timers          => Runtime.Timers;

        public ActorRef Self => self ?? (self = System.ActorOf(Path));

        public virtual Task<object> OnReceive(object message) => 
            Dispatch(message);

        public virtual Task OnActivate()    => TaskDone.Done;
        public virtual Task OnDeactivate()  => TaskDone.Done;

        public virtual Task OnReminder(string id)
        {
            var message = $"Override {nameof(OnReminder)}() method in class {GetType()} to implement corresponding behavior";
            throw new NotImplementedException(message);
        }

        public async Task<TResult> Dispatch<TResult>(object message, Func<object, Task<object>> fallback = null) => 
            (TResult)await Dispatch(message, fallback);

        public Task<object> Dispatch(object message, Func<object, Task<object>> fallback = null)
        {
            Requires.NotNull(message, nameof(message));
            return Dispatcher.Dispatch(this, message, fallback);
        }
    }
}