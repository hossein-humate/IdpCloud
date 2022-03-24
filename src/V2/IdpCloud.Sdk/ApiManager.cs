using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdpCloud.Sdk.Auth;

namespace IdpCloud.Sdk
{
    public class ApiManager
    {
        private readonly ClientOptions _options;

        public const string HeaderClientId = "X-ClientId";
        public const string HeaderSecretKey = "X-Secret-Key";
        public const string HeaderRemoteIpAddress = "X-Proxy-Client-Remote-Ip-Address";
        public const string HeaderSecurityToken = "X-Security-Token";

        public ApiManager(ClientOptions options)
        {
            _options = options;
        }

        public async Task<T> ReadAsJsonAsync<T>(HttpContent content) where T : class
        {
            var json = await content.ReadAsStringAsync();
            if (!IsValidJson(json))
            {
                return json as T;
            }

            var value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }

        public async Task<T> GetAsync<T>(string baseAddress, string urlPathAndQuery,
            string token = default, IDictionary<string, string> headers = default,
             CancellationToken cancellationToken = default) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(HeaderClientId, _options.ClientId);
            if (token != default)
            {
                client.DefaultRequestHeaders.Add(HeaderSecurityToken, token);
            }
            if (headers != default)
            {
                foreach (var item in headers)
                {
                    if (client.DefaultRequestHeaders.Contains(item.Key))
                    {
                        client.DefaultRequestHeaders.Remove(item.Key);
                    }

                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            try
            {
                var response = await client.GetAsync(urlPathAndQuery, cancellationToken);
                var obj = await ReadAsJsonAsync<T>(response.Content);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> DeleteAsync<T>(string baseAddress, string urlPathAndQuery,
            string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(HeaderClientId, _options.ClientId);
            if (token != default)
            {
                client.DefaultRequestHeaders.Add(HeaderSecurityToken, token);
            }
            if (headers != default)
            {
                foreach (var item in headers)
                {
                    if (client.DefaultRequestHeaders.Contains(item.Key))
                    {
                        client.DefaultRequestHeaders.Remove(item.Key);
                    }

                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            try
            {
                var response = await client.DeleteAsync(urlPathAndQuery, cancellationToken);
                var obj = await ReadAsJsonAsync<T>(response.Content);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> PostAsync<T, TParam>(string baseAddress, string urlPath,
            TParam param, string token = default, IDictionary<string, string> headers = default,
            NamingStrategy namingStrategy = null, CancellationToken cancellationToken = default) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(HeaderClientId, _options.ClientId);
            if (token != default)
            {
                client.DefaultRequestHeaders.Add(HeaderSecurityToken, token);
            }
            if (headers != default)
            {
                foreach (var item in headers)
                {
                    if (client.DefaultRequestHeaders.Contains(item.Key))
                    {
                        client.DefaultRequestHeaders.Remove(item.Key);
                    }

                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            try
            {

                var json = namingStrategy == null ? JsonConvert.SerializeObject(param) :
                    JsonConvert.SerializeObject(param, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy },
                        Formatting = Formatting.Indented
                    });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(urlPath, data, cancellationToken);
                return await ReadAsJsonAsync<T>(response.Content);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> PutAsync<T, TParam>(string baseAddress, string urlPath,
            TParam param, string token = default, IDictionary<string, string> headers = default,
            NamingStrategy namingStrategy = null, CancellationToken cancellationToken = default) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(HeaderClientId, _options.ClientId);
            if (token != default)
            {
                client.DefaultRequestHeaders.Add(HeaderSecurityToken, token);
            }
            if (headers != default)
            {
                foreach (var item in headers)
                {
                    if (client.DefaultRequestHeaders.Contains(item.Key))
                    {
                        client.DefaultRequestHeaders.Remove(item.Key);
                    }

                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            try
            {
                var json = namingStrategy == null ? JsonConvert.SerializeObject(param) :
                    JsonConvert.SerializeObject(param, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy },
                        Formatting = Formatting.Indented
                    });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(urlPath, data, cancellationToken);
                var obj = await ReadAsJsonAsync<T>(response.Content);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> OptionsAsync<T>(string baseAddress, string urlPath,
            string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(HeaderClientId, _options.ClientId);
            if (token != default)
            {
                client.DefaultRequestHeaders.Add(HeaderSecurityToken, token);
            }
            if (headers != default)
            {
                foreach (var item in headers)
                {
                    if (client.DefaultRequestHeaders.Contains(item.Key))
                    {
                        client.DefaultRequestHeaders.Remove(item.Key);
                    }

                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Options, urlPath);
                var response = await client.SendAsync(httpRequest, cancellationToken);
                var obj = await ReadAsJsonAsync<T>(response.Content);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        #region Methods
        public bool IsValidJson(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();
            if (value.StartsWith("{", StringComparison.Ordinal) &&
                value.EndsWith("}", StringComparison.Ordinal) || //For object
                value.StartsWith("[", StringComparison.Ordinal) &&
                value.EndsWith("]", StringComparison.Ordinal)) //For array
            {
                try
                {
                    var _ = JToken.Parse(value);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
        #endregion
    }
}
