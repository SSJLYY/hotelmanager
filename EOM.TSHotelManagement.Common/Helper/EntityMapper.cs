using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mapster;

namespace EOM.TSHotelManagement.Common
{
    public static class EntityMapper
    {
        /// <summary>
        /// Maps a single object instance.
        /// </summary>
        public static TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            if (source == null) return default;

            try
            {
                var destination = source.Adapt<TDestination>();
                ApplyLegacyOverrides(source, destination);
                return destination;
            }
            catch
            {
                return LegacyMap<TSource, TDestination>(source);
            }
        }

        /// <summary>
        /// Maps a list of objects.
        /// </summary>
        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> sourceList)
            where TDestination : new()
        {
            return sourceList?.Select(Map<TSource, TDestination>).ToList();
        }

        // Preserve the legacy edge cases while Mapster handles the common path.
        private static void ApplyLegacyOverrides<TSource, TDestination>(TSource source, TDestination destination)
        {
            var sourceProperties = typeof(TSource).GetProperties(
                BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = typeof(TDestination).GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var destinationProperty in destinationProperties)
            {
                if (!destinationProperty.CanWrite) continue;

                var exactSourceProperty = sourceProperties
                    .SingleOrDefault(p => p.Name.Equals(destinationProperty.Name, StringComparison.Ordinal));
                var matchedSourceProperty = exactSourceProperty ?? sourceProperties
                    .SingleOrDefault(p => p.Name.Equals(destinationProperty.Name, StringComparison.OrdinalIgnoreCase));

                if (matchedSourceProperty == null) continue;

                var sourceValue = matchedSourceProperty.GetValue(source);

                if (sourceValue == null)
                {
                    if (destinationProperty.Name.Equals("RowVersion", StringComparison.OrdinalIgnoreCase)
                        && destinationProperty.PropertyType == typeof(long))
                    {
                        destinationProperty.SetValue(destination, 0L);
                        continue;
                    }

                    if (destinationProperty.PropertyType.IsValueType &&
                        Nullable.GetUnderlyingType(destinationProperty.PropertyType) != null)
                    {
                        destinationProperty.SetValue(destination, null);
                    }

                    continue;
                }

                if (matchedSourceProperty.Name.Equals(destinationProperty.Name, StringComparison.Ordinal) &&
                    !NeedConversion(matchedSourceProperty.PropertyType, destinationProperty.PropertyType))
                {
                    continue;
                }

                destinationProperty.SetValue(
                    destination,
                    ConvertValue(sourceValue, destinationProperty.PropertyType));
            }
        }

        private static TDestination LegacyMap<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            var destination = new TDestination();

            var sourceProperties = typeof(TSource).GetProperties(
                BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = typeof(TDestination).GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties
                    .SingleOrDefault(p => p.Name.Equals(
                        sourceProperty.Name,
                        StringComparison.OrdinalIgnoreCase
                    ));

                if (destinationProperty == null || !destinationProperty.CanWrite) continue;

                var sourceValue = sourceProperty.GetValue(source);

                if (sourceValue == null)
                {
                    if (destinationProperty.Name.Equals("RowVersion", StringComparison.OrdinalIgnoreCase)
                        && destinationProperty.PropertyType == typeof(long))
                    {
                        destinationProperty.SetValue(destination, 0L);
                        continue;
                    }

                    if (destinationProperty.PropertyType.IsValueType &&
                        Nullable.GetUnderlyingType(destinationProperty.PropertyType) != null)
                    {
                        destinationProperty.SetValue(destination, null);
                    }

                    continue;
                }

                destinationProperty.SetValue(
                    destination,
                    ConvertValue(sourceValue, destinationProperty.PropertyType));
            }

            return destination;
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (!NeedConversion(value.GetType(), targetType))
            {
                return value;
            }

            return SmartConvert(value, targetType);
        }

        /// <summary>
        /// Performs legacy type conversions.
        /// </summary>
        private static object SmartConvert(object value, Type targetType)
        {
            if (value is DateOnly dateOnly)
            {
                return HandleDateOnlyConversion(dateOnly, targetType);
            }

            if (value is DateTime dateTime)
            {
                return HandleDateTimeConversion(dateTime, targetType);
            }

            if (value is string dateString)
            {
                return HandleStringConversion(dateString, targetType);
            }

            if (IsMinValue(value))
            {
                return ConvertMinValueToNull(value, targetType);
            }

            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (InvalidCastException)
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    try
                    {
                        return Convert.ChangeType(value, underlyingType);
                    }
                    catch
                    {
                    }
                }

                throw new InvalidOperationException(
                    $"Cannot convert {value.GetType()} to {targetType}");
            }
        }

        /// <summary>
        /// Checks whether a value is treated as a legacy minimum sentinel.
        /// </summary>
        private static bool IsMinValue(object value)
        {
            return value switch
            {
                DateTime dt => dt == DateTime.MinValue || dt == new DateTime(1900, 1, 1),
                DateOnly d => d == DateOnly.MinValue || d == new DateOnly(1900, 1, 1),
                DateTimeOffset dto => dto == DateTimeOffset.MinValue || dto == new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero),
                _ => false
            };
        }

        /// <summary>
        /// Converts legacy minimum sentinel values to null or empty string.
        /// </summary>
        private static object ConvertMinValueToNull(object value, Type targetType)
        {
            if (Nullable.GetUnderlyingType(targetType) != null)
            {
                return null;
            }

            if (targetType == typeof(string))
            {
                return string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Handles DateOnly conversions.
        /// </summary>
        private static object HandleDateOnlyConversion(DateOnly dateOnly, Type targetType)
        {
            if (IsMinValue(dateOnly))
            {
                return ConvertMinValueToNull(dateOnly, targetType);
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            switch (underlyingType.Name)
            {
                case nameof(DateTime):
                    return dateOnly.ToDateTime(TimeOnly.MinValue);

                case nameof(DateTimeOffset):
                    return new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue));

                case nameof(String):
                    return dateOnly.ToString("yyyy-MM-dd");

                case nameof(DateOnly):
                    return dateOnly;

                default:
                    throw new InvalidCastException($"Unsupported DateOnly conversion to {targetType}");
            }
        }

        /// <summary>
        /// Handles DateTime conversions.
        /// </summary>
        private static object HandleDateTimeConversion(DateTime dateTime, Type targetType)
        {
            if (IsMinValue(dateTime))
            {
                return ConvertMinValueToNull(dateTime, targetType);
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            switch (underlyingType.Name)
            {
                case nameof(DateOnly):
                    return DateOnly.FromDateTime(dateTime);

                case nameof(DateTimeOffset):
                    return new DateTimeOffset(dateTime);

                case nameof(String):
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                case nameof(DateTime):
                    return dateTime;

                default:
                    return dateTime;
            }
        }

        /// <summary>
        /// Handles string-to-date conversions.
        /// </summary>
        private static object HandleStringConversion(string dateString, Type targetType)
        {
            if (DateTime.TryParse(dateString, out DateTime dt))
            {
                return HandleDateTimeConversion(dt, targetType);
            }

            if (DateOnly.TryParse(dateString, out DateOnly d))
            {
                return HandleDateOnlyConversion(d, targetType);
            }

            if (string.IsNullOrWhiteSpace(dateString))
            {
                return ConvertMinValueToNull(DateTime.MinValue, targetType);
            }

            throw new FormatException($"Invalid date string: {dateString}");
        }

        /// <summary>
        /// Determines whether source and target types still need a custom conversion.
        /// </summary>
        private static bool NeedConversion(Type sourceType, Type targetType)
        {
            var underlyingSource = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var underlyingTarget = Nullable.GetUnderlyingType(targetType) ?? targetType;

            return underlyingSource != underlyingTarget;
        }
    }
}
