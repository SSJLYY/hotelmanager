namespace EOM.TSHotelManagement.Infrastructure
{
    public class CodeConstantBase<T> where T : CodeConstantBase<T>
    {
        public string Code { get; }
        public string Description { get; }

        private static readonly List<T> _constants = new List<T>();

        protected CodeConstantBase(string code, string description)
        {
            Code = code;
            Description = description;
            _constants.Add((T)this);
        }

        public static IEnumerable<T> GetAll()
        {
            EnsureInitialized();
            return _constants;
        }

        public static string GetDescriptionByCode(string code)
        {
            return GetConstantByCode(code)?.Description ?? string.Empty;
        }

        public static string GetCodeByDescription(string description)
        {
            return GetConstantByDescription(description)?.Code ?? string.Empty;
        }

        public static T? GetConstantByCode(string code)
        {
            EnsureInitialized();
            var normalizedCode = NormalizeValue(code);
            if (string.IsNullOrWhiteSpace(normalizedCode))
            {
                return null;
            }

            return _constants.FirstOrDefault(c => string.Equals(c.Code, normalizedCode, StringComparison.OrdinalIgnoreCase));
        }

        public static T? GetConstantByDescription(string description)
        {
            EnsureInitialized();
            var normalizedDescription = NormalizeValue(description);
            if (string.IsNullOrWhiteSpace(normalizedDescription))
            {
                return null;
            }

            return _constants.FirstOrDefault(c => string.Equals(c.Description, normalizedDescription, StringComparison.OrdinalIgnoreCase));
        }

        public static bool TryGetDescriptionByCode(string code, out string description)
        {
            description = GetDescriptionByCode(code);
            return !string.IsNullOrWhiteSpace(description);
        }

        public static bool TryGetCodeByDescription(string description, out string code)
        {
            code = GetCodeByDescription(description);
            return !string.IsNullOrWhiteSpace(code);
        }

        private static string NormalizeValue(string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        private static void EnsureInitialized()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        }
    }
}
