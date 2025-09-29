// Copyright (c) SC Strategic Solutions. All rights reserved.

using Domain.SharedKernel;
using System.Collections.Concurrent;
using System.Text;

namespace Domain.Common;

public static class IdentifierExtensions
{
    private static readonly ConcurrentDictionary<Type, Type> KeyTypeCache = new();

    public static string Compress(this Guid guid)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        guid.TryWriteBytes(guidBytes);

        Span<char> base64Chars = stackalloc char[24];
        Convert.TryToBase64Chars(guidBytes, base64Chars, out var charsWritten);

        var compressedBuilder = new StringBuilder(22);

        for (var i = 0; i < charsWritten; i++)
        {
            if (base64Chars[i] == '+')
            {
                compressedBuilder.Append('-');
            }
            else if (base64Chars[i] == '/')
            {
                compressedBuilder.Append('_');
            }
            else if (base64Chars[i] != '=')
            {
                compressedBuilder.Append(base64Chars[i]);
            }
        }

        return compressedBuilder.ToString();
    }

    public static bool TryDecompress(string compressed, out Guid guid)
    {
        try
        {
            guid = DecompressToGuid(compressed);
            return true;
        }
        catch
        {
            guid = Guid.Empty;
            return false;
        }
    }

    public static Guid DecompressToGuid(string compressed)
    {
        var base64Builder = new StringBuilder(24);

        foreach (var t in compressed)
        {
            switch (t)
            {
                case '-':
                    base64Builder.Append('+');
                    break;
                case '_':
                    base64Builder.Append('/');
                    break;
                default:
                    base64Builder.Append(t);
                    break;
            }
        }

        if (base64Builder.Length % 4 != 0)
        {
            base64Builder.Append('=', 4 - base64Builder.Length % 4);
        }

        Span<byte> guidBytes = stackalloc byte[16];
        Convert.TryFromBase64String(base64Builder.ToString(), guidBytes, out _);

        return new Guid(guidBytes);
    }

    /// <summary>
    ///     Asserts that the provided id is of the expected type for the given storage type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void AssertIdType<T>(UniqueId id) where T : IDocumentStorage
    {
        var expectedKeyType = GetExpectedKeyType<T>();

        if (!expectedKeyType.IsInstanceOfType(id))
        {
            throw new ArgumentException(
                $"The id provided UniqueId does not match the expected key type for {typeof(T).Name}. Expected: {expectedKeyType}, Actual: {id.GetType()}"
            );
        }
    }

    private static Type GetExpectedKeyType<T>() where T : IDocumentStorage
    {
        return KeyTypeCache.GetOrAdd(
            typeof(T), static type =>
            {
                var storageInterface =
                    type.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDocumentStorage<>))
                    ?? throw new InvalidOperationException($"Type {typeof(T).Name} does not implement IDocumentStorage<>");

                return storageInterface.GetGenericArguments()[0];
            }
        );
    }


}
