using UnityEngine;
using System;

namespace RuStore.CoreClient {

    /// <summary>
    /// Класс реализует коллекцию вспомогательных методов для взаимодействия с приложением RuStore.
    /// </summary>
    public class RuStoreCoreClient {

        /// <summary>
        /// Версия плагина.
        /// </summary>
        public static string PluginVersion = "9.1.0";

        private static RuStoreCoreClient instance = null;
        private AndroidJavaObject clientWrapper { get; }

        /// <summary>
        /// Возвращает единственный экземпляр RuStoreCoreClient (реализация паттерна Singleton).
        /// Если экземпляр еще не создан, создает его.
        /// </summary>
        public static RuStoreCoreClient Instance {
            get {
                if (instance == null) instance = new RuStoreCoreClient();

                return instance;
            }
        }

        private RuStoreCoreClient() {
            if (!IsPlatformSupported()) return;

            CallbackHandler.InitInstance();
            using (var clientJavaClass = new AndroidJavaClass("ru.rustore.unitysdk.core.RuStoreUnityCoreClient")) {
                clientWrapper = clientJavaClass.GetStatic<AndroidJavaObject>("INSTANCE");
            }
        }

        /// <summary>
        /// Проверка наличия приложения RuStore на устройстве пользователя.
        /// </summary>
        /// <returns>Возвращает true, если RuStore установлен, в противном случае — false.</returns>
        public bool IsRuStoreInstalled() {
            if (!IsPlatformSupported()) return false;

            return clientWrapper?.Call<bool>("isRuStoreInstalled") ?? false;
        }

        /// <summary>
        /// Открыть веб-страницу для скачивания приложения RuStore.
        /// </summary>
        public void openRuStoreDownloadInstruction() {
            if (IsPlatformSupported()) clientWrapper?.Call("openRuStoreDownloadInstruction");
        }

        /// <summary>
        /// Запуск приложения RuStore.
        /// </summary>
        public void openRuStore() {
            if (IsPlatformSupported()) clientWrapper?.Call("openRuStore");
        }

        /// <summary>
        /// Запуск приложения RuStore для авторизации.
        /// После успешной авторизации пользователя приложение RuStore автоматически закроется.
        /// </summary>
        public void openRuStoreAuthorization() {
            if (IsPlatformSupported()) clientWrapper?.Call("openRuStoreAuthorization");
        }

        private bool IsPlatformSupported(Action<RuStoreError> onFailure = null) {
            bool isSupported = Application.platform == RuntimePlatform.Android;

            if (!isSupported) {
                onFailure?.Invoke(new RuStoreError() {
                    name = "RuStoreCoreClientError",
                    description = "Unsupported platform"
                });
            }

            return isSupported;
        }
    }
}
