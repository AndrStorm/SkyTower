using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RuStore.Response.Internal.ReviewResponseListener")]

namespace RuStore {

    /// <summary>
    /// Информация об ошибке.
    /// </summary>
    public class RuStoreError {

        /// <summary>
        /// Название ошибки.
        /// Содержит имя simpleName класса ошибки.
        /// </summary>
        public string name;

        /// <summary>
        /// Сообщение ошибки.
        /// </summary>
        public string description;
    }
}
