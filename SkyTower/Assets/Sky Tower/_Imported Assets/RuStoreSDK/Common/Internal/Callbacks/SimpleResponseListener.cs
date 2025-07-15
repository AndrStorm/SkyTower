using System;
using UnityEngine;

namespace RuStore.Internal {

    public class SimpleResponseListener : ErrorListener {

        private Action onSuccessAction;

        public SimpleResponseListener(string className, Action<RuStoreError> onFailure, Action onSuccess) : base(className, onFailure) {
            onSuccessAction = onSuccess;
        }

        public void OnSuccess() {
            CallbackHandler.AddCallback(() => {
                onSuccessAction();
            });
        }
    }

    public class SimpleResponseListener<T> : ErrorListener {

        private Action<T> onSuccessAction;

        public SimpleResponseListener(string className, Action<RuStoreError> onFailure, Action<T> onSuccess) : base(className, onFailure) {
            onSuccessAction = onSuccess;
        }

        public void OnSuccess(T response) {
            CallbackHandler.AddCallback(() => {
                onSuccessAction(response);
            });
        }
    }
}
