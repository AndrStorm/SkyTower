namespace RuStore {

    /// <summary>
    /// Проверка доступности функционала.
    /// </summary>
    public class FeatureAvailabilityResult {

        /// <summary>
        /// Информация о доступности.
        /// Если все условия выполняются, возвращается RuStore.FeatureAvailabilityResult.isAvailable == true.
        /// В противном случае возвращается RuStore.FeatureAvailabilityResult.isAvailable == false.
        /// </summary>
        public bool isAvailable;

        /// <summary>
        /// Информация об ошибке.
        /// </summary>
        public RuStoreError cause;
    }
}
