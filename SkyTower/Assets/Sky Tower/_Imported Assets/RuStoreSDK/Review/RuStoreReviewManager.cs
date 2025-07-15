using UnityEngine;
using System;
using RuStore.Internal;
using RuStore.Response.Internal;

namespace RuStore.Review {

    /// <summary>
    /// Класс для работы с оценками и отзывами.
    /// Предоставляет API для запуска UI-формы, позволяющей пользователю оставить оценку и отзыв о вашем приложении в "RuStore".
    /// </summary>
    public class RuStoreReviewManager {

        /// <summary>
        /// Версия плагина.
        /// </summary>
        public static string PluginVersion = "9.1.0";

        private static RuStoreReviewManager _instance;
        private static bool _isInstanceInitialized;

        private bool _isInitialized;

        /// <summary>
        /// Возвращает true, если синглтон инициализирован, в противном случае — false.
        /// </summary>
        public bool IsInitialized => _isInitialized;
        private AndroidJavaObject _managerWrapper;

        /// <summary>
        /// Возвращает единственный экземпляр RuStoreReviewManager (реализация паттерна Singleton).
        /// Если экземпляр еще не создан, создает его.
        /// </summary>
        public static RuStoreReviewManager Instance {
            get {
                if (!_isInstanceInitialized) {
                    _isInstanceInitialized = true;
                    _instance = new RuStoreReviewManager();
                }
                return _instance;
            }
        }

        private RuStoreReviewManager() {
        }

        /// <summary>
        /// Выполняет инициализацию синглтона RuStoreReviewManager.
        /// </summary>
        public bool Init() {
            if (_isInitialized) {
                Debug.LogError("Error: RuStore In-App Review Manager is already initialized");
                return false;
            }

            if (Application.platform != RuntimePlatform.Android) {
                return false;
            }

            CallbackHandler.InitInstance();

            using (var managerClass = new AndroidJavaClass("ru.rustore.unitysdk.review.RuStoreUnityReviewManager")) {
                _managerWrapper = managerClass.GetStatic<AndroidJavaObject>("INSTANCE");
            }

            _managerWrapper.Call("init", "unity");
            _isInitialized = true;

            return true;
        }

        /// <summary>
        /// Выполняет подготовку данных для запуска формы оценки и отзыва.
        /// Метод должен быть вызван перед каждым вызовом LaunchReviewFlow.
        /// Время жизни подготовленных данных — около пяти минут.
        /// <param name="onFailure">
        /// Действие, выполняемое в случае ошибки.
        /// Возвращает объект RuStore.RuStoreError с информацией об ошибке.
        /// </param>
        /// <param name="onSuccess">
        /// Действие, выполняемое при успешном завершении операции.
        /// </param>
        /// </summary>
        public void RequestReviewFlow(Action<RuStoreError> onFailure, Action onSuccess) {
            if (!IsPlatformSupported(onFailure)) {
                return;
            }

            var listener = new ReviewResponseListener(onFailure, onSuccess);
            _managerWrapper.Call("requestReviewFlow", listener);
        }

        /// <summary>
        /// Выполняет запуск формы для запроса оценки и отзыва у пользователя.
        /// Каждому вызову метода должен предшествовать вызов RequestReviewFlow.
        /// <param name="onFailure">
        /// Действие, выполняемое в случае ошибки.
        /// Возвращает объект RuStoreError с информацией об ошибке.
        /// </param>
        /// <param name="onSuccess">
        /// Действие, выполняемое при успешном завершении операции.
        /// </param>
        /// </summary>
        public void LaunchReviewFlow(Action<RuStoreError> onFailure, Action onSuccess) {
            if (!IsPlatformSupported(onFailure)) {
                return;
            }

            var listener = new ReviewResponseListener(onFailure, onSuccess);
            _managerWrapper.Call("launchReviewFlow", listener);
        }

        private bool IsPlatformSupported(Action<RuStoreError> onFailure) {
            if (Application.platform != RuntimePlatform.Android) {
                onFailure?.Invoke(new RuStoreError() {
                    name = "RuStoreReviewManagerError",
                    description = "Unsupported platform"
                });
                return false;
            }

            return true;
        }
    }
}
