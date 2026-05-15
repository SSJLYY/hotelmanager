using EOM.TSHotelManagement.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Common
{
    public class LskyHelper
    {
        private const string LskyHttpClientName = "HeartBeatCheckClient";
        private readonly LskyConfigFactory lskyConfigFactory;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<LskyHelper> logger;

        public LskyHelper(
            LskyConfigFactory lskyConfigFactory,
            IHttpClientFactory httpClientFactory,
            ILogger<LskyHelper> logger)
        {
            this.lskyConfigFactory = lskyConfigFactory;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<bool> CheckServiceStatusAsync()
        {
            var lskyConfig = lskyConfigFactory.GetLskyConfig();
            var address = lskyConfig.BaseAddress;
            var enabled = lskyConfig.Enabled;

            try
            {
                if (!enabled)
                {
                    logger.LogWarning("Lsky图床服务未启用，跳过状态检查。");
                    return false;
                }

                var domain = UrlHelper.ExtractDomainFromBaseAddress(lskyConfig.BaseAddress);

                if (string.IsNullOrWhiteSpace(domain))
                {
                    logger.LogError("无法从BaseAddress中提取有效域名: {BaseAddress}", lskyConfig.BaseAddress);
                    return false;
                }

                logger.LogInformation("使用域名进行健康检查: {Domain}", domain);

                var httpClient = httpClientFactory.CreateClient(LskyHttpClientName);
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, domain));

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Lsky图床服务状态检查成功，状态码: {StatusCode}", response.StatusCode);
                    return true;
                }
                else
                {
                    logger.LogWarning("Lsky图床服务返回异常状态码: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                logger.LogError(httpEx, "HTTP请求异常，Lsky图床服务可能不可用");
                return false;
            }
            catch (TaskCanceledException timeoutEx) when (timeoutEx.CancellationToken == CancellationToken.None)
            {
                logger.LogError(timeoutEx, "Lsky图床服务连接超时");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lsky图床服务状态检查发生未知异常");
                return false;
            }
        }

        public async Task<bool> GetEnabledState()
        {
            logger.LogError("Checking if Lsky image storage is enabled.");
            return lskyConfigFactory.GetLskyConfig().Enabled;
        }

        public async Task<string> GetImageStorageTokenAsync()
        {
            var lskConfig = lskyConfigFactory.GetLskyConfig();
            var httpClient = httpClientFactory.CreateClient(LskyHttpClientName);
            var tokenRequest = new
            {
                email = lskConfig.Email,
                password = lskConfig.Password
            };

            var response = await httpClient.PostAsJsonAsync($"{lskConfig.BaseAddress}{lskConfig.GetTokenApi}", tokenRequest);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return result?.Status == true ? result.Data.Token : null;
        }

        public async Task<string> UploadImageAsync(
                Stream fileStream,
                string fileName,
                string contentType,
                string token,
                int? strategyId = null)
        {
            try
            {
                if (fileStream == null || fileStream.Length == 0)
                    throw new ArgumentException(LocalizationHelper.GetLocalizedString("File stream cannot be empty", "文件流不能为空"));

                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException(LocalizationHelper.GetLocalizedString("File name cannot be empty", "文件名不能为空"));

                if (string.IsNullOrWhiteSpace(contentType))
                    throw new ArgumentException(LocalizationHelper.GetLocalizedString("Content type cannot be empty", "内容类型不能为空"));

                var lskConfig = lskyConfigFactory.GetLskyConfig();
                if (string.IsNullOrEmpty(lskConfig?.BaseAddress))
                    throw new InvalidOperationException(LocalizationHelper.GetLocalizedString("The base URL for the Lsky service is not configured.", "兰空图床基础地址未配置"));

                var httpClient = httpClientFactory.CreateClient(LskyHttpClientName);
                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                content.Add(fileContent, "file", fileName);

                if (strategyId.HasValue)
                {
                    content.Add(new StringContent(strategyId.Value.ToString()), "strategy_id");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.PostAsync(
                    $"{lskConfig.BaseAddress}{lskConfig.UploadApi}",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"上传失败: {response.StatusCode} - {errorContent}");
                }

                var result = await response.Content.ReadFromJsonAsync<UploadResponse>();
                return result?.Data?.Links?.Url ?? throw new Exception("无效图片，请重新选择图片上传");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        public class TokenResponse
        {
            public bool Status { get; set; }
            public TokenData Data { get; set; }
        }

        public class TokenData
        {
            public string Token { get; set; }
        }

        public class UploadResponse
        {
            public UploadData Data { get; set; }
        }

        public class UploadData
        {
            public UploadLinks Links { get; set; }
        }

        public class UploadLinks
        {
            public string Url { get; set; }
        }

    }
}
