using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Advice.Ranoi.Core.Security.Domain.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Security.Domain
{
    public class Token : IToken
    {
        public Guid UserId { get; private set; }
        public String UserName { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }

        public DateTimeOffset IssuedAt { get; private set; }
        public String Issuer { get; private set; }

        private String SecretKey { get; set; }
        private IAdviceDateTimeProvider DateTime { get; set; }

        public Boolean Valid { get; set; }
        public String Error { get; set; }

        private Dictionary<String, String> CustomClaims { get; set; }

        public Token(String secretKey, IAdviceDateTimeProvider dateProvider)
        {
            this.CustomClaims = new Dictionary<string, String>();
            this.SecretKey = secretKey;
            this.DateTime = dateProvider;
        }

        public Token(String secretKey, IAdviceDateTimeProvider dateProvider, Guid userId, String userName) : this(secretKey, dateProvider)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.IssuedAt = DateTime.GetNow();
            this.ExpiresAt = DateTime.GetNow().AddMinutes(15);
        }

        public Token(String secretKey, IAdviceDateTimeProvider dateProvider, String token) : this(secretKey, dateProvider)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IJwtValidator validator = new JwtValidator(serializer, this.DateTime);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            try
            {
                var json = decoder.Decode(token, this.SecretKey, verify: true);

                var jsonObject = JObject.Parse(json);

                this.UserId = Guid.Parse(jsonObject["userId"].Value<String>());
                this.UserName = jsonObject["userName"].Value<String>();
                this.IssuedAt = ConvertFromUnixTimestamp(jsonObject["iat"].Value<Int64>());
                this.ExpiresAt = ConvertFromUnixTimestamp(jsonObject["exp"].Value<Int64>());

                foreach (var child in jsonObject.Children())
                {
                    if (child.Path.Equals("userId") || child.Path.Equals("userName") || child.Path.Equals("iat") || child.Path.Equals("exp"))
                        continue;

                    //JProperty childProperty = child as JProperty;

                    //if(childProperty != null)
                    //    this.CustomClaims.Add(childProperty.Name, childProperty.Value<String>());

                    if (child.Type == JTokenType.Property)
                    {
                        var childProperty = (JProperty)child;
                        this.CustomClaims.Add(childProperty.Name, child.First.Value<String>());
                    }
                    else
                    {
                        this.CustomClaims.Add(child.Path, child.First.Value<String>());
                    }
                }

                this.Valid = true;

            }
            catch (TokenExpiredException)
            {
                this.Error = "Token Expired";
                var json = decoder.Decode(token, this.SecretKey, false);
                var jsonObject = JObject.Parse(json);
                this.UserId = Guid.Parse(jsonObject["userId"].Value<String>());
                this.UserName = jsonObject["userName"].Value<String>();
            }
            catch (SignatureVerificationException)
            {
                this.Error = "Token Signature Failure";
            }
        }

        public String GetToken()
        {
            var payload = new Dictionary<string, object>
            {
                { "userId", UserId },
                { "userName", UserName },
                { "exp", ConvertToUnixTimestamp(ExpiresAt) },
                { "iat", ConvertToUnixTimestamp(IssuedAt) }
            };

            foreach (var customClaim in CustomClaims)
            {
                payload.Add(customClaim.Key, customClaim.Value);
            }

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, this.SecretKey);

            return token;
        }

        public void SetClaim(String name, String value)
        {
            if (this.CustomClaims.ContainsKey(name))
                this.CustomClaims[name] = value;
            else
                this.CustomClaims.Add(name, value);
        }

        public string GetClaim(String name)
        {
            return this.CustomClaims[name];
        }

        private DateTime ConvertFromUnixTimestamp(Int64 timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        private Int64 ConvertToUnixTimestamp(DateTimeOffset date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (Int64)Math.Floor(diff.TotalSeconds);
        }
    }
}
