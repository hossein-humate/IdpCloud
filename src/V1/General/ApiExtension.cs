using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General
{
    public static class ApiExtension
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content) where T : class
        {
            var json = await content.ReadAsStringAsync();
            if (json.IsValidJson())
            {
                var value = JsonConvert.DeserializeObject<T>(json);
                return value;
            }

            return json as T;
        }

        public static async Task<Stream> GetPdfAsync(string baseAddress, string urlPathAndQuery,
            IDictionary<string, string> headers = default,
            CancellationToken cancellationToken = default)
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            using (var client = new HttpClient(clientHandler))
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
                        var obj = await response.Content.ReadAsStreamAsync();
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
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            using var client = new HttpClient(clientHandler) { BaseAddress = new Uri(baseAddress) };
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
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            using var client = new HttpClient(clientHandler) { BaseAddress = new Uri(baseAddress) };
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
                var obj = await response.Content.ReadAsJsonAsync<T>();
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
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            using var client = new HttpClient(clientHandler) { BaseAddress = new Uri(baseAddress) };
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
                return await response.Content.ReadAsJsonAsync<T>();
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
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            using var client = new HttpClient(clientHandler) { BaseAddress = new Uri(baseAddress) };
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
                var obj = await response.Content.ReadAsJsonAsync<T>();
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
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            using var client = new HttpClient(clientHandler) { BaseAddress = new Uri(baseAddress) };
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
                var obj = await response.Content.ReadAsJsonAsync<T>();
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
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => true
            };
            using (var client = new HttpClient(clientHandler))
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
                        var obj = await response.Content.ReadAsJsonAsync<T>();
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