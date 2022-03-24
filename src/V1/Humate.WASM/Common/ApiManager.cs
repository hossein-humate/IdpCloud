using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Serialization;

namespace Humate.WASM.Common
{
    public static class ApiManager
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken = default) where T : class
        {
            var json = await content.ReadAsStringAsync(cancellationToken);
            if (!json.IsValidJson()) return json as T;
            var value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }

        public static async Task<Stream> GetPdfAsync(string baseAddress, string urlPathAndQuery,
            IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
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
                    if (response.IsSuccessStatusCode)
                    {
                        var obj = await response.Content.ReadAsStreamAsync(cancellationToken);
                        return obj;
                    }
                }
                catch (Exception)
                {
                    return default;
                }
            }
            return default;
        }

        public static async Task<Stream> PostReturnPdfAsync<TParam>(string baseAddress, string urlPath,
            TParam param, string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default, NamingStrategy namingStrategy = null)
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != default)
            {
                client.DefaultRequestHeaders.Add("Security-Token", token);
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
                return await response.Content.ReadAsStreamAsync(cancellationToken);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> GetAsync<T>(string baseAddress, string urlPathAndQuery,
            string token = default, IDictionary<string, string> headers = default,
             CancellationToken cancellationToken = default) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != default)
            {
                client.DefaultRequestHeaders.Add("Security-Token", token);
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
                var obj = await response.Content.ReadAsJsonAsync<T>(cancellationToken);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> DeleteAsync<T>(string baseAddress, string urlPathAndQuery,
            string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != default)
            {
                client.DefaultRequestHeaders.Add("Security-Token", token);
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
                var obj = await response.Content.ReadAsJsonAsync<T>(cancellationToken);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> PostAsync<T, TParam>(string baseAddress, string urlPath,
            TParam param, string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default, NamingStrategy namingStrategy = null) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != default)
            {
                client.DefaultRequestHeaders.Add("Security-Token", token);
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
                return await response.Content.ReadAsJsonAsync<T>(cancellationToken);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> PutAsync<T, TParam>(string baseAddress, string urlPath,
            TParam param, string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default, NamingStrategy namingStrategy = null) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != default)
            {
                client.DefaultRequestHeaders.Add("Security-Token", token);
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
                var obj = await response.Content.ReadAsJsonAsync<T>(cancellationToken);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> PatchAsync<T, TParam>(string baseAddress, string urlPath,
            TParam param, string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default, NamingStrategy namingStrategy = null) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != default)
            {
                client.DefaultRequestHeaders.Add("Security-Token", token);
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
                var response = await client.PatchAsync(urlPath, data, cancellationToken);
                var obj = await response.Content.ReadAsJsonAsync<T>(cancellationToken);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> OptionsAsync<T>(string baseAddress, string urlPath,
            string token = default, IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default, NamingStrategy namingStrategy = null) where T : class
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != default)
            {
                client.DefaultRequestHeaders.Add("Security-Token", token);
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
                var obj = await response.Content.ReadAsJsonAsync<T>(cancellationToken);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<T> PostFormUrlEncodedAsync<T>(string baseAddress, string urlPath,
            IDictionary<string, string> parameters, IDictionary<string, string> headers = default,
           CancellationToken cancellationToken = default) where T : class
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
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
                    var data = new FormUrlEncodedContent(parameters);
                    var response = await client.PostAsync(urlPath, data, cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var obj = await response.Content.ReadAsJsonAsync<T>(cancellationToken);
                        return obj;
                    }
                }
                catch (Exception)
                {
                    return default;
                }
            }
            return default;
        }
    }
}
