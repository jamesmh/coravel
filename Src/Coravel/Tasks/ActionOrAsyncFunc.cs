using System;
using System.Threading.Tasks;

namespace Coravel.Tasks
{
    public class ActionOrAsyncFunc
    {
        private Action _action;
        private Func<Task> _asyncAction;
        private bool _isAsync;

        public ActionOrAsyncFunc(Action action) {
            this._isAsync = false;
            this._action = action;
        }

        public ActionOrAsyncFunc(Func<Task> asyncAction) {
            this._isAsync = true;
            this._asyncAction = asyncAction;
        }

        public async Task Invoke(){
            if(this._isAsync) {
                await this._asyncAction();
            }
            else {
                this._action();
            }
        }
    }
}