using System;
using UnityEngine;

namespace RuStore.Internal {

    public class ErrorListener : AndroidJavaProxy {

        private Action<RuStoreError> onFailureAction;

        public ErrorListener(string className, Action<RuStoreError> onFailure) : base(className) {
            onFailureAction = onFailure;
        }

        public void OnFailure(AndroidJavaObject errorObject) {
            var error = ConvertError(errorObject);
            CallbackHandler.AddCallback(() => {
                onFailureAction(error);
            });
        }

        protected virtual RuStoreError ConvertError(AndroidJavaObject errorObject) =>
            ErrorDataConverter.ConvertError(errorObject);
    }
}
