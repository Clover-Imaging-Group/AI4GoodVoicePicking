using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConversationalAI.API.Helpers
{
    public class GraphQLHelper
    {
        static string currentBearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiY29uY3VycmVuY3lfcm9fZ3JvdXAiLCJleHAiOjE2NTA0NjYyODcsInVzZXJfbmFtZSI6ImFrcmlzaCIsImF1dGhlbnRpY2F0b3IiOiJjb25jdXJyZW5jeSIsImF1dGhlbnRpY2F0aW9uX2lkIjoiMTUiLCJpYXQiOjE2NTA0NjQ0ODYsImF1ZCI6InBvc3RncmFwaGlsZSIsImlzcyI6InBvc3RncmFwaGlsZSJ9.HbiSRygSgLXK8vhYYInvMxWF8zVeRNN8sDkV8u1RvUA";
        private static readonly string InstanceGraphQlEndpoint = "https://concurrency.cesmii.net/graphql";
        static readonly string clientId = "akrish";
        // static readonly string clientSecret = "QFcS@PHAFmk2kx";
        private static readonly string clientSecret = "QFcS@PHAFmk2kx";
        static readonly string userName = "akrish";
        static readonly string role = "concurrency_group";
        
        
        /// <summary>
        /// Forms and sends a GraphQL request (query or mutation) and returns the response
        /// </summary>
        /// <param name="content">The JSON payload you want to send to GraphQL</param>
        /// <returns>The JSON payload returned from the Server</returns>
        public static async Task<string> PerformGraphQLRequest(string content)
        {
            try
            {
                var graphQlClient = new GraphQLHttpClient(InstanceGraphQlEndpoint, new NewtonsoftJsonSerializer());

                GraphQLRequest dataRequest = new GraphQLRequest() { Query = content };
                graphQlClient.HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", currentBearerToken);
                GraphQLResponse<JObject> dataResponse = await graphQlClient.SendQueryAsync<JObject>(dataRequest);
                
                JObject data = dataResponse.Data;
                return data.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                //An exception was thrown indicating the current bearer token is no longer allow.
                //  Authenticate and get a new token, then try again
                if (ex.Message.ToLower().IndexOf("forbidden", StringComparison.Ordinal) != -1 || ex.Message.ToLower().IndexOf("unauthorized", StringComparison.Ordinal) != -1 || ex.Message.ToLower().IndexOf("badrequest") != -1)
                {
                    Console.WriteLine("Bearer Token expired!");
                    Console.WriteLine("Attempting to retreive a new GraphQL Bearer Token...");
                    Console.WriteLine();

                    //Authenticate
                    JwtSecurityToken newToken = await GetBearerToken(InstanceGraphQlEndpoint);
                    currentBearerToken = newToken.RawData;

                    Console.WriteLine($"New Token received: {newToken.EncodedPayload}");
                    Console.WriteLine($"New Token valid until: {newToken.ValidTo.ToLocalTime()}");
                    Console.WriteLine();

                    //Re-try our data request, using the updated bearer token
                    return await PerformGraphQLRequest(content);
                }
                else
                {
                    Console.WriteLine("An error occured accessing the SM Platform!");
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }
        }

        public static async Task<string> PerformGraphQLMutation(string content)
        {
            try
            {
                var graphQlClient = new GraphQLHttpClient(InstanceGraphQlEndpoint, new NewtonsoftJsonSerializer());

                GraphQLRequest dataRequest = new GraphQLRequest { Query = content };
                graphQlClient.HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", currentBearerToken);
                GraphQLResponse<JObject> dataResponse = await graphQlClient.SendMutationAsync<JObject>(dataRequest);
                
                JObject data = dataResponse.Data;
                return data.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                //An exception was thrown indicating the current bearer token is no longer allow.
                //  Authenticate and get a new token, then try again
                if (ex.Message.ToLower().IndexOf("forbidden", StringComparison.Ordinal) != -1 || ex.Message.ToLower().IndexOf("unauthorized", StringComparison.Ordinal) != -1 || ex.Message.ToLower().IndexOf("badrequest") != -1)
                {
                    Console.WriteLine("Bearer Token expired!");
                    Console.WriteLine("Attempting to retreive a new GraphQL Bearer Token...");
                    Console.WriteLine();

                    //Authenticate
                    JwtSecurityToken newToken = await GetBearerToken(InstanceGraphQlEndpoint);
                    currentBearerToken = newToken.RawData;

                    Console.WriteLine($"New Token received: {newToken.EncodedPayload}");
                    Console.WriteLine($"New Token valid until: {newToken.ValidTo.ToLocalTime()}");
                    Console.WriteLine();

                    //Re-try our data request, using the updated bearer token
                    return await PerformGraphQLMutation(content);
                }
                else
                {
                    Console.WriteLine("An error occured accessing the SM Platform!");
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }
        }
        
        
        /// <summary>
        /// Gets a JWT Token containing the Bearer string returned from the Platform, assuming authorization is granted.
        /// </summary>
        /// <param name="endPoint">The URL of the GraphQL endpoint</param>
        /// <returns>A valid JWT (or an exception if one can't be found! Add some error handling)</returns>
        public static async Task<JwtSecurityToken> GetBearerToken(string endPoint)
        {
            var graphQLClient = new GraphQLHttpClient(endPoint, new NewtonsoftJsonSerializer());

            // Step 1: Request a challenge
            string authRequestQuery = @$"
                mutation authRequest {{
                  authenticationRequest(input: {{authenticator: ""{clientId}"", role: ""{role}"", userName: ""{userName}""}}) {{
                    jwtRequest {{
                      challenge
                      message
                    }}
                  }}
                }}";

            GraphQLRequest authRequest = new GraphQLRequest() { Query = authRequestQuery };
            GraphQLResponse<JObject> authResponse = await graphQLClient.SendQueryAsync<JObject>(authRequest);
            string challenge = authResponse.Data["authenticationRequest"]?["jwtRequest"]?["challenge"].Value<string>();
            Console.WriteLine($"Challenge received: {challenge}");

            // Step 2: Get token
            var authValidationQuery = @$"
                mutation authValidation {{
                  authenticationValidation(input: {{authenticator: ""{clientId}"", signedChallenge: ""{challenge}|{clientSecret}""}}) {{
                    jwtClaim
                  }}
                }}";

            GraphQLRequest validationRequest = new GraphQLRequest() { Query = authValidationQuery };
            GraphQLResponse<JObject> validationQLResponse = await graphQLClient.SendQueryAsync<JObject>(validationRequest);
            var tokenString = validationQLResponse.Data["authenticationValidation"]["jwtClaim"].Value<string>();
            var newJwtToken = new JwtSecurityToken(tokenString);
            return newJwtToken;

            //TODO: Handle errors!
        }
    }
}