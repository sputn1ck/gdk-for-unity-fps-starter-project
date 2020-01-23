using Fps.UI;

namespace Fps.StateMachine
{
    public class DefaultInitState : DefaultState
    {
        public DefaultInitState(BBHUIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Manager.ShowFrontEnd();
            ScreenManager.SwitchToDefaultScreen();
            Owner.SetState(new DefaultConnectState(Manager, Owner));
        }
    }
}
