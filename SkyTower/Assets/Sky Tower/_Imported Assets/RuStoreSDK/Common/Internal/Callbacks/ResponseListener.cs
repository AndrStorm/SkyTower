using System;
using UnityEngine;

namespace RuStore.Internal {

    public abstract class ResponseListener<T> : ErrorListener {

        private Action<T> onSuccessAction;

        public ResponseListener(string className, Action<RuStoreError> onFailure, Action<T> onSuccess) : base(className, onFailure) {
            onSuccessAction = onSuccess;
        }

        public void OnSuccess(AndroidJavaObject responseObject) {
            var response = ConvertResponse(responseObject);
            CallbackHandler.AddCallback(() => {
                onSuccessAction(response);
            });
        }

        protected abstract T ConvertResponse(AndroidJavaObject responseObject);
    }
}
