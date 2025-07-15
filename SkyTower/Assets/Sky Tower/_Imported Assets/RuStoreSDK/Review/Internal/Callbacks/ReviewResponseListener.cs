using RuStore.Internal;
using System;
using UnityEngine;

namespace RuStore.Response.Internal {

    public class ReviewResponseListener : SimpleResponseListener {

        private const string javaClassName = "ru.rustore.unitysdk.review.ReviewResponseListener";

        public ReviewResponseListener(Action<RuStoreError> onFailure, Action onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }
    }
}
