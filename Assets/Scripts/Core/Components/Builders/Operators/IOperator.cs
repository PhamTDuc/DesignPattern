
namespace Guinea.Core.Components
{
    public interface IOperator
    {
        public abstract Result Invoke(Context context);
        public abstract Result Execute(Context context);
        public abstract ICommand Finish(Context context);
        public abstract Result Cancel(Context context);


        public enum Result
        {
            NO_OPERATOR,
            INVOKE,
            RUNNING_MODAL,
            PASS_THROUGH,
            CANCELLED,
            FINISHED,
        }
    }
}