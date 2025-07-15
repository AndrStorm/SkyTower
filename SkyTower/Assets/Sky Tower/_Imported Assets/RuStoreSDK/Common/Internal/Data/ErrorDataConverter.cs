using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuStore.Internal {

    public static class ErrorDataConverter
    {
        public static RuStoreError ConvertError(AndroidJavaObject errorObject)
        {
            var error = new RuStoreError();
            using (var errorJavaClass = errorObject.Call<AndroidJavaObject>("getClass"))
            {
                error.name = errorJavaClass.Call<string>("getSimpleName");
                error.description = errorObject.Call<string>("getMessage");
            }

            return error;
        }
    }
}
