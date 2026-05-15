using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Common
{
    public static class UrlHelper
    {
        /// <summary>
        /// 从 BaseAddress 中提取域名（不包含 API 路径）
        /// </summary>
        /// <param name="baseAddress">完整的 BaseAddress</param>
        /// <returns>提取的域名</returns>
        public static string ExtractDomainFromBaseAddress(string baseAddress)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                return string.Empty;
            }

            try
            {
                // 确保地址是有效的 URI
                if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out Uri uri))
                {
                    // 如果不是完整 URI，尝试添加协议
                    if (!baseAddress.StartsWith("http://") && !baseAddress.StartsWith("https://"))
                    {
                        var httpsAddress = "https://" + baseAddress;
                        if (Uri.TryCreate(httpsAddress, UriKind.Absolute, out uri))
                        {
                            return $"{uri.Scheme}://{uri.Host}";
                        }
                    }
                    return baseAddress; // 返回原始地址作为后备方案
                }

                // 提取协议和主机名
                return $"{uri.Scheme}://{uri.Host}";
            }
            catch (Exception)
            {
                return baseAddress; // 如果解析失败，返回原始地址
            }
        }

        /// <summary>
        /// 获取干净的域名（不包含协议）
        /// </summary>
        public static string ExtractCleanDomain(string baseAddress)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                return string.Empty;
            }

            try
            {
                if (Uri.TryCreate(baseAddress, UriKind.Absolute, out Uri uri))
                {
                    return uri.Host;
                }

                // 尝试处理不完整的地址
                var cleanAddress = baseAddress
                    .Replace("https://", "")
                    .Replace("http://", "")
                    .Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(); // 取第一个部分（域名）

                return cleanAddress;
            }
            catch (Exception)
            {
                return baseAddress;
            }
        }
    }
}
